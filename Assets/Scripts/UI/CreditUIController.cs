using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditUIController : MonoBehaviour
{
    private int m_clickedNum = 0;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            m_clickedNum++;
            if (m_clickedNum >= 2)
                BackToMenu();
        }
    }

    public async void BackToMenu()
    {
        await LobbyManager.Instance.LeaveLobby();
    }
}
