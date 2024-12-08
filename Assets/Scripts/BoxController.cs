using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BoxController : NetworkBehaviour
{
    private NetworkVariable<Vector3> nv_position = new NetworkVariable<Vector3>();

    private const float eps = 0.1f;
    private bool m_isPushing = false;

    private Vector3 m_lastPosition;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            nv_position.Value = transform.position;

        if (IsClient)
            nv_position.OnValueChanged += nv_position_OnValueChanged;
    }

    public override void OnNetworkDespawn()
    {
        nv_position.OnValueChanged -= nv_position_OnValueChanged;
    }

    private void nv_position_OnValueChanged(Vector3 previousValue, Vector3 newValue)
    {
        m_lastPosition = newValue;

        if (!m_isPushing)
            transform.position = newValue;
    }

    private void Update()
    {
        Vector3 distance = transform.position - nv_position.Value;
        Vector3 delta = transform.position - m_lastPosition;
        if (delta.magnitude > eps)
        {
            Debug.Log($"transform.position:{transform.position}; m_lastPosition={m_lastPosition}");
            m_isPushing = true;
            UpdatePostionServerRpc(transform.position);
        } else
        {
            m_isPushing = false;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePostionServerRpc(Vector3 position)
    {
        nv_position.Value = position;
    }
}
