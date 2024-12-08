using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : NetworkBehaviour
{
    public enum NPC {
        RadishBoy,
        MrRock,
        WarriorPlant,
        Mushrooms,
        King,
        End,
    };

    [Serializable]
    public class NPCData
    {
        public NPC npc;
        public string name = null;
        public Sprite avatar = null;
        public AudioClip audioClip = null;
    }


    [Serializable]
    public struct Dialog: INetworkSerializable
    {
        public NPC npc;
        public string text;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref npc);
            serializer.SerializeValue(ref text);
        }
    }

    static public DialogManager Instance { get; private set; }

    [SerializeField] private NPCData[] npcData;
    [SerializeField] private float textDelay = 0.1f;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private RectTransform dialogPanel;

    static public EventHandler onFinished;

    private AudioSource m_audioSource;
    private Coroutine m_animateTextCoroutine = null;
    private Dialog[] m_dialogs = null;
    private int m_curDialogId;
    private float m_closeTimeout = -1;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(Instance);

        m_audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        StartCoroutine(SubribeToNetworkManagerEvents());
    }

    private void OnDisable()
    {
        UnSubribeToNetworkManagerEvents();
    }

    private IEnumerator SubribeToNetworkManagerEvents()
    {
        yield return new WaitUntil(() => NetworkManager.Singleton != null);
        UnSubribeToNetworkManagerEvents();
        NetworkManager.Singleton.OnClientStopped += NetworkManager_OnClientStopped;
    }

    private void UnSubribeToNetworkManagerEvents()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientStopped -= NetworkManager_OnClientStopped;
    }

    private void NetworkManager_OnClientStopped(bool isHost)
    {
        HideDialog();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Skip();
        }
    }

    public void PlayDialogs(List<Dialog> dialogs, float timeout = -1)
    {
        SetDialogsServerRpc(dialogs.ToArray(), timeout);
        PlayNextDialog();
    }

    private void SetDialogsImpl(Dialog[] dialogs, float timeout = -1)
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.active = false;
        }
        m_curDialogId = 0;
        m_dialogs = dialogs;
        m_closeTimeout = timeout;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetDialogsServerRpc(Dialog[] dialogs, float timeout = -1)
    {
        Debug.Log("SetDialogsServerRpc");
        SetDialogsClientRpc(dialogs, timeout);
    }

    [ClientRpc]
    public void SetDialogsClientRpc(Dialog[] dialogs, float timeout = -1)
    {
        Debug.Log("SetDialogsClientRpc");
        SetDialogsImpl(dialogs, timeout);
    }

    private IEnumerator AnimateDialog(Dialog dialog)
    {
        StartDialog(dialog);

        // Typing effect
        dialogText.maxVisibleCharacters = 0;
        int total = dialogText.text.Length;

        for (int i = 1; i <= total; ++i)
        {
            dialogText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(textDelay);
        }

        FinishDialog();

        // If the closeTimeout is set, close the dialog automatically after the timeout
        if (m_closeTimeout >= 0f)
        {
            yield return new WaitForSeconds(m_closeTimeout);
            PlayNextDialog();
        }
    }

    private void PlayNextDialog()
    {
        PlayNextDialogServerRpc();
    }

    private void PlayNextDialogImpl()
    {
        if (m_dialogs != null && m_curDialogId < m_dialogs.Length)
        {
            m_animateTextCoroutine = StartCoroutine(AnimateDialog(m_dialogs[m_curDialogId++]));
        } else
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.active = true;
            }
            if (m_dialogs != null)
                onFinished?.Invoke(this, EventArgs.Empty);
            m_dialogs = null;
            m_curDialogId = 0;
            HideDialog();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void PlayNextDialogServerRpc()
    {
        PlayNextDialogClientRpc();
    }

    [ClientRpc]
    private void PlayNextDialogClientRpc()
    {
        PlayNextDialogImpl();
    }

    private void StartDialog(Dialog dialog)
    {
        ShowDialog();
        NPCData data = GetNPCData(dialog.npc);
        if (data.avatar != null)
        {
            avatarImage.gameObject.SetActive(true);
            avatarImage.sprite = data.avatar;
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }
        nameText.text = data.name;
        dialogText.text = dialog.text;

        m_audioSource.clip = data.audioClip;
        m_audioSource.Play();
    }

    private void FinishDialog()
    {
        m_audioSource.Stop();
        m_animateTextCoroutine = null;
    }

    private void Skip()
    {
        if (m_animateTextCoroutine != null)
        {
            // Stop the animation and show all text
            Debug.Log("DialogManager:StopCoroutine");
            StopCoroutine(m_animateTextCoroutine);
            dialogText.maxVisibleCharacters = dialogText.text.Length;

            FinishDialog();

            // If the closeTimeout is set, it means that player want to close
            // the dialog immediately.
            if (m_closeTimeout >= 0f)
            {
                PlayNextDialog();
            }
        } else
        {
            PlayNextDialog();
        }
    }

    private void ShowDialog()
    {
        dialogPanel.gameObject.SetActive(true);
    }

    private void HideDialog()
    {
        dialogPanel.gameObject.SetActive(false);
    }

    private NPCData GetNPCData(NPC npc)
    {
        foreach (NPCData data in npcData)
        {
            if (npc == data.npc)
            {
                return data;
            }
        }
        return null;
    }
}
