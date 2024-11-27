using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioController : MonoBehaviour
{
    public enum PlayerAudioState
    {
        Run,
        Hit,
        Jump,
        Land,
        Die,
    }

    [Serializable]
    struct PlayerAudioClip {
        public PlayerAudioState state;
        public AudioClip audioClip;
    }

    [SerializeField] private PlayerAudioClip[] playerAudioClips;

    private AudioSource m_audioSource;

    private void Awake()
    {
        m_audioSource = GetComponent<AudioSource>();
    }

    public void Play(PlayerAudioState state)
    {
        AudioClip audioClip = GetAudioClip(state);
        m_audioSource.PlayOneShot(audioClip);
    }

    private AudioClip GetAudioClip(PlayerAudioState state)
    {
        foreach (PlayerAudioClip clip in playerAudioClips)
        {
            if (state == clip.state)
            {
                return clip.audioClip;
            }
        }
        return null;
    }
}
