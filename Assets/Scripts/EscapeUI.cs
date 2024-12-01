using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EscapeUI : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button quitButton;

    [SerializeField] private Transform escapePanel;

    private bool m_isActive = false;

    private void Start()
    {
        escapePanel.gameObject.SetActive(m_isActive);
    }

    private void OnEnable()
    {
        continueButton.onClick.AddListener(() =>
        {
            escapePanel.gameObject.SetActive(false);
        });

        if (LobbyManager.Instance.IsHost())
        {
            restartButton.gameObject.SetActive(true);
            restartButton.onClick.AddListener(() =>
            {
                SceneLoadingManager.Instance.ReloadScene(true);
            });
        } else
        {
            restartButton.gameObject.SetActive(false);
        }

        quitButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.LeaveLobby();
        });
    }

    private void OnDisable()
    {
        continueButton.onClick.RemoveAllListeners();
        restartButton.onClick.RemoveAllListeners();
        quitButton.onClick.RemoveAllListeners();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapePanel.gameObject.SetActive(!m_isActive);
            m_isActive = !m_isActive;
        } 
    }
}
