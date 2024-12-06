using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockBehaviour : MonoBehaviour, IInteractable
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
                    text = "DANGER AHEAD!!! DANGER AHEAD!!!",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "But we are in danger. The army of snails is comming.",
                    audioClip = null,
                    avatar = avatar,
                },
                new DialogManager.Dialog
                {
                    name = npcName,
                    text = "The King is surrounded by furious foes. We have no choice but to put our fate in your hands. Please wake the King up!!!",
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
                    text = "DANGER AHEAD!!! DANGER AHEAD!!!",
                    audioClip = null,
                    avatar = avatar,
                }
            });
        }
    }
}
