using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadingScreenUI : MonoBehaviour
{
    private Animator m_animator;

    static private string k_fade = "Fade";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    public void Fade()
    {
        m_animator.SetTrigger(k_fade);
    }

    private void End()
    {
        SceneStateManager.Instance.End();
    }
}
