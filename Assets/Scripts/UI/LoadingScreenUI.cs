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
        if (Instance != null)
        {
            loadingScreenPanel.gameObject.SetActive(true);
        }
    }

    public void Hide()
    {
        if (Instance != null)
        {
            loadingScreenPanel.gameObject.SetActive(false);
        }
    }
}
