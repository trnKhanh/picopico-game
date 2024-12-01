using UnityEngine;
using System.Collections;
using System;
using Unity.Netcode;

public class PlayerMovement: NetworkBehaviour
{
    [SerializeField] private float runSpeed = 100.0f;
    [SerializeField] private float jumpForce = 7.0f;
    [SerializeField] private float feetRadius = 0.01f;
    [SerializeField] private float lookDownDistance = 4.0f;
    [SerializeField] private bool airControll = true;
    [SerializeField] private bool rawInput = true;
    [SerializeField] private Transform feet;
    [SerializeField] private LayerMask groundMask;

    // Events
    public event EventHandler onJumped;
    public event EventHandler onLanded;
    public event EventHandler onStopped;
    public event EventHandler onMoved;
    public event EventHandler onChangedDirection;

    private Rigidbody2D m_rigidbody;
    private Animator m_animator;
    private BoxCollider2D m_collider;

    private const float eps = 0.01f;

    // Player's states
    private bool m_isFacingRight = true;
    private bool m_isGrounded = false;
    private bool m_isRunning = false;

    private float m_inputX;
    private float m_inputY;
    private bool m_jump;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
        m_collider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        UpdateInput();
        UpdatePlayerStates();
    }

    private void FixedUpdate()
    {
        if (!IsOwner)
            return;

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
        if (m_isGrounded && !m_jump)
        {
            m_jump = Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W);
        }
    }

    private void UpdatePlayerStates()
    {
        if ((m_isFacingRight && m_inputX < -eps) || (!m_isFacingRight && m_inputX > eps))
        {
            m_isFacingRight = !m_isFacingRight;
            onChangedDirection?.Invoke(this, EventArgs.Empty);
        }

        if (Mathf.Abs(m_inputX) > eps)
        {
            if (!m_isRunning)
                onMoved?.Invoke(this, EventArgs.Empty);
            m_isRunning = true;
        }
        else
        {
            if (m_isRunning)
                onStopped?.Invoke(this, EventArgs.Empty);
            m_isRunning = false;
        }
    }

    private void UpdatePlayerStatesPhysics()
    {
        bool tmpGrounded = m_isGrounded;

        m_isGrounded = Physics2D.BoxCast(feet.position, m_collider.size, 0, Vector2.down, feetRadius, groundMask);

        // Check if player has just landed
        if (m_isGrounded && !tmpGrounded)
        {
            onLanded?.Invoke(this, EventArgs.Empty);
        }
    }

    private void Move()
    {
        if (m_isGrounded || airControll)
        {
            // Move player using velocity
            Vector2 velocity = m_rigidbody.velocity;
            velocity.x = m_inputX * runSpeed;
            m_rigidbody.velocity = velocity;
        }

        if (m_jump && m_isGrounded)
        {
            // Jump
            m_rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            onJumped?.Invoke(this, EventArgs.Empty);

            m_jump = false;
        }

        // Check if player want to look down
        if (m_isGrounded && m_inputY < 0)
        {
            CameraManager.Instance.SetOffset(new Vector2(0, -lookDownDistance));
        } else
        {
            CameraManager.Instance.SetOffset(Vector2.zero);
        }
    }
}
