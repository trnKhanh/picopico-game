using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform startPosition;

    private void Start()
    {
        GameObject player = Instantiate(playerPrefab);
        player.transform.position = startPosition.position;
    }
}
