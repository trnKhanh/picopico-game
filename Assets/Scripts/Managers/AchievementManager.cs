using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    static public AchievementManager Instance { get; private set; }

    [SerializeField] private TMP_Text popUpText;
    [SerializeField] private TMP_Text popUpDescription;
    [SerializeField] private BaseAchievement[] baseAchievements;
    [SerializeField] private Animator m_animator;

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
        LoadAchievementState();
        Application.wantsToQuit += Application_wantsToQuit;
    }

    private bool Application_wantsToQuit()
    {
        SaveAchievementState();
        return true;
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
        foreach (BaseAchievement baseAchievement in baseAchievements)
        {
            baseAchievement.onAchieved += BaseAchievement_onAchieved;
        }
    }

    private void UnSubribeToBaseAchievements()
    {
        foreach (BaseAchievement baseAchievement in baseAchievements)
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
        if (m_animator == null || AudioManager.Instance == null)
            return;
        popUpText.text = achievement.text;
        popUpDescription.text = achievement.description;
        m_animator.SetTrigger(k_popup);
        AudioManager.Instance.PlaySFX(AudioManager.SFXState.Achievement);
    }

    private void SaveAchievementState()
    {
        string path = Path.Join(Application.persistentDataPath, "Achievement.txt");
        string data = "";
        for (int i = 0; i < baseAchievements.Length; ++i)
        {
            data += $"{i}:{baseAchievements[i].isAchieved}\n";
        }
        File.WriteAllText(path, data);
    }
    private void LoadAchievementState()
    {
        string path = Path.Join(Application.persistentDataPath, "Achievement.txt");
        if (!File.Exists(path))
            return;
        string[] data = File.ReadAllLines(path);
        for (int i = 0; i < data.Length; ++i)
        {
            string[] parts = data[i].Split(':');
            if (parts.Length < 2)
                continue;
            int id = Int32.Parse(parts[0]);
            bool isAchieved = Boolean.Parse(parts[1]);
            baseAchievements[id].isAchieved = isAchieved;
        }
        
    }
}