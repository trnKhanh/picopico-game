using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public class Dialog
    {
        public string name = null;
        public Sprite avatar = null;
        public string text;
        public AudioClip audioClip = null;
    }

    static public DialogManager Instance { get; private set; }

    [SerializeField] private float textDelay = 0.1f;
    [SerializeField] private Image avatarImage;
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private RectTransform dialogPanel;

    static public EventHandler onFinished;

    private AudioSource m_audioSource;
    private Coroutine m_animateTextCoroutine = null;
    private List<Dialog> m_dialogs = null;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            Skip();
        }
    }

    public void PlayDialogs(List<Dialog> dialogs, float timeout = -1)
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.active = false;
        }
        m_curDialogId = 0;
        m_dialogs = dialogs;
        m_closeTimeout = timeout;
        PlayNextDialog();
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
        if (m_dialogs != null && m_curDialogId < m_dialogs.Count)
        {
            Debug.Log(m_dialogs);
            Debug.Log(m_curDialogId);
            m_animateTextCoroutine = StartCoroutine(AnimateDialog(m_dialogs[m_curDialogId++]));
        } else
        {
            if (InputManager.Instance != null)
            {
                InputManager.Instance.active = true;
            }

            onFinished?.Invoke(this, EventArgs.Empty);
            HideDialog();
        }
    }

    private void StartDialog(Dialog dialog)
    {
        ShowDialog();

        if (dialog.avatar != null)
        {
            avatarImage.gameObject.SetActive(true);
            avatarImage.sprite = dialog.avatar;
        }
        else
        {
            avatarImage.gameObject.SetActive(false);
        }
        nameText.text = dialog.name;
        dialogText.text = dialog.text;

        m_audioSource.clip = dialog.audioClip;
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
}
