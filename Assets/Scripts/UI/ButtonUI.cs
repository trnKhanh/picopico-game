using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonUI : MonoBehaviour
{
    private Animator m_animator;
    private Button m_button;

    static private string k_clicked = "Clicked";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        m_button.onClick.AddListener(() =>
        {
            m_animator.SetTrigger(k_clicked);
        });
    }

    private void OnDisable()
    {
        m_button.onClick.RemoveAllListeners();
    }
}
