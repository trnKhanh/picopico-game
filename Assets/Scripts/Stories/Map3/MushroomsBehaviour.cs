using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomBehaviour : MonoBehaviour, IInteractable
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
                    text = "We are servants of the King.",
                    npc = DialogManager.NPC.Mushrooms
                },
                new DialogManager.Dialog
                {
                    text = "He is ahead this road. But it is swarmed by the snail army already.",
                    npc = DialogManager.NPC.Mushrooms
                },
                new DialogManager.Dialog
                {
                    text = "Please help us to save the king.",
                    npc = DialogManager.NPC.Mushrooms
                },
            });
            interacted = true;
        }
        else
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "Be hurry, please.",
                    npc = DialogManager.NPC.Mushrooms
                }
            });
        }
    }
}
