using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    public float verticalInput { get; private set; }
    public float horizontalInput { get; private set; }
    public bool jump { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance.gameObject);
        }
        Instance = this;
    }

    private void Update()
    {
        verticalInput = Input.GetAxis("Vertical") * Time.deltaTime;
        horizontalInput = Input.GetAxis("Horizontal") * Time.deltaTime;

        jump = (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space));
    }
}
