using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadishEndBehaviour : MonoBehaviour, IInteractable
{
    [SerializeField] private Sprite avatar;
    [SerializeField] private string npcName;
    [SerializeField] private AudioClip voice;

    private bool interacted = false;
    public void Interact()
    {
        if (!interacted)
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "You are fast. Come on, let's save the king.",
                    npc = DialogManager.NPC.RadishBoy
                },
                new DialogManager.Dialog
                {
                    text = "Let's venture.",
                    npc = DialogManager.NPC.RadishBoy
                }
            });
            interacted = true;
        }
        else
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "Let's go.",
                    npc = DialogManager.NPC.RadishBoy
                }
            });
        }
    }
}
