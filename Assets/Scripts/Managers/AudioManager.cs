using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public enum BackgroundState
    {
        Menu,
        NormalLevel,
        BossLevel,
    }
    public enum SFXState
    {
        Confirm,
        Cancle,
    }

    [Serializable]
    struct BackgroundAudioClip
    {
        public BackgroundState state;
        public AudioClip audioClip;
    }

    [Serializable]
    struct SFXAudioClip
    {
        public SFXState state;
        public AudioClip audioClip;
    }


    [SerializeField] private BackgroundAudioClip[] backgroundAudioClips;
    [SerializeField] private SFXAudioClip[] sfxAudioClips;

    [SerializeField] private AudioSource backgroundAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        PlayBackgroundMusic(BackgroundState.Menu);
    }

    public void PlayBackgroundMusic(BackgroundState state) 
    {
        AudioClip audioClip = GetBackgroundMusicAudioClip(state);
        backgroundAudioSource.clip = audioClip;
        backgroundAudioSource.Play();
    }

    private AudioClip GetBackgroundMusicAudioClip(BackgroundState state)
    {
        foreach (BackgroundAudioClip clip in backgroundAudioClips)
        {
            if (state == clip.state)
                return clip.audioClip;
        }
        return null;
    }

    public void PlaySFX(SFXState state)
    {
        AudioClip audioClip = GetSFXAudioClip(state);
        sfxAudioSource.PlayOneShot(audioClip);
    }

    private AudioClip GetSFXAudioClip(SFXState state)
    {
        foreach (SFXAudioClip clip in sfxAudioClips)
        {
            if (state == clip.state)
                return clip.audioClip;
        }
        return null;
    }
}
