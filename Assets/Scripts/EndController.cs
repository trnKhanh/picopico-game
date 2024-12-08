using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndController : NetworkBehaviour, IInteractable
{
    [SerializeField] private Sprite avatar;
    [SerializeField] private AudioClip voice;

    private Animator m_animator;

    private bool m_ended = false;

    private static string k_press = "Press";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void EndEffect()
    {
        m_animator.SetTrigger(k_press);
    }

    private void End()
    {
        AudioManager.Instance.PlaySFX(AudioManager.SFXState.End);
        SceneStateManager.Instance.End();
    }

    public void Interact()
    {
        if (m_ended)
            return;

        DialogManager.onFinished += DialogManager_onFinished;
        DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
        {
            new DialogManager.Dialog
            {
                text = "Let's me carry you to the king",
                npc = DialogManager.NPC.End
            }
        });
    }

    private void DialogManager_onFinished(object sender, EventArgs e)
    {
        DialogManager.onFinished -= DialogManager_onFinished;
        EndEffect();
    }
}
