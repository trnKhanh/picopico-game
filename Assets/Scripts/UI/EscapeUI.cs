using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    private void OnEnable()
    {
        SubcribeToButtonEvents();
    }

    private void OnDisable()
    {
        UnSubcribeToButtonEvents();
    }

    // Button Events
    private void SubcribeToButtonEvents()
    {
        UnSubcribeToButtonEvents();
        continueButton.onClick.AddListener(ContinueButton_onClick);

        if (LobbyManager.Instance.IsHost())
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(RestartButton_onClick);
        }
        else
        {
            restartButton.gameObject.SetActive(false);
        }

        quitButton.onClick.AddListener(QuitButton_onClick);
    }

    private void UnSubcribeToButtonEvents()
    {
        continueButton.onClick.RemoveListener(ContinueButton_onClick);
        restartButton.onClick.RemoveListener(RestartButton_onClick);
        quitButton.onClick.RemoveListener(QuitButton_onClick);
    }

    private void ContinueButton_onClick()
    {
        Hide();
    }

    private void RestartButton_onClick()
    {
        SceneLoadingManager.Instance.ReloadScene(true);
    }

    private async void QuitButton_onClick()
    {
        LoadingScreenUI.Instance.Show();
        await LobbyManager.Instance.LeaveLobby();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Toggle()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
