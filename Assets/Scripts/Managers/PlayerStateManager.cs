using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStateManager : NetworkBehaviour
{
    static public PlayerStateManager Instance { get; private set; }

    [SerializeField] private Color[] playerColours;

    static public event EventHandler onUpdatedPlayerState;

    private List<int> availableColours;

    public Color playerColour { get; private set; } = Color.white;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            availableColours = new List<int>();
            for (int i = 0; i < playerColours.Length; ++i)
                availableColours.Add(i);
        }
        if (IsClient)
        {
            RequestColourServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestColourServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (availableColours.Count > 0)
        {
            int id = availableColours[0];
            availableColours.RemoveAt(0);
            RespondColourClientRpc(serverRpcParams.Receive.SenderClientId, id);
        }
    }
    [ClientRpc]
    private void RespondColourClientRpc(ulong clientId, int colourId)
    {
        if (clientId != NetworkManager.Singleton.LocalClientId)
            return;

        playerColour = playerColours[colourId];
        onUpdatedPlayerState?.Invoke(this, EventArgs.Empty);
    }
}
