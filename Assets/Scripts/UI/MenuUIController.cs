using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController: MonoBehaviour
{
    [SerializeField] private Button hostGameButton;
    [SerializeField] private Button quickJoinGameButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button quitGameButton;
    [SerializeField] private Button confirmGameButton;

    [SerializeField] private TMP_InputField playerName;
    [SerializeField] private TMP_InputField joinCode;
    [SerializeField] private LobbyUI lobbyUI;
    [SerializeField] private CreateRoomUI createRoomUI;

    private Animator m_animator;

    private static string k_enteredPlayerName = "EnteredPlayerName";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        lobbyUI.Hide();
        createRoomUI.Hide();
    }

    private void OnEnable()
    {
        RefreshUI();
        SubcribeToButtonEvents();
    }

    private void OnDisable()
    {
        UnSubcribeToButtonEvents();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (confirmGameButton.gameObject.activeInHierarchy)
            {
                EnteredPlayerName();
            }
        }
    }

    private void RefreshUI()
    {
        if (LobbyManager.Instance != null && LobbyManager.Instance.IsSignedIn())
        {
            m_animator.SetTrigger(k_enteredPlayerName);
        }
    }

    // Button Events
    private void SubcribeToButtonEvents()
    {
        UnSubcribeToButtonEvents();

        confirmGameButton.onClick.AddListener(ConfirmButton_onClick);
        hostGameButton.onClick.AddListener(HostGameButton_onClick);
        quickJoinGameButton.onClick.AddListener(QuickJoinButton_onClick);
        joinGameButton.onClick.AddListener(JoinGameButton_onClick);
        quitGameButton.onClick.AddListener(QuitGameButton_onClick);
    }

    private void UnSubcribeToButtonEvents()
    {
        confirmGameButton.onClick.RemoveListener(ConfirmButton_onClick);
        hostGameButton.onClick.RemoveListener(HostGameButton_onClick);
        quickJoinGameButton.onClick.RemoveListener(QuickJoinButton_onClick);
        joinGameButton.onClick.RemoveListener(JoinGameButton_onClick);
        quitGameButton.onClick.RemoveListener(QuitGameButton_onClick);
    }

    private void ConfirmButton_onClick()
    {
        EnteredPlayerName();
    }

    private void HostGameButton_onClick()
    {
        createRoomUI.Show();
    }

    private async void QuickJoinButton_onClick()
    {
        await LobbyManager.Instance.QuickJoinLobby();
    }

    private async void JoinGameButton_onClick()
    {
        if (joinCode.text == null || joinCode.text.Length == 0)
            return;

        await LobbyManager.Instance.JoinLobbyByCode(joinCode.text);
    }

    private void QuitGameButton_onClick()
    {
        Application.Quit();
    }

    // Lobby Manager Events
    private void SubcribeToLobbyManagerEvents()
    {
        UnSubcribeToLobbyManagerEvents();

        LobbyManager.OnCreatedLobby += LobbyManager_OnCreatedLobby;
        LobbyManager.OnJoinedLobby += LobbyManager_OnJoinedLobby;
        LobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;
    }

    private void UnSubcribeToLobbyManagerEvents()
    {
        LobbyManager.OnCreatedLobby -= LobbyManager_OnCreatedLobby;
        LobbyManager.OnJoinedLobby -= LobbyManager_OnJoinedLobby;
        LobbyManager.OnKickedFromLobby -= LobbyManager_OnKickedFromLobby;
    }

    private void LobbyManager_OnCreatedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        lobbyUI.Show();
        createRoomUI.Hide();
    }

    private void LobbyManager_OnJoinedLobby(object sender, EventArgs e)
    {
        lobbyUI.Show();
        createRoomUI.Hide();
    }

    private void LobbyManager_OnKickedFromLobby(object sender, EventArgs e)
    {
        lobbyUI.Hide();
        createRoomUI.Hide();
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
