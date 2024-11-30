using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameData
{
    public int jumpNums = 0;

}

public class GameDataManager : MonoBehaviour
{
    static public GameDataManager Instance { get; private set; }

    static public event EventHandler onUpdatedData;

    public GameData gameData { get; private set; } = new GameData();

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

    private void OnEnable()
    {
        SubcribeToPlayerControllerEvents();
    }

    private void OnDisable()
    {
        
    }

    private void SubcribeToPlayerControllerEvents()
    {
        UnSubcribeToPlayerControllerEvents();
        PlayerController.onAppeared += PlayerController_onAppeared;
        PlayerController.onDissapeared += PlayerController_onDisappeared;
    }

    private void UnSubcribeToPlayerControllerEvents()
    {
        PlayerController.onAppeared -= PlayerController_onAppeared;
        PlayerController.onDissapeared -= PlayerController_onDisappeared;
    }

    private void PlayerController_onAppeared(object sender, EventArgs e)
    {
        PlayerController player = (PlayerController)sender;

        if (player.IsOwner)
        {
            SubcribeToOwnerPlayerEvents(player);
        }
    }

    private void PlayerController_onDisappeared(object sender, EventArgs e)
    {
        PlayerController player = (PlayerController)sender;

        if (player.IsOwner)
        {
            UnSubcribeToOwnerPlayerEvents(player);
        }
    }

    private void SubcribeToOwnerPlayerEvents(PlayerController player)
    {
        UnSubcribeToOwnerPlayerEvents(player);
        player.playerMovement.onJumped += PlayerMovement_onJumped;
    }

    private void UnSubcribeToOwnerPlayerEvents(PlayerController player)
    {
        player.playerMovement.onJumped -= PlayerMovement_onJumped;
    }

    private void PlayerMovement_onJumped(object sender, EventArgs e)
    {
        gameData.jumpNums += 1;
        onUpdatedData?.Invoke(this, EventArgs.Empty);
    }
}
