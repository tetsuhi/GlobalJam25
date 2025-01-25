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
    private float bubbleForce = 13f;

    private SpriteRenderer characterSr;
    private Rigidbody2D rb;

    public Vector2 direction;
    private bool isOnFloor;
    private bool coyoteJump;
    private float coyoteJumpTime = 0.1f;
    public LayerMask groundMask;

    public GameObject gun;
    private SpriteRenderer gunSr;

    public GameObject smallBubble;
    public GameObject bigBubble;
    public float bubbleVelocity;
    public float bubbleSlowVelocity = 2.5f;
    public float bubbleFastVelocity = 10f;
    private bool bubbleCooldown;
    public float chargeTime = 2f;
    public bool chargeShoot;

    public bool powerUp1;
    public bool powerUp2;

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

        bubbleVelocity = bubbleSlowVelocity;
    }

    void Update()
    {
        CheckGround();

        if (uncontrollable) return;

        direction = new Vector2(
            Input.GetAxisRaw("Horizontal") > 0.1f ? 1 : (Input.GetAxisRaw("Horizontal") < -0.1f ? -1 : 0),
            Input.GetAxisRaw("Vertical") > 0.1f ? 1 : (Input.GetAxisRaw("Vertical") < -0.1f ? -1 : 0)
        );

        FlipCharacter();
        Animations();
        
        if(Input.GetButtonDown("Jump") && coyoteJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            AudioManager.instance.PlayJump();
        }

        if (!powerUp2)
        {
            if (Input.GetButtonDown("Fire1") && !bubbleCooldown)
            {
                ShootBubble(false);
            }
        }
        else
        {
            if(Input.GetButtonDown("Fire1") && !bubbleCooldown)
            {
                StartCoroutine(ChargeBubble());
            }
            else if(Input.GetButtonUp("Fire1") && !bubbleCooldown)
            {
                ShootBubble(chargeShoot);
                StopAllCoroutines();
                chargeShoot = false;
            }
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

        bool wasGrounded = isOnFloor;
        isOnFloor = hit1 || hit2;

        if(!wasGrounded && isOnFloor) AudioManager.instance.PlayLand();

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

    private void ShootBubble(bool charge)
    {
        bubbleCooldown = true;
        Invoke("BubbleReady", 0.4f);

        AudioManager.instance.PlayBubble();

        GameObject bubbleGO = charge ? bigBubble : smallBubble;

        var bubble = Instantiate(bubbleGO, transform.position + new Vector3(0, 0.4f, 0), transform.rotation, transform);
        bubble.transform.parent = null;
        Rigidbody2D bubbleRb = bubble.GetComponent<Rigidbody2D>();
        bubble.GetComponent<BubbleTimer>().fastBubble = powerUp1;
        bubble.GetComponent<BubbleTimer>().bigBubble = charge;
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

            try
            {
                if (enemy.trapped)
                {
                    AudioManager.instance.PlayJump();
                    rb.linearVelocity = new Vector2(rb.linearVelocity.x, bubbleForce);
                    return;
                }
            }
            catch { }

            if (beenHit) return;

            AudioManager.instance.PlayHit();

            beenHit = true;
            uncontrollable = true;

            Vector3 collisionDirection = transform.position - collision.transform.position;
            collisionDirection = collisionDirection.normalized;

            rb.linearVelocity = Vector2.zero;
            rb.AddForce(collisionDirection * 5f, ForceMode2D.Impulse);

            StartCoroutine(Hit());
        }

        if (collision.CompareTag("Flag"))
        {
            gameManager.LoadNextLevel();
        }

        if (collision.CompareTag("PowerUp1"))
        {
            ActivePowerUp1();
            AudioManager.instance.PlayPickUp();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("PowerUp2"))
        {
            ActivePowerUp2();
            AudioManager.instance.PlayPickUp();
            Destroy(collision.gameObject);
        }

        if (collision.CompareTag("Bubble"))
        {
            if (collision.GetComponent<Rigidbody2D>().linearVelocity == Vector2.zero)
            {
                collision.GetComponent<BubbleTimer>().DestroyAnim();
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, bubbleForce);
                AudioManager.instance.PlayJump();
            }
        }
    }

    public void ActivePowerUp1()
    {
        bubbleVelocity = bubbleFastVelocity;
        powerUp1 = true;
    }

    public void ActivePowerUp2()
    {
        powerUp2 = true;
    }

    private IEnumerator Hit()
    {
        gameManager.UpdateHp(-1);

        int aux = 0;
        while (aux < hitTime)
        {
            aux += 1;
            characterSr.enabled = aux % 2 == 0;
            gunSr.enabled = aux % 2 == 0;
            yield return new WaitForSeconds(0.1f);

            if(aux > 5) uncontrollable = false;
        }

        characterSr.enabled = true;
        gunSr.enabled = true;

        beenHit = false;
    }

    private void BubbleReady()
    {
        bubbleCooldown = false;
    }

    private IEnumerator ChargeBubble()
    {
        yield return new WaitForSeconds(chargeTime);
        chargeShoot = true;
    }
}
