using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;


public class CommandManager : MonoBehaviour
{
    private void Start()
    {
        LobbyManager.Instance.OnCreatedLobby += CommandManager_OnCreatedLobby;
        LobbyManager.Instance.OnLobbiesListChange += CommandManager_OnLobbiesListChange;
        LobbyManager.Instance.OnJoinedLobby += CommandManager_OnJoinedLobby;
        LobbyManager.Instance.OnUpdatedLobby += CommandManager_OnUpdatedLobby;
        LobbyManager.Instance.OnLeftLobby += CommandManager_OnLeftLobby;
    }

    private void CommandManager_OnLeftLobby(object sender, EventArgs e)
    {
        Debug.Log("Left lobby");
    }

    private void CommandManager_OnUpdatedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = e.lobby;
        Debug.Log($"Update lobby: {lobby.Name}; {lobby.MaxPlayers}; {lobby.IsPrivate}");
        foreach (Player player in lobby.Players)
        {
            Debug.Log($"\t {player.Id} {player.Data[LobbyManager.KEY_PLAYER_NAME].Value}");
        }
    }

    private void CommandManager_OnLobbiesListChange(object sender, LobbyManager.LobbyListEventArgs e)
    {
        Debug.Log($"Lobbies found: {e.lobbies.Count}");
        foreach (Lobby lobby in e.lobbies)
        {
            Debug.Log($"\t {lobby.Name}; {lobby.MaxPlayers};");
        }
    }

    private void CommandManager_OnCreatedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = e.lobby;
        Debug.Log($"Created lobby: {lobby.Name}; {lobby.MaxPlayers}; {lobby.IsPrivate}");
        Debug.Log($"Join code: {lobby.LobbyCode}");
    }

    private void CommandManager_OnJoinedLobby(object sender, LobbyManager.LobbyEventArgs e)
    {
        Lobby lobby = e.lobby;
        Debug.Log($"Joined lobby: {lobby.Name}; {lobby.MaxPlayers}; {lobby.IsPrivate}");
    }

    [ConsoleMethod("authenticate", "authenticate")]
    public static void CreateLobby(string playerName)
    {
        LobbyManager.Instance.Authenticate(playerName);

    }

    [ConsoleMethod("create-lobby", "Create lobby")]
    public static void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false)
    {
        LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);
    }
    [ConsoleMethod("list-lobbies", "List lobbies")]
    public static void ListLobbies()
    {
        LobbyManager.Instance.ListLobbies(0, 10);
    }

    [ConsoleMethod("join-lobby-by-code", "Join lobby by code")]
    public static void JoinLobbyByCode(string joinCode)
    {
        LobbyManager.Instance.JoinLobbyByCode(joinCode);
    }

    [ConsoleMethod("quick-join", "Join lobby by code")]
    public static void QuickJoin()
    {
        LobbyManager.Instance.QuickJoinLobby();
    }

    [ConsoleMethod("change-lobby-name", "Change lobby name")]
    public static void ChangeLobbyName(string name)
    {
        LobbyManager.Instance.ChangeLobbyName(name);
    }

    [ConsoleMethod("change-lobby-max-players", "Change lobby max players")]
    public static void ChangeLobbyName(int maxPlayers)
    {
        LobbyManager.Instance.ChangeLobbyMaxPlayers(maxPlayers);
    }

    [ConsoleMethod("leave-lobby", "Leave lobby")]
    public static void LeaveLobby()
    {
        LobbyManager.Instance.LeaveLobby();
    }

    [ConsoleMethod("start-host", "Start host")]
    public static void StartHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    [ConsoleMethod("start-server", "Start server")]
    public static void StartServer()
    {
        NetworkManager.Singleton.StartServer();
    }

    [ConsoleMethod("start-client", "Start client")]
    public static void StartClient()
    {
        NetworkManager.Singleton.StartClient();
    }

    [ConsoleMethod("start-game", "Start game")]
    public static void StartGame()
    {
        LobbyManager.Instance.StartGame();
    }
}
