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
                    name = npcName,
                    text = "M*TH*R F*CK*R. I am born to fight zombies. Not these snails f*ck*r.",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "Oh. Hello there.",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "I have lost control in my anger. I am Warrior Plant, the protector of the forest, the first knight of the King,...",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "..., the undying and undefeated warrior, the one should not be eaten, the anti-zombie matter,...",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "...blah...blah...",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "... and the last wall of the forest.",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "Sorry for keeping you waiting. The King is ahead this road. Please wake him up while I am holding these piece of sh*t from invading.",
                    audioClip = null,
                    avatar = avatar,
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
                    name = npcName,
                    text = "F*ck these snails. Where the f*ck is all the zombies.",
                    audioClip = null,
                    avatar = avatar,
                }
            });
        }
    }
}
