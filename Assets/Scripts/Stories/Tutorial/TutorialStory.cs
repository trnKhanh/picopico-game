using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialStory : MonoBehaviour
{
    void Start()
    {
        InputManager.Instance.active = true;
    }

    private void OnEnable()
    {
        DialogManager.onFinished += DialogManager_onFinished;
    }

    private void DialogManager_onFinished(object sender, EventArgs e)
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.active = true;
        }
    }
}
