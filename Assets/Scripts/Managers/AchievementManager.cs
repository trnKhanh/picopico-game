using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    static public AchievementManager Instance { get; private set; }

    [SerializeField] private TMP_Text popUpText;

    [SerializeField] private BaseAchievement[] m_baseAchievements;
    private Animator m_animator;

    static private string k_popup = "Popup";

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        m_animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        SubribeToBaseAchievements();
    }

    private void OnDisable()
    {
        UnSubribeToBaseAchievements();
    }

    private void SubribeToBaseAchievements()
    {
        UnSubribeToBaseAchievements();
        foreach (BaseAchievement baseAchievement in m_baseAchievements)
        {
            baseAchievement.onAchieved += BaseAchievement_onAchieved;
        }
    }

    private void UnSubribeToBaseAchievements()
    {
        foreach (BaseAchievement baseAchievement in m_baseAchievements)
        {
            baseAchievement.onAchieved -= BaseAchievement_onAchieved;
        }
    }

    private void BaseAchievement_onAchieved(object sender, EventArgs e)
    {
        BaseAchievement achievement = (BaseAchievement)sender;
        ShowAchievementPopup(achievement);
    }

    private void ShowAchievementPopup(BaseAchievement achievement)
    {
        popUpText.text = achievement.text;
        m_animator.SetTrigger(k_popup);
    }
}