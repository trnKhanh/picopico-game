using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAudioController : NetworkBehaviour
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
        if (!IsOwner)
            return;

        PlayServerRpc(state);
    }

    private void PlayImpl(PlayerAudioState state)
    {
        AudioClip audioClip = GetAudioClip(state);
        if (audioClip != null)
            m_audioSource.PlayOneShot(audioClip);
    }

    [ServerRpc]
    public void PlayServerRpc(PlayerAudioState state)
    {
        PlayClientRpc(state);
    }

    [ClientRpc]
    public void PlayClientRpc(PlayerAudioState state)
    {
        PlayImpl(state);
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
