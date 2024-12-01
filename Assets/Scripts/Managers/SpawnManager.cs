using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour
{
    static public SpawnManager Instance { get; private set; }

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform startPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void SpawnPlayer(ulong clientId)
    {
        GameObject playerObject = Instantiate(playerPrefab, startPosition.position, Quaternion.identity);
        playerObject.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
    }
}
