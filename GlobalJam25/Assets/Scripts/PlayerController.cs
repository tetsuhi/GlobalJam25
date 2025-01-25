using System;
using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class PlayerController : MonoBehaviour
{
    private float accelerationSmoothingFloor = 20.0f;
    private float accelerationSmoothingAir = 5.0f;
    private float speed = 6f;
    private float jumpForce = 10f;
    private float bubbleForce = 12f;

    private SpriteRenderer characterSr;
    private Rigidbody2D rb;

    private Vector2 direction;
    private bool isOnFloor;
    private bool coyoteJump;
    private float coyoteJumpTime = 0.1f;
    public LayerMask groundMask;

    public GameObject gun;
    private SpriteRenderer gunSr;

    public GameObject smallBubble;
    public float bubbleVelocity;
    private bool bubbleCooldown;

    private GameManager gameManager;
    public GameObject cameraFollow;

    private bool beenHit;
    private bool uncontrollable;
    private int hitTime = 10;

    private Animator animator;

    private void Start()
    {
        characterSr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gunSr = gun.GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        gameManager = FindFirstObjectByType<GameManager>();
    }

    void Update()
    {
        CheckGround();

        if (uncontrollable) return;

        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        FlipCharacter();
        Animations();
        
        if(Input.GetButtonDown("Jump") && coyoteJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if(Input.GetButtonDown("Fire1") && !bubbleCooldown)
        {
            ShootBubble();        
        }
    }

    private void FixedUpdate()
    {
        if (uncontrollable) return;

        float accelerationSmoothing = isOnFloor ? accelerationSmoothingFloor : accelerationSmoothingAir;

        rb.linearVelocity = new Vector2(
            Mathf.Lerp(rb.linearVelocity.x, 
            direction.x * speed, 
            1 - Mathf.Exp(-Time.deltaTime * accelerationSmoothing)),
            rb.linearVelocityY
        );
    }

    private void CheckGround()
    {
        float centerDistance = 0.39f;
        float distance = 0.4f;

        Vector2 positionL = transform.position + Vector3.left * centerDistance + Vector3.up * 0.1f;
        Vector2 positionR = transform.position + Vector3.right * centerDistance + Vector3.up * 0.1f;

        bool hit1 = Physics2D.Raycast(positionL, -Vector3.up, distance, groundMask);
        bool hit2 = Physics2D.Raycast(positionR, -Vector3.up, distance, groundMask);

        isOnFloor = hit1 || hit2;

        Debug.DrawRay(positionL, -Vector3.up * distance, hit1 ? Color.green : Color.red);
        Debug.DrawRay(positionR, -Vector3.up * distance, hit2 ? Color.green : Color.red);

        if (!isOnFloor)
        {
            Invoke("DisableCoyoteJump", coyoteJumpTime);
        }
        else
        {
            coyoteJump = true;
        }
    }

    private void DisableCoyoteJump()
    {
        coyoteJump = false;
    }

    private void FlipCharacter()
    {
        if (direction.x == 1)
        {
            characterSr.flipX = true;
            gunSr.flipX = true;
        }
        else if (direction.x == -1)
        {
            characterSr.flipX = false;
            gunSr.flipX = false;
        }
        
        
        if (direction.y == 1)
        {
            if(characterSr.flipX) gun.transform.rotation = Quaternion.Euler(0, 0, 90);
            else gun.transform.rotation = Quaternion.Euler(0, 0, -90);
            cameraFollow.transform.localPosition = Vector3.up;
        }
        else if (direction.y == -1)
        {
            if (characterSr.flipX) gun.transform.rotation = Quaternion.Euler(0, 0, -90);
            else gun.transform.rotation = Quaternion.Euler(0, 0, 90);
            cameraFollow.transform.localPosition = Vector3.down;
        }
        else
        {
            gun.transform.rotation = Quaternion.Euler(0, 0, 0);
            cameraFollow.transform.localPosition = Vector3.zero;
        }
    }

    private void ShootBubble()
    {
        bubbleCooldown = true;
        Invoke("BubbleReady", 0.4f); ;

        var bubble = Instantiate(smallBubble, transform.position + new Vector3(0, 0.4f, 0), transform.rotation, transform);
        bubble.transform.parent = null;
        Rigidbody2D bubbleRb = bubble.GetComponent<Rigidbody2D>();
        if (direction.y == 1)
        {
            bubble.transform.position += Vector3.up * 0.5f;
            bubbleRb.linearVelocity = new Vector2(0, bubbleVelocity);
        }
        else if (direction.y == -1)
        {
            bubble.transform.position -= Vector3.up * 0.5f;
            bubbleRb.linearVelocity = new Vector2(0, -bubbleVelocity);
        }
        else
        {
            if (characterSr.flipX)
            {
                bubble.transform.position += Vector3.right * 0.5f;
                bubbleRb.linearVelocity = new Vector2(bubbleVelocity, 0);
            }
            else
            {
                bubble.transform.position -= Vector3.right * 0.5f;
                bubbleRb.linearVelocity = new Vector2(-bubbleVelocity, 0);
            }
        }
        bubbleRb.linearVelocity += rb.linearVelocity / 3;
    }

    private void Animations()
    {
        if (rb.linearVelocityY > 0.1f)
        {
            animator.Play("Jump");
        }
        else if (rb.linearVelocityY < -0.1f)
        {
            animator.Play("Fall");
        }
        else
        {
            if(direction.x != 0)
            {
                animator.Play("Run");
            }
            else
            {
                animator.Play("Idle");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();

            if(enemy.trapped)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bubbleForce);
                return;
            }

            if (beenHit) return;

            beenHit = true;
            uncontrollable = true;

            Vector3 collisionDirection = transform.position - collision.transform.position;
            collisionDirection = collisionDirection.normalized;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(collisionDirection * 5f, ForceMode2D.Impulse);

            StartCoroutine(Hit());
        }
    }

    private IEnumerator Hit()
    {
        gameManager.UpdateHp(-1);

        int aux = 0;
        while (aux < hitTime)
        {
            aux += 1;
            characterSr.enabled = aux % 2 == 0;
            yield return new WaitForSeconds(0.1f);

            if(aux > 5) uncontrollable = false;
        }

        beenHit = false;
    }

    private void BubbleReady()
    {
        bubbleCooldown = false;
    }
}
