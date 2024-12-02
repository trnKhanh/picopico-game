using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingScreenUI : MonoBehaviour
{
    static public LoadingScreenUI Instance { get; private set; }

    [SerializeField] private RectTransform loadingScreenPanel;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void Show()
    {
        loadingScreenPanel.gameObject.SetActive(true);
    }

    public void Hide()
    {
        loadingScreenPanel.gameObject.SetActive(false);
    }
}
