using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyController : NetworkBehaviour
{
    private EnemyBehaviour m_enemyBehaviour;
    private Animator m_animator;

    private NetworkVariable<Vector3> nv_position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> nv_localScale = new NetworkVariable<Vector3>();

    private bool m_isConnected = false;

    // Key
    static private string k_moving = "Moving";
    static private string k_hit = "Hit";

    private void Awake()
    {
        m_enemyBehaviour = GetComponent<EnemySnailBehaviour>();
        m_animator = GetComponent<Animator>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            nv_position.Value = transform.position;
            nv_localScale.Value = transform.localScale;

            SubribeEnemyMovement();
        }
        m_isConnected = true;
    }
    public override void OnNetworkDespawn()
    {
        if (IsServer)
        { 
            UnSubribeEnemyMovement();
        }
        m_isConnected = false;
    }

    private void Update()
    {
        if (IsServer)
        {
            UpdateNetworkPosition();
        } else
        {
            UpdateLocalPosition();
        }
    }

    private void UpdateNetworkPosition()
    {
        if (m_isConnected)
        {
            nv_position.Value = transform.position;
            nv_localScale.Value = transform.localScale;
        }
    }

    private void UpdateLocalPosition()
    {
        if (m_isConnected)
        {
            transform.position = nv_position.Value;
            transform.localScale = nv_localScale.Value;
        }
    }

    private void UnSubribeEnemyMovement()
    {
        m_enemyBehaviour.onMoved -= EnemySnailBehaviour_onMoved;
        m_enemyBehaviour.onStopped -= EnemySnailBehaviour_onStopped;
        m_enemyBehaviour.onChangedDirection -= EnemySnailBehaviour_onChangedDirection;
    }

    private void SubribeEnemyMovement()
    {
        UnSubribeEnemyMovement();

        m_enemyBehaviour.onMoved += EnemySnailBehaviour_onMoved;
        m_enemyBehaviour.onStopped += EnemySnailBehaviour_onStopped;
        m_enemyBehaviour.onChangedDirection += EnemySnailBehaviour_onChangedDirection;
    }

    private void EnemySnailBehaviour_onMoved(object sender, EventArgs e)
    {
        UpdateIsMovingClientRpc(true);
    }

    private void EnemySnailBehaviour_onStopped(object sender, EventArgs e)
    {
        UpdateIsMovingClientRpc(false);
    }

    [ClientRpc]
    private void UpdateIsMovingClientRpc(bool isMoving)
    {
        m_animator.SetBool(k_moving, isMoving);
    }

    private void EnemySnailBehaviour_onChangedDirection(object sender, EventArgs e)
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
