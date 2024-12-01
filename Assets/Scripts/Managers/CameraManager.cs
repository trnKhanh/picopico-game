using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    static public CameraManager Instance { get; private set; }

    [SerializeField] private Camera mainCamera;
    [SerializeField] private float followSpeed = 10.0f;
    [SerializeField] private float yOffset = 3.0f;

    public Transform target;

    private List<PlayerController> players = new List<PlayerController>();
    private Vector2 curOffset = Vector2.zero;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        SubribeToPlayerEvent();
    }

    private void OnDisable()
    {
        UnSubribeToPlayerEvent();
    }

    public void SubribeToPlayerEvent()
    {
        UnSubribeToPlayerEvent();

        PlayerController.onAppeared += PlayerController_onAppeared;
        PlayerController.onDissapeared += PlayerController_onDied;
    }

    private void PlayerController_onAppeared(object sender, EventArgs e)
    {
        PlayerController player = (PlayerController)sender;
        if (player.IsOwner)
        {
            target = player.transform;
        }
        players.Add(player);
    }

    private void PlayerController_onDied(object sender, EventArgs e)
    {
        PlayerController player = (PlayerController)sender;
        players.Remove(player);

        if (players.Count != 0)
        {
            if (player.IsOwner)
            {
                target = players[0].transform;
            }
        }
    }

    private void Update()
    {
        FollowTarget();
    }

    private void FollowTarget()
    {
        if (target == null)
            return;

        Vector3 newPosition = target.position;
        newPosition.x += curOffset.x;
        newPosition.y += yOffset + curOffset.y;
        newPosition.z = mainCamera.transform.position.z;

        mainCamera.transform.position = Vector3.Slerp(mainCamera.transform.position, newPosition, followSpeed * Time.deltaTime);
    }

    public void SetOffset(Vector2 offset)
    {
        curOffset = offset;
    }

    public void UnSubribeToPlayerEvent()
    {
        PlayerController.onAppeared -= PlayerController_onAppeared;
        PlayerController.onDissapeared -= PlayerController_onDied;
    }

    
}
