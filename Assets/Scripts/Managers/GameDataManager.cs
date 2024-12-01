using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GameData
{
    public int jumpNums = 0;
    public int deathNums = 0;
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

    private void Start()
    {
        LoadGameData();

        Application.wantsToQuit += Application_wantsToQuit;
    }

    private bool Application_wantsToQuit()
    {
        SaveGameData();
        return true;
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
        player.onDied += PlayerController_onDied;
    }

    private void UnSubcribeToOwnerPlayerEvents(PlayerController player)
    {
        player.playerMovement.onJumped -= PlayerMovement_onJumped;
        player.onDied -= PlayerController_onDied;
    }

    private void PlayerMovement_onJumped(object sender, EventArgs e)
    {
        gameData.jumpNums += 1;
        onUpdatedData?.Invoke(this, EventArgs.Empty);
    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {
        gameData.deathNums += 1;
        onUpdatedData?.Invoke(this, EventArgs.Empty);
    }

    private void SaveGameData()
    {
        string path = Path.Join(Application.persistentDataPath, "GameData.json");

        string json = JsonUtility.ToJson(gameData);
        File.WriteAllText(path, json);
    }

    private void LoadGameData()
    {
        string path = Path.Join(Application.persistentDataPath, "GameData.json");
        if (!File.Exists(path))
            return;

        string json = File.ReadAllText(path);
        gameData = JsonUtility.FromJson<GameData>(json);
        onUpdatedData?.Invoke(this, EventArgs.Empty);
    }
}
