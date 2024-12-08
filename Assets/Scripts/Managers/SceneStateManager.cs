using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static SceneLoadingManager;

public class SceneStateManager : NetworkBehaviour
{
    static public SceneStateManager Instance { get; private set; }

    [SerializeField] private SceneType nextScene;

    private List<PlayerController> players = new List<PlayerController>();
    private bool m_ended = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            SubribeToPlayerEvent();
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer)
        {
            UnSubribeToPlayerEvent();
        }
    }

    public void UnSubribeToPlayerEvent()
    {
        PlayerController.onAppeared -= PlayerController_onAppeared;
        PlayerController.onDissapeared -= PlayerController_onDied;
    }

    public void SubribeToPlayerEvent()
    {
        UnSubribeToPlayerEvent();

        PlayerController.onAppeared += PlayerController_onAppeared;
        PlayerController.onDissapeared += PlayerController_onDied;
    }

    private void PlayerController_onAppeared(object sender, EventArgs e)
    {
        PlayerController player = (PlayerController)sender;
        if (player.IsOwner)
        {
            CameraManager.Instance.target = player.transform;
        }
        players.Add(player);
    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {
        if (m_ended)
            return;

        PlayerController player = (PlayerController)sender;
        players.Remove(player);

        if (players.Count == 0)
        {
            Restart();
        } else
        {
            if (player.IsOwner)
            {
                CameraManager.Instance.target = players[0].transform;
            }
        }
    }

    public void Restart()
    {
        if (m_ended)
            return;

        if (IsServer)
        {
            m_ended = true;
            SceneLoadingManager.Instance.ReloadScene(true);
        }
    }
    public void End()
    {
        EndServerRpc();
    }

    private void EndImpl()
    {
        if (m_ended)
            return;

        if (IsServer)
        {
            m_ended = true;
            SceneLoadingManager.Instance.LoadScene(nextScene, true, true);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void EndServerRpc()
    {
        EndImpl();
    }
}
