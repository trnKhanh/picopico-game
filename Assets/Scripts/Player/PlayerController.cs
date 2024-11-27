using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IDamgable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float invincibleTime = 2.0f;
    [SerializeField] private float invincibleFadedInterval = 0.2f;
    [SerializeField] private float invincibleFadedAmount = 0.4f;

    static public event EventHandler onAppeared;
    static public event EventHandler onDied;
    public event EventHandler onHit;

    private PlayerMovement m_playerMovement;
    private PlayerAudioController m_playerAudioController;
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;
    private SpriteRenderer m_spriteRenderer;

    private int m_curHealth;
    private bool m_died = false;
    private bool m_isInvincible = false;

    // Key
    static private string k_running = "Running";
    static private string k_hit = "Hit";
    static private string k_disappear = "Disappear";
    static private string k_velocityY = "VelocityY";

    private void Awake()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerAudioController = GetComponent<PlayerAudioController>();
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        m_curHealth = maxHealth;

        // TODO; remove this in multiplayer
        CameraManager.Instance.target = transform;
    }

    private void OnEnable()
    {
        SubcribePlayerMovementEvent();

        onAppeared?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisable()
    {
        UnSubcribePlayerMovementEvent();
    }

    private void Update()
    {

        UpdateAnimation();
    }

    private void UpdateAnimation()
    {
        m_animator.SetFloat(k_velocityY, m_rigidbody.velocity.y);
    }

    private void UnSubcribePlayerMovementEvent()
    {
        m_playerMovement.onMoved -= PlayerMovement_onMoved;
        m_playerMovement.onStopped -= PlayerMovement_onStopped;
        m_playerMovement.onChangedDirection -= PlayerMovement_onChangedDirection;
        m_playerMovement.onJumped -= PlayerMovement_onJumped;
        m_playerMovement.onLanded -= PlayerMovement_onLanded;
    }

    private void SubcribePlayerMovementEvent()
    {
        UnSubcribePlayerMovementEvent();

        m_playerMovement.onMoved += PlayerMovement_onMoved;
        m_playerMovement.onStopped += PlayerMovement_onStopped;
        m_playerMovement.onChangedDirection += PlayerMovement_onChangedDirection;
        m_playerMovement.onJumped += PlayerMovement_onJumped;
        m_playerMovement.onLanded += PlayerMovement_onLanded;
    }

    private void PlayerMovement_onMoved(object sender, EventArgs e)
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Run);
        m_animator.SetBool(k_running, true);
    }

    private void PlayerMovement_onStopped(object sender, EventArgs e)
    {
        m_animator.SetBool(k_running, false);
    }

    private void PlayerMovement_onChangedDirection(object sender, EventArgs e)
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void PlayerMovement_onJumped(object sender, EventArgs e)
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Jump);
    }

    private void PlayerMovement_onLanded(object sender, EventArgs e)
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Land);
    }

    public void Hit(int damage)
    {
        if (m_died || m_isInvincible)
            return;

        m_curHealth -= damage;
        if (m_curHealth <= 0)
        {
            DieEffect();
            return;
        }

        StartCoroutine(HitEffect());
    }

    private IEnumerator HitEffect()
    {
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Hit);
        m_animator.SetTrigger(k_hit);
        onHit?.Invoke(this, EventArgs.Empty);

        m_isInvincible = true;

        bool isFaded = false;
        float timeLeft = invincibleTime;

        while (timeLeft > 0f)
        {
            SetSpriteOpacity(isFaded ? invincibleFadedAmount : 1);

            isFaded = !isFaded;

            yield return new WaitForSeconds(invincibleFadedInterval);
            timeLeft -= invincibleFadedInterval;
        }

        SetSpriteOpacity(1);
        m_isInvincible = false;
    }

    private void SetSpriteOpacity(float alpha)
    {
        Color fadedColor = m_spriteRenderer.color;
        fadedColor.a = alpha;
        m_spriteRenderer.color = fadedColor;
    }

    private void DieEffect()
    {
        m_died = true;
        m_playerMovement.enabled = false;
        m_playerAudioController.Play(PlayerAudioController.PlayerAudioState.Die);

        m_animator.SetTrigger(k_disappear);
    }

    public void Die()
    {
        onDied?.Invoke(this, EventArgs.Empty);
        gameObject.SetActive(false);
    }
}
