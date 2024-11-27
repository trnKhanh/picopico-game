using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    static public SceneLoadingManager Instance { get; private set; }

    public enum SceneType
    {
        Menu,
        Tutorial,
    }

    [Serializable]
    public class Scene
    {
        public SceneType sceneType;
        public string sceneName;
    }

    [SerializeField] private Scene[] scenes;

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
        m_curSceneName = SceneManager.GetActiveScene().name;
    }

    public void ReloadScene()
    {
        if (m_curSceneName != null)
        {
            SceneManager.LoadScene(m_curSceneName);
        }
    }

    public void LoadScene(SceneType sceneType)
    {
        Debug.Log("Load Scene");

        string sceneName = GetSceneName(sceneType);

        if (sceneName != null)
        {
            SceneManager.LoadSceneAsync(sceneName);
            m_curSceneName = sceneName;
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
}
