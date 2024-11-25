using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float runSpeed = 100.0f;
    [SerializeField] private float jumpForce = 7.0f;

    private Rigidbody2D rb2d;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool facingRight = true;
    private bool running = false;

    private string Animator_Running = "Running";
    private string Animator_Hit = "Hit";
    private string Animator_VelocityY = "VelocityY";

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
        
    }

    private void Move()
    {
        Vector3 positionChange = Vector3.zero;

        positionChange.x += InputManager.Instance.horizontalInput;

        transform.position += positionChange * runSpeed;

        if (InputManager.Instance.jump)
        {
            rb2d.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }


        if (Mathf.Abs(positionChange.x) > 0.001f)
        {
            running = true;
            facingRight = positionChange.x > 0f;
        } else
        {
            running = false;
        }
        

        // Update animation
        animator.SetBool(Animator_Running, running);
        animator.SetFloat(Animator_VelocityY, rb2d.velocity.y);

        spriteRenderer.flipX = !facingRight;
    }
}
