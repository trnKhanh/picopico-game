using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
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
    public event EventHandler onDied;

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
    private NetworkVariable<bool> nv_isRunning = new NetworkVariable<bool>(false);
    private NetworkVariable<float> nv_velocityY = new NetworkVariable<float>(0f);
    private NetworkVariable<Color> nv_playerColour = new NetworkVariable<Color>(Color.white);

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
        SubcribeNetworkVariables();
        if (IsOwner)
        {
            SetColourServerRpc(PlayerStateManager.Instance.playerColour);
            SubcribePlayerMovementEvent();
            SubcribePlayerStateEvent();
        }

        if (IsServer)
        {
            nv_curHealth.Value = maxHealth;
        }
        onAppeared?.Invoke(this, EventArgs.Empty);
    }

    public override void OnNetworkDespawn()
    {
        UnSubcribeNetworkVariables();
        if (IsOwner)
        {
            UnSubcribePlayerMovementEvent();
            UnSubcribePlayerStateEvent();
        }
        onDissapeared?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        if (IsOwner)
        {
            UpdateNetworkPosition();
            UpdateNetworkAnimation();
        } else
        {
            UpdateLocalPosition();
            UpdateLocalAnimation();
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

    private void UpdateNetworkAnimation()
    {
        m_animator.SetFloat(k_velocityY, m_rigidbody.velocity.y);
        UpdateVelocityYServerRpc(m_rigidbody.velocity.y);
    }
    private void UpdateLocalAnimation()
    {
        m_animator.SetBool(k_running, nv_isRunning.Value);
        m_animator.SetFloat(k_velocityY, nv_velocityY.Value);
    }

    private void SubcribeNetworkVariables()
    {
        UpdateToNetworkVariables();
        Debug.Log("SubcribeNetworkVariables");
        UnSubcribeNetworkVariables();
        nv_playerColour.OnValueChanged += playerColour_onValueChange;
    }

    private void UpdateToNetworkVariables()
    {
        m_spriteRenderer.color = nv_playerColour.Value;
    }

    private void UnSubcribeNetworkVariables()
    {
        Debug.Log("UnSubcribeNetworkVariables");
        nv_playerColour.OnValueChanged -= playerColour_onValueChange;
    }

    private void playerColour_onValueChange(Color previousValue, Color newValue)
    {
        Debug.Log($"New color: {newValue}");
        m_spriteRenderer.color = newValue;
    }

    [ServerRpc]
    private void UpdateNetworkPositionServerRpc(Vector3 newPosition, Vector3 newLocalScale)
    {
        nv_position.Value = newPosition;
        nv_localScale.Value = newLocalScale;
    }

    [ServerRpc]
    private void UpdateVelocityYServerRpc(float newVelocityY)
    {
        nv_velocityY.Value = newVelocityY;
    }

    [ServerRpc]
    private void SetColourServerRpc(Color colour)
    {
        nv_playerColour.Value = colour;
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

    private void SubcribePlayerStateEvent()
    {
        UnSubcribePlayerStateEvent();

        PlayerStateManager.onUpdatedPlayerState += PlayerStateManager_onUpdatedPlayerState;
    }

    private void UnSubcribePlayerStateEvent()
    {
        PlayerStateManager.onUpdatedPlayerState -= PlayerStateManager_onUpdatedPlayerState;
    }

    private void PlayerStateManager_onUpdatedPlayerState(object sender, EventArgs e)
    {
        Debug.Log("PlayerStateManager_onUpdatedPlayerState");
        SetColourServerRpc(PlayerStateManager.Instance.playerColour);
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
        nv_isRunning.Value = isRunning;
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
        // Only owner fires events
        if (IsOwner)
        {
            onDied?.Invoke(this, EventArgs.Empty);
        }
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
