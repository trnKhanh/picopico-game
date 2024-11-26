using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private float hostHeartBeatTimeoutMax = 15.0f;
    [SerializeField] private float lobbyPoolingTimeoutMax = 1.1f;

    public static LobbyManager Instance { get; private set; }

    public static string KEY_PLAYER_NAME = "PlayerName";
    public static string KEY_RELAY_CODE = "RelayCode";

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


    private string _playerName;
    private Lobby _hostLobby;
    private Lobby _joinedLobby;
    private float _hostHeartBeatTimeout;
    private float _lobbyPoolingTimeout;


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

    private void Update()
    {
        HandleHostHeartBeat();
        HandleLobbyPooling();
    }

    private async void HandleHostHeartBeat()
    {
        if (_hostLobby == null)
            return;
        
            _hostHeartBeatTimeout -= Time.deltaTime;
            if (_hostHeartBeatTimeout < 0.0f)
            {
                _hostHeartBeatTimeout = hostHeartBeatTimeoutMax;
                try
                {
                    await LobbyService.Instance.SendHeartbeatPingAsync(_hostLobby.Id);
                }
                catch (LobbyServiceException e)
                {
                    Debug.Log(e);
                }
            }
        
    }

    private async void HandleLobbyPooling()
    {
        if (_joinedLobby == null)
            return;

        _lobbyPoolingTimeout -= Time.deltaTime;
        if (_lobbyPoolingTimeout < 0.0f)
        {
            _lobbyPoolingTimeout = lobbyPoolingTimeoutMax;

            try
            {
                Lobby lobby = await LobbyService.Instance.GetLobbyAsync(_joinedLobby.Id);

                OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });

                if (!IsPlayerInLobby(lobby))
                {
                    OnKickedFromLobby?.Invoke(this, EventArgs.Empty);
                    _joinedLobby = null;
                }
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }

        }
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

    // Authenticate a player with corresponding playerName
    public async void Authenticate(string playerName)
    {
        _playerName = playerName;

        InitializationOptions initializationOptions = new InitializationOptions();
        initializationOptions.SetProfile(_playerName);

        await UnityServices.InitializeAsync(initializationOptions);

        AuthenticationService.Instance.SignedIn += () =>
        {
            string playerId = AuthenticationService.Instance.PlayerId;
            Debug.Log($"Player {playerName} signed in with id {playerId}");
        };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    // Create lobby
    public async void CreateLobby(string lobbyName, int maxPlayers, bool isPrivate = false, string password = null, bool isLocked = false)
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions
            {
                IsPrivate = isPrivate,
                Password = password,
                IsLocked = isLocked,
                Player = GetPlayer(),
            };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(lobbyName, maxPlayers, options);
            _hostLobby = lobby;
            _joinedLobby = lobby;

            OnCreatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby});
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // List lobbies
    public async void ListLobbies(int skip, int count)
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
    public async void JoinLobbyByCode(string joinCode, string password = null)
    {
        try
        {
            JoinLobbyByCodeOptions options = new JoinLobbyByCodeOptions
            {
                Password = password,
                Player = GetPlayer(),
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(joinCode, options);
            _joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Join lobby by id
    public async void JoinLobbyById(string id, string password = null)
    {
        try
        {
            JoinLobbyByIdOptions options = new JoinLobbyByIdOptions
            {
                Password = password,
                Player = GetPlayer(),
            };

            Lobby lobby = await LobbyService.Instance.JoinLobbyByIdAsync(id, options);
            _joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Quick join lobby
    public async void QuickJoinLobby()
    {
        try
        {
            QuickJoinLobbyOptions options = new QuickJoinLobbyOptions
            {
                Player = GetPlayer(),
            };

            Lobby lobby = await LobbyService.Instance.QuickJoinLobbyAsync(options);
            _joinedLobby = lobby;

            OnJoinedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
        }
    }

    // Change lobby name
    public async void ChangeLobbyName(string name)
    {
        if (_hostLobby != null)
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Name = name,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, options);
                _hostLobby = lobby;

                OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change max players
    public async void ChangeLobbyMaxPlayers(int maxPlayers)
    {
        if (_hostLobby != null)
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    MaxPlayers = maxPlayers,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, options);
                _hostLobby = lobby;

                OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change lobby password
    public async void ChangeLobbyPassword(string password)
    {
        if (_hostLobby != null)
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    Password = password,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, options);
                _hostLobby = lobby;

                OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Change lobby private status
    public async void ChangeLobbyIsPrivate(bool isPrivate)
    {
        if (_hostLobby != null)
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions
                {
                    IsPrivate = isPrivate,
                };

                Lobby lobby = await LobbyService.Instance.UpdateLobbyAsync(_hostLobby.Id, options);
                _hostLobby = lobby;

                OnUpdatedLobby?.Invoke(this, new LobbyEventArgs { lobby = lobby });
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Leave lobby
    public async void LeaveLobby()
    {
        if (_joinedLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_joinedLobby.Id, AuthenticationService.Instance.PlayerId);

                _hostLobby = null;
                _joinedLobby = null;

                OnLeftLobby?.Invoke(this, EventArgs.Empty);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    // Kick player
    public async void KickPlayer(string playerId)
    {
        if (_hostLobby != null)
        {
            try
            {
                await LobbyService.Instance.RemovePlayerAsync(_hostLobby.Id, playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.Log(e);
            }
        }
    }

    private Player GetPlayer()
    {
        return new Player
        {
            Data = new Dictionary<string, PlayerDataObject>
            {
                { KEY_PLAYER_NAME, new PlayerDataObject(PlayerDataObject.VisibilityOptions.Public, _playerName)}
            }
        };
    }
}
