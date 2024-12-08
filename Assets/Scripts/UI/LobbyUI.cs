using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private GameObject playerNamePanelPrefab;
    [SerializeField] private Button startGameButton;
    [SerializeField] private Button leaveRoomButton;

    [SerializeField] private TMP_Text roomName;
    [SerializeField] private TMP_Text joinCode;
    [SerializeField] private TMP_Text playerNums;
    [SerializeField] private Transform playersList;

    private void OnEnable()
    {
        RefreshUI();

        SubribeToButtonEvents();
        SubcribeToLobbyManagerEvents();
    }

    private void OnDisable()
    {
        UnSubribeToButtonEvents();
        UnSubcribeToLobbyManagerEvents();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        if (lobby == null)
        {
            Hide();
            return;
        }

        startGameButton.gameObject.SetActive(LobbyManager.Instance.IsHost());

        roomName.text = lobby.Name;
        joinCode.text = lobby.LobbyCode;
        List<Player> players = lobby.Players;
        int playerCount = (players != null ? players.Count : 0);
        playerNums.text = $"Players ({playerCount}/{lobby.MaxPlayers})";
        if (playerCount >= 2 && LobbyManager.Instance.IsHost())
        {
            startGameButton.gameObject.SetActive(true);
            SubribeToButtonEvents();
        } else
        {
            startGameButton.gameObject.SetActive(false);
            UnSubribeToButtonEvents();
        }

        DestroyAllChildren(playersList);

        foreach (Player player in lobby.Players)
        {
            GameObject gameObject = Instantiate(playerNamePanelPrefab, playersList);
            TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();
            Image image = gameObject.GetComponentInChildren<Image>();
            text.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
            text.enabled = true;
            image.enabled = true;
        }
    }

    private void DestroyAllChildren(Transform parent)
    {
        GameObject[] children = new GameObject[parent.childCount];
        int i = 0;
        foreach (Transform child in parent)
        {
            children[i++] = child.gameObject;
        }

        foreach (GameObject gameObject in children)
        {
            Destroy(gameObject);
        }
    }

    //Button Events
    private void SubribeToButtonEvents()
    {
        UnSubribeToButtonEvents();
        startGameButton.onClick.AddListener(StartGameButton_onClick);
        leaveRoomButton.onClick.AddListener(LeaveRoomButton_onClick);
    }

    private void UnSubribeToButtonEvents()
    {
        startGameButton.onClick.RemoveListener(StartGameButton_onClick);
        leaveRoomButton.onClick.RemoveListener(LeaveRoomButton_onClick);
    }

    private async void StartGameButton_onClick()
    {
        List <Player> players = LobbyManager.Instance.GetJoinedLobby().Players;
        if (players == null || players.Count < 2)
            return;
        await LobbyManager.Instance.StartGame();
    }

    private async void LeaveRoomButton_onClick()
    {
        await LobbyManager.Instance.LeaveLobby();
        gameObject.SetActive(false);
    }

    // Lobby Manager Events
    private void SubcribeToLobbyManagerEvents()
    {
        UnSubcribeToLobbyManagerEvents();

        LobbyManager.OnUpdatedLobby += LobbyManager_OnUpdatedLobby;
    }

    private void UnSubcribeToLobbyManagerEvents()
    {
        LobbyManager.OnUpdatedLobby -= LobbyManager_OnUpdatedLobby;
    }

    private void LobbyManager_OnUpdatedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        RefreshUI();
    }
}
