using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadishBehaviour : MonoBehaviour, IInteractable
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
                    text = "Hello, I am Radish Boy. Welcome to the forest.",
                    npc = DialogManager.NPC.RadishBoy
                },
                new DialogManager.Dialog
                {
                    text = "The forest is attacked by the snails army. They are killing all the plants!!!",
                    npc = DialogManager.NPC.RadishBoy
                },
                new DialogManager.Dialog
                {
                    text = "We cannot deal with such strong foes. Please help us.",
                    npc = DialogManager.NPC.RadishBoy
                },
                new DialogManager.Dialog
                {
                    text = "However, you cannot eradicate them by yourself. You must get to the Forest King and wake him from the Forever Sleep.",
                    npc = DialogManager.NPC.RadishBoy
                }
            });
            interacted = true;
        } else
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "What are you waiting for. Let's go.",
                    npc = DialogManager.NPC.RadishBoy
                }
            });
        }
    }
}
