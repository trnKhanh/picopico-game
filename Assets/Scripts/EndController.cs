using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndController : MonoBehaviour
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
        if (m_ended)
            return;

        if (collision.gameObject.tag == "Player")
        {
            EndEffect();
        }
    }

    private void EndEffect()
    {
        m_ended = true;
        m_animator.SetTrigger(k_press);
    }

    private void End()
    {
        SceneLoadingManager.Instance.LoadScene(SceneLoadingManager.SceneType.Tutorial);
    }
}
