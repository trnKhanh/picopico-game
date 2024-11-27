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

    private void OnEnable()
    {
        SubribeToPlayerEvent();
    }

    private void OnDisable()
    {
        UnSubribeToPlayerEvent();
    }

    public void UnSubribeToPlayerEvent()
    {
        PlayerController.onAppeared -= PlayerController_onAppeared;
        PlayerController.onDied -= PlayerController_onDied;
    }

    public void SubribeToPlayerEvent()
    {
        PlayerController.onAppeared += PlayerController_onAppeared;
        PlayerController.onDied += PlayerController_onDied;
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
            SceneLoadingManager.Instance.ReloadScene();
        }
    }
}
