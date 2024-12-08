using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class EnemyBehaviour : NetworkBehaviour
{
    [SerializeField] protected int damage = 1;
    [SerializeField] protected float wanderingSpeed = 5.0f;
    [SerializeField] protected float chaseSpeed = 5.0f;
    [SerializeField] protected float destinationRadius = 0.25f;
    [SerializeField] protected float wanderingMinRange = 3.0f;
    [SerializeField] protected float wanderingMaxRange = 4.0f;
    [SerializeField] protected float wanderingInterval = 5.0f;

    public event EventHandler onMoved;
    public event EventHandler onStopped;
    public event EventHandler onChangedDirection;

    private const float eps = 0.1f;

    // Enemies states
    private Vector2 m_anchorPosition;
    private Vector2 m_destination;
    private bool m_isMoving = false;
    private bool m_isFacingRight = true;
    private float m_moveSpeed = 0;
    private float m_velocityX = 0;
    private List<Transform> m_chaseTargets = new List<Transform>();

    private Rigidbody2D m_rigidbody;
    private Coroutine m_wanderingCoroutine;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        m_anchorPosition = transform.position;
        m_destination = m_anchorPosition;
    }

    private void Update()
    {
        if (!IsServer)
        {
            return;
        }

        UpdateBehaviour();
        UpdateMovementState();
        UpdateEnemyStates();
    }

    private void FixedUpdate()
    {
        if (!IsServer)
        {
            return;
        }

        Move();
    }

    private void UpdateBehaviour()
    {
        if (m_chaseTargets.Count > 0)
        {
            StopWandering();
            m_destination = m_chaseTargets[0].position;
            m_moveSpeed = chaseSpeed;
        }
        else
        {
            StartWandering();
            m_moveSpeed = wanderingSpeed;
        }
    }

    private void UpdateMovementState()
    {
        if (Mathf.Abs(m_destination.x - transform.position.x) > destinationRadius)
        {
            if (m_destination.x - transform.position.x > eps)
            {
                m_velocityX = m_moveSpeed;
            }
            else
            {
                m_velocityX = -m_moveSpeed;
            }
        }
        else
        {
            m_velocityX = 0;
        }
    }

    private void UpdateEnemyStates()
    {
        if ((m_isFacingRight && m_velocityX < -eps) || (!m_isFacingRight && m_velocityX > eps))
        {
            m_isFacingRight = !m_isFacingRight;

            InvokeOnChangedDirection(EventArgs.Empty);
        }

        if (Mathf.Abs(m_velocityX) > eps)
        {
            if (!m_isMoving)
                InvokeOnMoved(EventArgs.Empty);
            m_isMoving = true;
        }
        else
        {
            if (m_isMoving)
                InvokeOnStopped(EventArgs.Empty);
            m_isMoving = false;
        }
    }

    private void StartWandering()
    {
        if (m_wanderingCoroutine == null)
            m_wanderingCoroutine = StartCoroutine(Wandering());
    }

    private void StopWandering()
    {
        if (m_wanderingCoroutine != null)
            StopCoroutine(m_wanderingCoroutine);
        m_wanderingCoroutine = null;
    }

    IEnumerator Wandering()
    {
        while (true)
        {
            float wanderingDelta = wanderingMaxRange - wanderingMinRange;
            float randomDistance = UnityEngine.Random.Range(0, wanderingDelta) + wanderingMinRange;
            bool goToLeft = transform.position.x > m_anchorPosition.x;
            m_destination.x = m_anchorPosition.x + randomDistance * (goToLeft ? -1 : 1);
            yield return new WaitForSeconds(wanderingInterval);
        }
    }

    private void Move()
    {
        Vector2 velocity = m_rigidbody.velocity;
        velocity.x = m_velocityX;
        m_rigidbody.velocity = velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_chaseTargets.Add(collision.gameObject.transform);
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            m_chaseTargets.Remove(collision.gameObject.transform);
        }
    }


    private void OnCollisionStay2D(Collision2D collision)
    {
        float contactAngle = Vector2.Angle(collision.contacts[0].normal, Vector2.up);
        float damageAngleRange = 100;
        float minAngle = (180 - damageAngleRange) / 2;
        float maxAngle = 90 + damageAngleRange / 2;
        bool dealDamage = minAngle < contactAngle && contactAngle < maxAngle;
        GameObject other = collision.gameObject;
        IDamgable damgable;
        if (dealDamage)
        {
            if (other.TryGetComponent<IDamgable>(out damgable))
            {
                if (other.tag == "Player")
                {
                    damgable.Hit(damage);
                }
            }
        }
    }

    protected void InvokeOnMoved(EventArgs e)
    {
        onMoved?.Invoke(this, e);
    }

    protected void InvokeOnStopped(EventArgs e)
    {
        onStopped?.Invoke(this, e);
    }

    protected void InvokeOnChangedDirection(EventArgs e)
    {
        onChangedDirection?.Invoke(this, e);
    }
}
