using System;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float accelerationSmoothingFloor = 20.0f;
    public float accelerationSmoothingAir = 5.0f;
    public float speed = 2.5f;
    public float jumpForce = 5.0f;
    private SpriteRenderer characterSr;
    private Rigidbody2D rb;
    public Vector2 direction;
    public bool isOnFloor;
    public LayerMask groundMask;
    public GameObject gun;
    private SpriteRenderer gunSr;

    private void Start()
    {
        characterSr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        gunSr = gun.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        CheckGround();
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        FlipCharacter();
        
        if(Input.GetButtonDown("Jump") && isOnFloor)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if(Input.GetButtonDown("Fire1"))
        {

        }
    }

    private void FixedUpdate()
    {
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
        }
        else if (direction.y == -1)
        {
            if (characterSr.flipX) gun.transform.rotation = Quaternion.Euler(0, 0, -90);
            else gun.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else
        {
            gun.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
