using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField] private Button createRoomButton;
    [SerializeField] private Button cancleRoomButton;

    [SerializeField] private TMP_InputField roomName;
    [SerializeField] private Toggle isPrivate;
    [SerializeField] private Canvas roomCanvas;

    private void Start()
    {
        createRoomButton.onClick.AddListener(async () =>
        {
            await LobbyManager.Instance.CreateLobby(roomName.text, 4, isPrivate.isOn);
            roomCanvas.gameObject.SetActive(true);
            gameObject.SetActive(false);
        });
        cancleRoomButton.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });
    }
}
