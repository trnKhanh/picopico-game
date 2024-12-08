using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour, IInteractable
{
    private bool interacted = false;
    public void Interact()
    {
        if (!interacted)
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    text = "DANGER AHEAD!!! DANGER AHEAD!!!",
                    npc = DialogManager.NPC.MrRock
                },
                new DialogManager.Dialog
                {
                    text = "But we are in danger. The army of snails is comming.",
                    npc = DialogManager.NPC.MrRock
                },
                new DialogManager.Dialog
                {
                    text = "The King is surrounded by furious foes. We have no choice but to put our fate in your hands. Please wake the King up!!!",
                    npc = DialogManager.NPC.MrRock,
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
                    text = "DANGER AHEAD!!! DANGER AHEAD!!!",
                    npc = DialogManager.NPC.MrRock,
                }
            });
        }
    }
}
