using System;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    public float accelerationSmoothingFloor = 20.0f;
    public float accelerationSmoothingAir = 5.0f;
    public float speed = 2.5f;
    public float jumpForce = 5.0f;
    private SpriteRenderer sr;
    private Rigidbody2D rb;
    private BoxCollider2D bc;
    private Vector2 targetVelocity;
    public Vector2 direction;
    public bool isOnFloor;
    public LayerMask groundMask;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        CheckGround();
        direction = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (direction.x > 0) sr.flipX = true;
        else if (direction.x < 0) sr.flipX = false;
        targetVelocity = direction * speed;
        
        if(Input.GetButtonDown("Jump") && isOnFloor)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
    }

    private void FixedUpdate()
    {
        float accelerationSmoothing;
        if (isOnFloor) accelerationSmoothing = accelerationSmoothingFloor;
        else accelerationSmoothing = accelerationSmoothingAir;
        rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, targetVelocity, 1 - Mathf.Exp(-Time.deltaTime * accelerationSmoothing));
    }

    private void CheckGround()
    {
        float centerDistance = 0.39f;
        float distance = 0.4f;

        Vector2 positionL = transform.position + Vector3.left * centerDistance - Vector3.up * 0.6f;
        Vector2 positionR = transform.position + Vector3.right * centerDistance - Vector3.up * 0.6f;

        bool hit1 = Physics2D.Raycast(positionL, -Vector3.up, distance, groundMask);
        bool hit2 = Physics2D.Raycast(positionR, -Vector3.up, distance, groundMask);

        isOnFloor = hit1 || hit2;

        Debug.DrawRay(positionL, -Vector3.up * distance, hit1 ? Color.green : Color.red);
        Debug.DrawRay(positionR, -Vector3.up * distance, hit2 ? Color.green : Color.red);
    }
}
