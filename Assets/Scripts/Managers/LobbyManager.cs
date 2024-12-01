using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private float hostHeartBeatTimeoutMax = 15.0f;
    [SerializeField] private float lobbyPoolingTimeoutMax = 1.1f;

    public static LobbyManager Instance { get; private set; }

    public static string KEY_PLAYER_NAME = "PlayerName";
    public static string KEY_RELAY_CODE = "RelayCode";
    public static string KEY_CLIENT_ID = "ClientId";

    public class LobbyEventArgs: EventArgs
    {
        public Lobby lobby;
    };

    public class LobbyListEventArgs : EventArgs
    {
        public List<Lobby> lobbies;
    }
    public event EventHandler<LobbyEventArgs> OnCreatedLobby;
    public event EventHandler<LobbyEventArgs> OnJoinedLobby;
    public event EventHandler<LobbyEventArgs> OnUpdatedLobby;
    public event EventHandler OnLeftLobby;
    public event EventHandler OnKickedFromLobby;


    public event EventHandler<LobbyListEventArgs> OnLobbiesListChange;


    private string m_playerName;
    private Lobby m_joinedLobby;
    private ILobbyEvents m_lobbyEvents;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    private void Start()
    {
        Application.wantsToQuit += Application_wantsToQuit;

        SubcribeToNetworkEvents();
        StartCoroutine(SendHeartBeat());
    }

    private void SubcribeToNetworkEvents()
    {
        NetworkManager.Singleton.OnClientConnectedCallback -= NetworkManager_OnClientConnectedCallback;
        NetworkManager.Singleton.OnClientConnectedCallback += NetworkManager_OnClientConnectedCallback;
    }

    private async void NetworkManager_OnClientConnectedCallback(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId)
        {
            return;
        }

        if (m_joinedLobby == null)
            return;

        UpdatePlayerOptions updatePlayerOptions = new UpdatePlayerOptions
        {
            Data = CreatePlayerData(),
        };

        await LobbyService.Instance.UpdatePlayerAsync(m_joinedLobby.Id, AuthenticationService.Instance.PlayerId, updatePlayerOptions);
    }

    private bool Application_wantsToQuit()
    {
        bool canQuit = !IsPlayerInLobby(m_joinedLobby);
        if (!canQuit)
        {
            StartCoroutine(LeaveBeforeQuit());
        }

        return canQuit;
    }

    private IEnumerator LeaveBeforeQuit()
    {
        Task task = LeaveLobby();
        yield return new WaitUntil(() => task.IsCompleted);
        Application.Quit();
    }

    private IEnumerator SendHeartBeat()
    {
        while (true)
        {
            yield return new WaitForSeconds(hostHeartBeatTimeoutMax);
            if (m_joinedLobby == null)
                continue;

            try
            {
                LobbyService.Instance.SendHeartbeatPingAsync(m_joinedLobby.Id);
                Debug.Log("Heartbeat");
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private async void SubribeToLobbyEvents()
    {
        if (m_joinedLobby == null)
            return;
        try
        {
            LobbyEventCallbacks callbacks = new LobbyEventCallbacks();
            callbacks.KickedFromLobby += LobbyEvents_KickedFromLobby;
            callbacks.LobbyChanged += LobbyEvents_LobbyChanged;

            m_lobbyEvents = await LobbyService.Instance.SubscribeToLobbyEventsAsync(m_joinedLobby.Id, callbacks);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    private void LobbyEvents_KickedFromLobby()
    {
        OnKickedFromLobby?.Invoke(this, EventArgs.Empty);
        m_joinedLobby = null;
    }

    private void LobbyEvents_LobbyChanged(ILobbyChanges changes)
    {
        changes.ApplyToLobby(m_joinedLobby);
        if (m_joinedLobby.Data != null && m_joinedLobby.Data.ContainsKey(KEY_RELAY_CODE))
        {
            JoinGame(m_joinedLobby);
        }

        OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = m_joinedLobby});
    }

    private Player GetPlayerData(Lobby lobby, string playerId)
    {
        if (lobby != null && lobby.Players != null && playerId != null)
        {
            foreach (Player player in lobby.Players)
            {
                if (player.Id == playerId)
                    return player;
            }
        }
        return null;
    }

    private bool IsPlayerInLobby(Lobby lobby)
    {
        if (lobby != null && lobby.Players != null)
        {
            foreach (Player player in lobby.Players)
            {
                if (player.Id == AuthenticationService.Instance.PlayerId)
                    return true;
            }
        }
        return false;
    }

    private bool IsPlayerHost(Lobby lobby, string playerId)
    {
        return lobby != null && playerId != null && lobby.HostId == playerId;
    }

    private async void JoinGame(Lobby lobby)
    {
        if (lobby == null || lobby.Data == null || NetworkManager.Singleton.IsClient)
            return;

        if (lobby.Data.ContainsKey(KEY_RELAY_CODE))
        {
            try
            {
                // Game was started
                string relayCode = lobby.Data[KEY_RELAY_CODE].Value;
                Debug.Log($"Relay code {relayCode}");

                JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(relayCode);

                RelayServerData relayServerData = new RelayServerData(joinAllocation, "dtls");

                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartClient();                
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Authenticate a player with corresponding playerName
    public async void Authenticate(string playerName)
    {
        m_playerName = playerName;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(m_playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Player {playerName} signed in with id {playerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Create lobby
    public async Task CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false, string password = null, bool isLocked = false)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Password = password,
                IsLocked = isLocked,
                Player = new Player { Data = CreatePlayerData() },
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            m_joinedLobby = lobby;

            SubribeToLobbyEvents();
            OnCreatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby});
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // List lobbies
    public async Task ListLobbies(int skip, int count)
    {
        try
        {
            QueryLobbiesOptions options = new QueryLobbiesOptions
            {
                Count = count,
                Skip = skip,
                Filters = new List<QueryFilter>
                {
                    new QueryFilter(QueryFilter.FieldOptions.AvailableSlots, "0", QueryFilter.OpOptions.GT),

                },
                Order = new List<QueryOrder>
                {
                    new QueryOrder(false, QueryOrder.FieldOptions.Created),
                }
            };

            QueryResponse response = await LobbyService.Instance.QueryLobbiesAsync(options);

            OnLobbiesListChange?.Invoke(this, new LobbyListEventArgs { lobbies = response.Results });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Join lobby by code
    public async Task JoinLobbyByCode(string joinCode, string password = null)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Password = password,
                Player = new Player { Data = CreatePlayerData() },
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, options);
            m_joinedLobby = lobby;

            SubribeToLobbyEvents();
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Join lobby by id
    public async Task JoinLobbyById(string id, string password = null)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Password = password,
                Player = new Player { Data = CreatePlayerData() },
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, options);
            m_joinedLobby = lobby;

            SubribeToLobbyEvents();
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Quick join lobby
    public async Task QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = new Player { Data = CreatePlayerData() }
            };

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            m_joinedLobby = lobby;

            SubribeToLobbyEvents();
            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Change lobby name
    public async Task ChangeLobbyName(string name)
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Name = name,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(m_joinedLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change max players
    public async Task ChangeLobbyMaxPlayers(int maxPlayers)
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    MaxPlayers = maxPlayers,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(m_joinedLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change lobby password
    public async Task ChangeLobbyPassword(string password)
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Password = password,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(m_joinedLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change lobby private status
    public async Task ChangeLobbyIsPrivate(bool isPrivate)
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    IsPrivate = isPrivate,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(m_joinedLobby.Id, options);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Leave lobby
    public async Task LeaveLobby()
    {
        if (m_joinedLobby != null)
        {
            try
            {
                string lobbyId = m_joinedLobby.Id;
                if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
                {
                    await LobbyService.Instance.DeleteLobbyAsync(lobbyId);
                }
                else
                {
                    await LobbyService.Instance.RemovePlayerAsync(lobbyId, AuthenticationService.Instance.PlayerId);
                }

                m_joinedLobby = null;
                NetworkManager.Singleton.Shutdown();
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Kick player
    public async Task KickPlayer(string playerId)
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                Player kickedPlayer = GetPlayerData(m_joinedLobby, playerId);
                Debug.Log($"kickedPlayer: {kickedPlayer.Data[KEY_CLIENT_ID].Value}");

                if (kickedPlayer.Data != null && kickedPlayer.Data.ContainsKey(KEY_CLIENT_ID))
                {
                    NetworkManager.Singleton.DisconnectClient((ulong)Decimal.Parse(kickedPlayer.Data[KEY_CLIENT_ID].Value));
                }

                await LobbyService.Instance.RemovePlayerAsync(m_joinedLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    public async Task StartGame()
    {
        if (IsPlayerHost(m_joinedLobby, AuthenticationService.Instance.PlayerId))
        {
            try
            {
                Allocation allocation = await RelayService.Instance.CreateAllocationAsync(m_joinedLobby.MaxPlayers - 1);
                string relayCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
                Debug.Log($"Server Relay code {relayCode}");
                RelayServerData relayServerData = new RelayServerData(allocation, "dtls");
                NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(relayServerData);
                NetworkManager.Singleton.StartHost();

                UpdateLobbyOptions updateLobbyOptions = new UpdateLobbyOptions
                {
                    IsPrivate = true,
                    Data = new Dictionary<string, DataObject>
                    {
                        { KEY_RELAY_CODE, new DataObject(DataObject.VisibilityOptions.Member, relayCode)}
                    }
                };
                await LobbyService.Instance.UpdateLobbyAsync(m_joinedLobby.Id, updateLobbyOptions);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
            catch (RelayServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private Dictionary<string, PlayerDataObject> CreatePlayerData()
    {
        Dictionary<string, PlayerDataObject> data = new Dictionary<string, PlayerDataObject>();
        data.Add(KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, m_playerName));

        if (NetworkManager.Singleton.IsClient)
        {
            data.Add(KEY_CLIENT_ID, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member, NetworkManager.Singleton.LocalClientId.ToString()));
        }

        return data;
    }
}
