using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySnailBahaviour : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float wanderingSpeed = 5.0f;
    [SerializeField] private float chaseSpeed = 5.0f;
    [SerializeField] private float destinationRadius = 0.25f;
    [SerializeField] private float wanderingMinRange = 3.0f;
    [SerializeField] private float wanderingMaxRange = 4.0f;
    [SerializeField] private float wanderingInterval = 5.0f;


    private const float eps = 0.1f;

    // Enemies states
    private Vector2 m_anchorPosition;
    private Vector2 m_destination;
    private bool m_isMoving = false;
    private bool m_isFacingRight = false;
    private float m_moveSpeed = 0;
    private List<Transform> m_chaseTargets = new List<Transform>();

    // Key
    static private string k_walking = "Walking";
    static private string k_hit = "Hit";

    private Rigidbody2D m_rigidbody;
    private Animator m_animator;
    private Coroutine m_wanderingCoroutine;

    private void Awake()
    {
        m_rigidbody = GetComponent<Rigidbody2D>();
        m_animator = GetComponent<Animator>();
    }

    private void Start()
    {
        m_anchorPosition = transform.position;
        m_destination = m_anchorPosition;
    }

    private void Update()
    {
        UpdateBehaviour();
        UpdateEnemyStates();
        UpdateAnimation();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void UpdateBehaviour()
    {
        if (m_chaseTargets.Count > 0)
        {
            StopWandering();
            m_destination = m_chaseTargets[0].position;
            m_moveSpeed = chaseSpeed;
        } else
        {
            StartWandering();
            m_moveSpeed = wanderingSpeed;
        }
    }

    private void UpdateEnemyStates()
    {
        m_isMoving = m_rigidbody.velocity.magnitude > eps;

        if (Mathf.Abs(m_destination.x - transform.position.x) > eps)
        {
            m_isFacingRight = m_destination.x - transform.position.x > eps;
        }
    }

    private void UpdateAnimation()
    {
        m_animator.SetBool(k_walking, m_isMoving);

        Vector3 localScale = transform.localScale;
        localScale.x = (m_isFacingRight ? 1 : -1) * Mathf.Abs(localScale.x);
        transform.localScale = localScale;
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
            float randomDistance = Random.Range(0, wanderingDelta) + wanderingMinRange;
            bool goToLeft = transform.position.x > m_anchorPosition.x;
            m_destination.x = m_anchorPosition.x + randomDistance * (goToLeft ? -1 : 1);
            yield return new WaitForSeconds(wanderingInterval);
        }
    }

    private void Move()
    {
        Vector2 velocity = m_rigidbody.velocity;

        if (Mathf.Abs(transform.position.x - m_destination.x) > destinationRadius)
        {
            if (transform.position.x - m_destination.x > destinationRadius)
            {
                velocity.x = -m_moveSpeed;
            } else
            {
                velocity.x = m_moveSpeed;
            }
        } else
        {
            velocity.x = 0;
        }

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
}
