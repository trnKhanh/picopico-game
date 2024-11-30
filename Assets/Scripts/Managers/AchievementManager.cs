using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    static public AchievementManager Instance { get; private set; }

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
        SubribeToPlayerEvent();
    }

    private void OnDisable()
    {
        UnSubribeToPlayerEvent();
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

    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {

    }
}


abstract class BaseAchievement
{
    public abstract bool GetIcon();
    public abstract string GetText();
    public abstract bool UpdateState(AchievementManager achievementManager);
}

class JumpAchievement : BaseAchievement
{
    public override bool GetIcon()
    {
        throw new NotImplementedException();
    }

    public override string GetText()
    {
        return "Step in new the new world";
    }

    public override bool UpdateState(AchievementManager achievementManager)
    {
        throw new NotImplementedException();
    }
}