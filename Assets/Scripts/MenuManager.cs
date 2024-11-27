using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExtiGame()
    {
        Application.Quit();
        Debug.Log("QUIT GAME");
    }

    public void Setting()
    {
        Debug.Log("Open Settings");
    }
}
