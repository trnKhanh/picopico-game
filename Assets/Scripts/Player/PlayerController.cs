using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerMovement m_playerMovement;
    private PlayerAudioController m_playerAudioController;

    private void Awake()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerAudioController = GetComponent<PlayerAudioController>();
    }

    private void Start()
    {
        SubcribePlayerMovementEvent();
        
    }

    private void SubcribePlayerMovementEvent()
    {
        m_playerMovement.onJumped -= PlayerMovement_onJumped;
        m_playerMovement.onLanded -= PlayerMovement_onLanded;

        m_playerMovement.onJumped += PlayerMovement_onJumped;
        m_playerMovement.onLanded += PlayerMovement_onLanded;
    }

    private void PlayerMovement_onJumped(object sender, EventArgs e)
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Jump);
    }

    private void PlayerMovement_onLanded(object sender, EventArgs e)
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Land);
    }
}
