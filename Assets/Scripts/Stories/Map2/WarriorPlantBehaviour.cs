using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarriorPlantBehaviour : MonoBehaviour, IInteractable
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
                    text = "M*TH*R F*CK*R. I am born to fight zombies. Not these snails f*ck*r.",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "Oh. Hello there.",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "I have lost control in my anger. I am Warrior Plant, the protector of the forest, the first knight of the King,...",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "..., the undying and undefeated warrior, the one should not be eaten, the anti-zombie matter,...",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "...blah...blah...",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "... and the last wall of the forest.",
                    npc = DialogManager.NPC.WarriorPlant,
                },
                new DialogManager.Dialog
                {
                    text = "Sorry for keeping you waiting. The King is ahead this road. Please wake him up while I am holding these piece of sh*t from invading.",
                    npc = DialogManager.NPC.WarriorPlant,
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
                    text = "F*ck these snails. Where the f*ck is all the zombies.",
                    npc = DialogManager.NPC.WarriorPlant,
                }
            });
        }
    }
}
