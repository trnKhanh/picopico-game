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
    [SerializeField] private Transform playersList;
    [SerializeField] private Canvas createRoomCanvas;

    private void OnEnable()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        Debug.Log("Created");

        Lobby lobby = LobbyManager.Instance.GetJoinedLobby();
        if (lobby == null)
            return;
        roomName.text = lobby.Name;

        foreach (Transform child in playersList)
        {
            Destroy(child.gameObject);
        }
        foreach (Player player in lobby.Players)
        {
            GameObject gameObject = Instantiate(playerNamePanelPrefab, playersList);
            TMP_Text text = gameObject.GetComponentInChildren<TMP_Text>();
            text.text = player.Data[LobbyManager.KEY_PLAYER_NAME].Value;
            text.enabled = true;
        }
    }
}
