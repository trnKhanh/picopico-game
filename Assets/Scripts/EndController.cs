using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndController : NetworkBehaviour
{
    private Animator m_animator;

    private bool m_ended = false;

    private static string k_press = "Press";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!IsServer)
            return;

        if (m_ended)
            return;

        if (collision.gameObject.tag == "Player")
        {
            m_ended = true;
            EndEffect();
        }
    }

    private void EndEffect()
    {
        m_animator.SetTrigger(k_press);
    }

    private void End()
    {
        SceneStateManager.Instance.End();
    }
}
