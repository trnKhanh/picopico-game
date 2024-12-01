using System;
using System.Collections;
using System.Collections.Generic;
using IngameDebugConsole;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;


public class CommandManager : MonoBehaviour
{
    static public CommandManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this);
    }
    private void Start()
    {
        LobbyManager.OnCreatedLobby += CommandManager_OnCreatedLobby;
        LobbyManager.OnLobbiesListChange += CommandManager_OnLobbiesListChange;
        LobbyManager.OnJoinedLobby += CommandManager_OnJoinedLobby;
        LobbyManager.OnUpdatedLobby += CommandManager_OnUpdatedLobby;
        LobbyManager.OnLeftLobby += CommandManager_OnLeftLobby;
        LobbyManager.OnKickedFromLobby += CommandManager_OnKickedFromLobby;
    }

    private void CommandManager_OnLeftLobby(object sender, EventArgs e)
    {
        Debug.Log("Left lobby");
    }
    private void CommandManager_OnKickedFromLobby(object sender, EventArgs e)
    {
        Debug.Log("Kicked");
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
    public static async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false)
    {
        await LobbyManager.Instance.CreateLobby(lobbyName, maxPlayers, isPrivate);
    }

    [ConsoleMethod("list-lobbies", "List lobbies")]
    public static async void ListLobbies()
    {
        await LobbyManager.Instance.ListLobbies(0, 10);
    }

    [ConsoleMethod("kick-player", "Kick player")]
    public static async void KickPlayer(string playerId)
    {
        await LobbyManager.Instance.KickPlayer(playerId);
    }

    [ConsoleMethod("join-lobby-by-code", "Join lobby by code")]
    public static async void JoinLobbyByCode(string joinCode)
    {
        await LobbyManager.Instance.JoinLobbyByCode(joinCode);
    }

    [ConsoleMethod("quick-join", "Join lobby by code")]
    public static async void QuickJoin()
    {
        await LobbyManager.Instance.QuickJoinLobby();
    }

    [ConsoleMethod("change-lobby-name", "Change lobby name")]
    public static async void ChangeLobbyName(string name)
    {
        await LobbyManager.Instance.ChangeLobbyName(name);
    }

    [ConsoleMethod("change-lobby-max-players", "Change lobby max players")]
    public static async void ChangeLobbyName(int maxPlayers)
    {
        await LobbyManager.Instance.ChangeLobbyMaxPlayers(maxPlayers);
    }

    [ConsoleMethod("leave-lobby", "Leave lobby")]
    public static async void LeaveLobby()
    {
        await LobbyManager.Instance.LeaveLobby();
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
    public static async void StartGame()
    {
        await LobbyManager.Instance.StartGame();
    }
}
