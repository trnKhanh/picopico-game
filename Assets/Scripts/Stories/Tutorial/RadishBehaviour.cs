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
                    name = npcName,
                    text = "Hello, I am Radish Boy. Welcome to the world",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "The forest is attacked by the snails army. They are killing all the plants!!!",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "We cannot deal with such strong foes. Please help us.",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "However, you cannot eradicate them by yourself. You must get to the Forest King and wake him from the Forever Sleep",
                    audioClip = null,
                    avatar = avatar,
                }
            });
            interacted = true;
        } else
        {
            DialogManager.Instance.PlayDialogs(new List<DialogManager.Dialog>
            {
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "What are you waiting for. Let's go.",
                    audioClip = null,
                    avatar = avatar,
                }
            });
        }
    }
}
