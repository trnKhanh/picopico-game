using UnityEngine;
using System.Collections;
using System;

public class PlayerMovement: MonoBehaviour
{
    [SerializeField] private float runSpeed = 100.0f;
    [SerializeField] private float jumpForce = 7.0f;
    [SerializeField] private float feetRadius = 0.01f;
    [SerializeField] private float lookDownDistance = 4.0f;
    [SerializeField] private float normalDistance = 0.01f;
    [SerializeField] private bool airControll = true;
    [SerializeField] private bool rawInput = true;
    [SerializeField] private Transform feet;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private FollowPlayer followPlayer;

    // Events
    public event EventHandler onHit;
    public event EventHandler onJumped;
    public event EventHandler onLanded;
    public event EventHandler onMoved;

    private Rigidbody2D m_rigidbody;
    private Animator m_animator;

    private const float eps = 0.01f;

    // Player's states
    private bool m_isFacingRight = true;
    private bool m_isRunning = false;
    private bool m_isGrounded = false;

    // Key
    static private string k_running = "Running";
    static private string k_hit = "Hit";
    static private string k_velocityY = "VelocityY";

    private float m_inputX;
    private float m_inputY;
    private bool m_jump;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateInput();
        UpdatePlayerStates();
        UpdateAntimation();
    }

    private void FixedUpdate()
    {
        UpdatePlayerStatesPhysics();
        Move();
    }

    private void UpdateInput()
    {
        if (rawInput)
        {
            m_inputX = InputManager.Instance.horizontalInputRaw;
            m_inputY = InputManager.Instance.verticalInputRaw;
        } else {
            m_inputX = InputManager.Instance.horizontalInput;
            m_inputY = InputManager.Instance.verticalInput;
        }

        m_jump = m_inputY > 0 || Input.GetKeyDown(KeyCode.Space);
    }

    private void UpdatePlayerStates()
    {
        if (Mathf.Abs(m_inputX) > eps)
        {
            m_isFacingRight = m_inputX > 0f;
        }

        m_isRunning = Mathf.Abs(m_inputX) > eps;
    }

    private void UpdatePlayerStatesPhysics()
    {
        bool tmpGrounded = m_isGrounded;
        m_isGrounded = Physics2D.Raycast(feet.position, Vector2.down, feetRadius, groundMask);

        // Check if player has just landed
        if (m_isGrounded && !tmpGrounded)
        {
            onLanded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void UpdateAntimation()
    {
        m_animator.SetBool(k_running, m_isRunning);
        m_animator.SetFloat(k_velocityY, m_rigidbody.velocity.y);
    }

    private void Move()
    {
        Vector3 localScale = transform.localScale;
        localScale.x = (m_isFacingRight ? 1 : -1) * Mathf.Abs(localScale.x);
        transform.localScale = localScale;

        if (m_isGrounded || airControll)
        {
            // Move player using velocity
            Vector2 velocity = m_rigidbody.velocity;
            velocity.x = m_inputX * runSpeed;
            m_rigidbody.velocity = velocity;

            onMoved?.Invoke(this, EventArgs.Empty);
        }

        if (m_jump && m_isGrounded)
        {
            // Jump
            m_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            onJumped?.Invoke(this, EventArgs.Empty);
        }

        // Check if player want to look down
        if (m_isGrounded && m_inputY < 0)
        {
            followPlayer.SetOffset(new Vector2(0, -lookDownDistance));
        } else
        {
            followPlayer.SetOffset(Vector2.zero);
        }
    }
}
