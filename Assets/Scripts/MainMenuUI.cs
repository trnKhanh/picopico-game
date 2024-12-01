using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button quickJoinGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button confirmGameButton;

    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField joinCode;
    [SerializeField] private Canvas roomCanvas;
    [SerializeField] private Canvas createRoomCanvas;

    private Animator m_animator;

    private static string k_enteredPlayerName = "EnteredPlayerName";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    void Start()
    {
       
    }

    private void OnEnable()
    {
        hostGameButton.onClick.AddListener(() =>
        {
            createRoomCanvas.gameObject.SetActive(true);
        });
        quickJoinGameButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.QuickJoinLobby();
            roomCanvas.gameObject.SetActive(true);
        });
        joinGameButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.JoinLobbyByCode(joinCode.text);
            roomCanvas.gameObject.SetActive(true);
        });
        quitGameButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        if (LobbyManager.Instance.IsSignedIn())
        {
            m_animator.SetTrigger(k_enteredPlayerName);
        }
        else
        {
            confirmGameButton.onClick.AddListener(() =>
            {
                EnteredPlayerName();
            });
        }

        SubribeToLobbyEvents();
    }

    private void OnDisable()
    {
        UnSubribeToLobbyEvents();

        hostGameButton.onClick.RemoveAllListeners();
        quickJoinGameButton.onClick.RemoveAllListeners();
        joinGameButton.onClick.RemoveAllListeners();
        quitGameButton.onClick.RemoveAllListeners();
        confirmGameButton.onClick.RemoveAllListeners();

    }

    private void SubribeToLobbyEvents()
    {
        UnSubribeToLobbyEvents();
        LobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
    }

    private void UnSubribeToLobbyEvents()
    {
        LobbyManager.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
    }

    private void LobbyManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        roomCanvas.enabled = true;
    }

    void EnteredPlayerName()
    {
        if (playerName.text != null && playerName.text.Length > 0)
        {
            LobbyManager.Instance.Authenticate(playerName.text);
            m_animator.SetTrigger(k_enteredPlayerName);
        }
    }
}
