using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUIController : MonoBehaviour
{
    [SerializeField] private EscapeUI escapeUI;

    private void Start()
    {
        escapeUI.Hide();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeUI.Toggle();
        }
    }
}
