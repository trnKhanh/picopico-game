using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static SceneLoadingManager;

public class SceneStateManager : NetworkBehaviour
{
    [SerializeField] private SceneType nextScene;

    public static SceneStateManager Instance { get; private set; }

    private List<PlayerController> players = new List<PlayerController>();

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
        players.Add((PlayerController) sender);
    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {
        players.Remove((PlayerController) sender);

        if (players.Count == 0)
        {
            SceneLoadingManager.Instance.ReloadScene(true);
        }
    }

    public void End()
    {
        Debug.Log("END");
        if (IsServer)
        {
            SceneLoadingManager.Instance.LoadScene(nextScene, true);
        }
    }
}
