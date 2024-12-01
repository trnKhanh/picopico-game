using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieAchievement : BaseAchievement
{
    [SerializeField] private int requiredDeathNumber = 1;

    public override void UpdateState(object sender, EventArgs e)
    {
        if (GameDataManager.Instance.gameData.deathNums >= requiredDeathNumber)
        {
            Achieve();
        }
    }
}
