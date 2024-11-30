using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAchievement : BaseAchievement
{
    [SerializeField] private int requiredJumpsNumber = 1;

    public override void UpdateState(object sender, EventArgs e)
    {
        Debug.Log("JumpAchievement:UpdateState");

        if (GameDataManager.Instance.gameData.jumpNums >= requiredJumpsNumber)
        {
            Achieve();
        }
    }
}
