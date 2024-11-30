using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerController : NetworkBehaviour, IDamgable
{
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private float invincibleTime = 2.0f;
    [SerializeField] private float invincibleFadedInterval = 0.2f;
    [SerializeField] private float invincibleFadedAmount = 0.4f;

    static public event EventHandler onAppeared;
    static public event EventHandler onDissapeared;
    public event EventHandler onHit;

    public PlayerMovement playerMovement { get; private set; }
    public PlayerAudioController playerAudioController { get; private set; }
    private Animator m_animator;
    private Rigidbody2D m_rigidbody;
    private SpriteRenderer m_spriteRenderer;

    private NetworkVariable<Vector3> nv_position = new NetworkVariable<Vector3>();
    private NetworkVariable<Vector3> nv_localScale = new NetworkVariable<Vector3>();
    private NetworkVariable<int> nv_curHealth = new NetworkVariable<int>();
    private NetworkVariable<bool> nv_isInvincible = new NetworkVariable<bool>(false);
    private NetworkVariable<bool> nv_died = new NetworkVariable<bool>(false);

    // Key
    static private string k_running = "Running";
    static private string k_hit = "Hit";
    static private string k_disappear = "Disappear";
    static private string k_velocityY = "VelocityY";

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerAudioController = GetComponent<PlayerAudioController>();
        m_animator = GetComponent<Animator>();
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            SubcribePlayerMovementEvent();
            CameraManager.Instance.target = transform;
        }

        if (IsServer)
        {
            onAppeared?.Invoke(this, EventArgs.Empty);
            nv_curHealth.Value = maxHealth;
        }       
    }

    public override void OnNetworkDespawn()
    {
        if (IsOwner)
        {
            UnSubcribePlayerMovementEvent();
        }
        if (IsServer)
        {
            onDissapeared?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Update()
    {
        if (IsOwner)
        {
            UpdateNetworkPosition();
            UpdateAnimation();
        } else
        {
            UpdateLocalPosition();
        }

    }

    private void UpdateNetworkPosition()
    {
        UpdateNetworkPositionServerRpc(transform.position, transform.localScale);
    }

    private void UpdateLocalPosition()
    {
        transform.position = nv_position.Value;
        transform.localScale = nv_localScale.Value;
    }

    [ServerRpc]
    private void UpdateNetworkPositionServerRpc(Vector3 newPosition, Vector3 newLocalScale)
    {
        nv_position.Value = newPosition;
        nv_localScale.Value = newLocalScale;
    }

    private void UpdateAnimation()
    {
        UpdateVelocityYServerRpc(m_rigidbody.velocity.y);
    }

    [ServerRpc]
    private void UpdateVelocityYServerRpc(float newVelocityY)
    {
        UpdateVelocityYClientRpc(newVelocityY);
    }
    [ClientRpc]
    private void UpdateVelocityYClientRpc(float newVelocityY)
    {

        m_animator.SetFloat(k_velocityY, newVelocityY);
    }

    private void UnSubcribePlayerMovementEvent()
    {
        playerMovement.onMoved -= PlayerMovement_onMoved;
        playerMovement.onStopped -= PlayerMovement_onStopped;
        playerMovement.onChangedDirection -= PlayerMovement_onChangedDirection;
        playerMovement.onJumped -= PlayerMovement_onJumped;
        playerMovement.onLanded -= PlayerMovement_onLanded;
    }

    private void SubcribePlayerMovementEvent()
    {
        UnSubcribePlayerMovementEvent();

        playerMovement.onMoved += PlayerMovement_onMoved;
        playerMovement.onStopped += PlayerMovement_onStopped;
        playerMovement.onChangedDirection += PlayerMovement_onChangedDirection;
        playerMovement.onJumped += PlayerMovement_onJumped;
        playerMovement.onLanded += PlayerMovement_onLanded;
    }

    private void PlayerMovement_onMoved(object sender, EventArgs e)
    {
        m_animator.SetBool(k_running, true);
        UpdateIsRunningServerRpc(true);
    }

    private void PlayerMovement_onStopped(object sender, EventArgs e)
    {
        m_animator.SetBool(k_running, false);
        UpdateIsRunningServerRpc(false);
    }

    [ServerRpc]
    private void UpdateIsRunningServerRpc(bool isRunning)
    {
        UpdateIsRunningClientRpc(isRunning);
    }

    [ClientRpc]
    private void UpdateIsRunningClientRpc(bool isRunning)
    {
        m_animator.SetBool(k_running, isRunning);
    }

    private void PlayerMovement_onChangedDirection(object sender, EventArgs e)
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    private void PlayerMovement_onJumped(object sender, EventArgs e)
    {
        playerAudioController.Play(PlayerAudioController.PlayerAudioState.Jump);
    }

    private void PlayerMovement_onLanded(object sender, EventArgs e)
    {
        playerAudioController.Play(PlayerAudioController.PlayerAudioState.Land);
    }

    public void Hit(int damage)
    {
        // Only the server is in charge of these logic.
        if (!IsServer)
            return;

        if (nv_died.Value || nv_isInvincible.Value)
            return;

        nv_curHealth.Value -= damage;
        if (nv_curHealth.Value <= 0)
        {
            Die();
            return;
        }
        StartCoroutine(InvincibleCountDown());
        TriggerHitEffectClientRpc();
    }


    private IEnumerator InvincibleCountDown()
    {
        nv_isInvincible.Value = true;
        yield return new WaitForSeconds(invincibleTime);
        nv_isInvincible.Value = false;
    }

    private IEnumerator HitEffect()
    {
        playerAudioController.Play(PlayerAudioController.PlayerAudioState.Hit);
        m_animator.SetTrigger(k_hit);

        // Only owner fires events
        if (IsOwner)
        {
            onHit?.Invoke(this, EventArgs.Empty);
        }

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
    }
    [ClientRpc]
    private void TriggerHitEffectClientRpc()
    {
        StartCoroutine(HitEffect());
    }

    private void SetSpriteOpacity(float alpha)
    {
        Color fadedColor = m_spriteRenderer.color;
        fadedColor.a = alpha;
        m_spriteRenderer.color = fadedColor;
    }

    public void Die()
    {
        nv_died.Value = true;
        TriggerDieEffectClientRpc();
    }

    [ClientRpc]
    private void TriggerDieEffectClientRpc()
    {
        DieEffect();
    }

    private void DieEffect()
    {
        playerMovement.enabled = false;
        playerAudioController.Play(PlayerAudioController.PlayerAudioState.Die);
        m_animator.SetTrigger(k_disappear);
    }

    public void Disappear()
    {
        if (IsOwner)
        {
            DisappearServerRpc();
        }
    }

    [ServerRpc]
    private void DisappearServerRpc()
    {
        NetworkObject.Despawn(true);
    }
}
