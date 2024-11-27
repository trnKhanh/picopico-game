using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneStateManager : MonoBehaviour
{
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

    public void UnSubribeToPlayerEvent(PlayerController player)
    {
        player.onDied -= PlayerController_onDied;
        player.onHit -= PlayerController_onHit;
    }

    public void SubribeToPlayerEvent(PlayerController player)
    {
        UnSubribeToPlayerEvent(player);

        players.Add(player);

        player.onDied += PlayerController_onDied;
        player.onHit += PlayerController_onHit;
    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {
        Debug.Log("die");
        players.Remove((PlayerController) sender);

        if (players.Count == 0)
        {
            SceneLoadingManager.Instance.ReloadScene();
        }
    }

    private void PlayerController_onHit(object sender, EventArgs e)
    {

    }
}
