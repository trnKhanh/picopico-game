using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : NetworkBehaviour
{
    static public SceneLoadingManager Instance { get; private set; }

    public enum SceneType
    {
        Menu,
        Tutorial,
        Credit,
        UNKNOWN,
    }

    [Serializable]
    public class Scene
    {
        public SceneType sceneType;
        public string sceneName;
    }

    [SerializeField] private Scene[] scenes;

    public SceneType localDefaultType = SceneType.Menu;
    public SceneType multiplayerDefaultScene = SceneType.Menu;

    private string m_curSceneName;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(Instance);
    }

    private void OnEnable()
    {
        StartCoroutine(SubribeToNetworkManagerEvents());
    }

    private void OnDisable()
    {
        UnSubribeToNetworkManagerEvents();
    }

    private IEnumerator SubribeToNetworkManagerEvents()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        UnSubribeToNetworkManagerEvents();
        NetworkManager.Singleton.OnClientStopped += NetworkManager_OnClientStopped;
    }

    private void UnSubribeToNetworkManagerEvents()
    {
        NetworkManager.Singleton.OnClientStopped -= NetworkManager_OnClientStopped;
    }

    private void NetworkManager_OnClientStopped(bool isHost)
    {
        LoadLocalDefaultScene();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SubcribeToNetworkSceneManagerEvents();
            LoadScene(multiplayerDefaultScene, true, true);
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            UnSubcribeToNetworkSceneManagerEvents();
        }
    }

    // NetworkSceneManager Events
    private void SubcribeToNetworkSceneManagerEvents()
    {
        UnSubcribeToNetworkSceneManagerEvents();
        NetworkSceneManager networkSceneManager = NetworkManager.Singleton.SceneManager;
        networkSceneManager.OnLoadComplete += NetworkSceneManager_OnLoadComplete;
    }

    private void UnSubcribeToNetworkSceneManagerEvents()
    {
        NetworkSceneManager networkSceneManager = NetworkManager.Singleton.SceneManager;
        networkSceneManager.OnLoadComplete -= NetworkSceneManager_OnLoadComplete;
    }

    private void NetworkSceneManager_OnLoadComplete(ulong clientId, string sceneName, LoadSceneMode loadSceneMode)
    {
        SceneType sceneType = GetSceneType(sceneName);
        switch (sceneType)
        {
            case SceneType.Tutorial:
                SpawnManager.Instance.SpawnPlayer(clientId);
                break;
        }
    }

    [ClientRpc]
    private void ChangeSceneClientRpc(string sceneName)
    {
        m_curSceneName = sceneName;
    }

    public void LoadLocalDefaultScene()
    {
        LoadScene(localDefaultType, true, false);
    }

    public void LoadMultiplayerDefaultScene()
    {
        LoadScene(multiplayerDefaultScene, true, true);
    }

    public void ReloadScene(bool useNetwork = true)
    {
        LoadScene(m_curSceneName, useNetwork);
    }

    public void LoadScene(SceneType sceneType, bool force, bool useNetwork)
    {
        string sceneName = GetSceneName(sceneType);
        if (!force && sceneName == m_curSceneName)
            return;

        LoadScene(sceneName, useNetwork);
    }

    private void LoadScene(string sceneName, bool useNetwork = true)
    {
        if (sceneName != null)
        {
            LoadingScreenUI.Instance.Show();
            m_curSceneName = sceneName;

            if (!useNetwork)
            {
                SceneManager.LoadSceneAsync(m_curSceneName, LoadSceneMode.Single);
            }
            else
            {
                try
                {
                    if (!IsServer)
                        throw new Exception("Only Server is allowed to change scene");

                    NetworkSceneManager networkSceneManager = NetworkManager.Singleton.SceneManager;
                    networkSceneManager.LoadScene(m_curSceneName, LoadSceneMode.Single);
                }
                catch (Exception e)
                {
                    Debug.Log($"Try loading scene using {nameof(NetworkSceneManager)} but get \"{e.Message}\". Load local scene instead.");
                    SceneManager.LoadSceneAsync(m_curSceneName, LoadSceneMode.Single);
                }
            }
        }
    }

    private string GetSceneName(SceneType sceneType)
    {
        foreach (Scene scene in scenes)
        {
            if (scene.sceneType == sceneType)
            {
                return scene.sceneName;
            }
        }
        return null;
    }

    private SceneType GetSceneType(string sceneName)
    {
        foreach (Scene scene in scenes)
        {
            if (scene.sceneType.ToString().ToLower() == sceneName.ToLower())
            {
                return scene.sceneType;
            }
        }
        return SceneType.UNKNOWN;
    }
}
