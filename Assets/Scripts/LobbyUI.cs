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
    [SerializeField] private Transform playersList;
    [SerializeField] private Canvas createRoomCanvas;

    private void OnEnable()
    {
        leaveRoomButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.LeaveLobby();
            gameObject.SetActive(false);
        });

        UpdateUI();

        Debug.Log("LobbyUI: Subcribe");
        LobbyManager.OnUpdatedLobby -= LobbyManager_OnUpdatedLobby;
        LobbyManager.OnUpdatedLobby += LobbyManager_OnUpdatedLobby;
        LobbyManager.OnKickedFromLobby += LobbyManager_OnKickedFromLobby;

    }

    private void LobbyManager_OnKickedFromLobby(object sender, EventArgs e)
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        LobbyManager.OnUpdatedLobby -= LobbyManager_OnUpdatedLobby;
    }

    private void LobbyManager_OnUpdatedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("LobbyUI:UpdateUI");
        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        if (lobby == null)
        {
            gameObject.SetActive(false);
            return;
        }

        if (LobbyManager.Instance.IsHost())
        {
            startGameButton.onClick.RemoveAllListeners();
            startGameButton.onClick.AddListener(async () =>
            {
                await LobbyManager.Instance.StartGame();
            });
        }
        else
        {
            startGameButton.gameObject.SetActive(false);
        }

        Debug.Log(lobby.Players.Count);
        roomName.text = lobby.Name;
        joinCode.text = lobby.LobbyCode;
        Debug.Log(lobby.Players.Count);

        GameObject[] children = new GameObject[playersList.childCount];
        int i = 0;
        foreach (Transform child in playersList)
        {
            children[i++] = child.gameObject;
        }
        foreach (GameObject gameObject in children)
        {
            Destroy(gameObject);
        }

        foreach (Player player in lobby.Players)
        {
            Debug.Log(playerNamePanelPrefab);
            Debug.Log(playersList);
            GameObject gameObject = Instantiate(playerNamePanelPrefab, playersList);
            TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();
            Image image = gameObject.GetComponentInChildren<Image>();
            text.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
            text.enabled = true;
            image.enabled = true;
        }
    }
}
