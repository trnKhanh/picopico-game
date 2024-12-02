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
    [SerializeField] private LobbyUI lobbyUI;

    private void OnEnable()
    {
        RefreshUI();

        SubcribeToButtonEvents();
    }

    private void OnDisable()
    {
        UnSubcribeToButtonEvents();
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void RefreshUI()
    {
        roomName.text = "";
        isPrivate.isOn = false;
    }

    // Button Events
    private void SubcribeToButtonEvents()
    {
        UnSubcribeToButtonEvents();

        createRoomButton.onClick.AddListener(CreateRoomButton_onClick);
        cancleRoomButton.onClick.AddListener(CancleRoomButton_onClick);
    }

    private void UnSubcribeToButtonEvents()
    {
        createRoomButton.onClick.RemoveListener(CreateRoomButton_onClick);
        cancleRoomButton.onClick.RemoveListener(CancleRoomButton_onClick);
    }

    private async void CreateRoomButton_onClick()
    {
        await LobbyManager.Instance.CreateLobby(roomName.text, 4, isPrivate.isOn);
        lobbyUI.Show();
        Hide();
    }

    private void CancleRoomButton_onClick()
    {
        Hide();
    }
}
