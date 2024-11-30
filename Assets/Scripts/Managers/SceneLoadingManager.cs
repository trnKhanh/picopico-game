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
        UNKNOWN,
    }

    [Serializable]
    public class Scene
    {
        public SceneType sceneType;
        public string sceneName;
    }

    [SerializeField] private Scene[] scenes;

    public SceneType defaultScene = SceneType.Menu;

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

    private void Start()
    {
        LoadScene(defaultScene, false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkSceneManager networkSceneManager = NetworkManager.Singleton.SceneManager;
            networkSceneManager.OnLoadComplete -= NetworkSceneManager_OnLoadComplete;
            networkSceneManager.OnLoadComplete += NetworkSceneManager_OnLoadComplete;

            LoadScene(defaultScene, true);
        }
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            NetworkSceneManager networkSceneManager = NetworkManager.Singleton.SceneManager;
            networkSceneManager.OnLoadComplete -= NetworkSceneManager_OnLoadComplete;
        }
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

    public void ReloadScene(bool useNetwork = true)
    {
        LoadScene(m_curSceneName, useNetwork);
    }

    public void LoadScene(SceneType sceneType, bool useNetwork = true)
    {
        string sceneName = GetSceneName(sceneType);

        LoadScene(sceneName, useNetwork);
    }
    public void LoadScene(string sceneName, bool useNetwork = true)
    {
        if (sceneName != null)
        {
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
