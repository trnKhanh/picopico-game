using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite avatar;
    [SerializeField] private string npcName;
    [SerializeField] private AudioClip voice;
    [SerializeField] private FadingScreenUI fadingScreen;

    private bool interacted = false;
    private Animator m_animator;

    static private string k_die = "Die";

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
    }

    private void DialogManager_onFinished(object sender, EventArgs e)
    {
        DialogManager.onFinished -= DialogManager_onFinished;
        m_animator.SetTrigger(k_die);
    }

    private void Die()
    {
        StartCoroutine(DieEffect());
    }

    private IEnumerator DieEffect()
    {
        yield return new WaitForSeconds(2);
        fadingScreen.Fade();
    }

    public void Interact()
    {
        if (!interacted)
        {
            AudioManager.Instance.PlayBackgroundMusic(AudioManager.BackgroundState.SadLevel);
            DialogManager.onFinished += DialogManager_onFinished;
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "Hmmm......hmmm........hmmm......Cough....cough.....Cough....cough.....Cough....cough.....",
                    npc = DialogManager.NPC.King
                },
                new DialogManager.Dialog
                {
                    text = "Hello there, I guess this is the end for me. Thank you for comming, but I guess it is too late already.",
                    npc = DialogManager.NPC.King
                },
                new DialogManager.Dialog
                {
                    text = "I leave this forest's fate in your hands, my friend. I need to go now. I have not done much. If only I have more power.",
                    npc = DialogManager.NPC.King
                },
                new DialogManager.Dialog
                {
                    text = "Hmmm......hmmm........hmmm......Cough....cough.....Cough....cough.....Cough....cough.....",
                    npc = DialogManager.NPC.King
                },
            });
            interacted = true;
        }
    }
}
