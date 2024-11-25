using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSManager : MonoBehaviour
{
    [SerializeField] private bool vSync = false;
    [SerializeField] private int targetFrameRate = 60;

    private void Start()
    {
        QualitySettings.vSyncCount = vSync ? 1 : 0;
        Application.targetFrameRate = targetFrameRate;
    }
}
