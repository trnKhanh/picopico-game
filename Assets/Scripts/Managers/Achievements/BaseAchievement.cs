using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAchievement : MonoBehaviour
{
    public event EventHandler onAchieved;
    public Sprite icon = null;
    public string text = null;
    public string description = null;
    public abstract void UpdateState(object sender, EventArgs e);

    public bool isAchieved = false;


    private void OnEnable()
    {
        GameDataManager.onUpdatedData += UpdateState;
    }

    private void OnDisable()
    {
        GameDataManager.onUpdatedData -= UpdateState;
        GameDataManager.onUpdatedData += UpdateState;
    }

    protected void Achieve()
    {
        if (!isAchieved)
        {
            isAchieved = true;
            onAchieved?.Invoke(this, EventArgs.Empty);
        }
    }
}
