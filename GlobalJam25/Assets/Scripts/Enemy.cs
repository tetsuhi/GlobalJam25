using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;

    private int currentPointIndex = 0;
    private int direction = 1;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    public bool trapped;
    private float trappedCooldown1 = 5f;
    private float trappedCooldown2 = 10f;

    public SpriteRenderer bubbleSP;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (points.Length == 0 || trapped) return;

        Transform targetPoint = points[currentPointIndex];
        transform.position = Vector3.MoveTowards(transform.position, targetPoint.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPoint.position) < 0.01f)
        {
            currentPointIndex += direction;

            if (currentPointIndex >= points.Length || currentPointIndex < 0)
            {
                direction *= -1;
                currentPointIndex += direction;
            }
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = transform.position.x < targetPoint.position.x;
        }
    }

    private void Trap(bool slowTime)
    {
        trapped = true;
        bubbleSP.enabled = true;
        animator.Play("Hit");

        AudioManager.instance.PlayBubble();

        if(slowTime) StartCoroutine(BreakFreeSlow());
        else StartCoroutine(BreakFreeFast());
    }

    private IEnumerator BreakFreeFast()
    {
        yield return new WaitForSeconds(trappedCooldown1);

        UnTrap();
    }
    private IEnumerator BreakFreeSlow()
    {
        yield return new WaitForSeconds(trappedCooldown2);

        UnTrap();
    }

    private void UnTrap()
    {
        trapped = false;
        bubbleSP.enabled = false;
        animator.Play("Idle");

        AudioManager.instance.PlayBubble();

        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Bubble") && !trapped)
        {
            bool longTime = collision.GetComponent<BubbleTimer>().fastBubble;
            Destroy(collision.gameObject);
            Trap(longTime);
        }

        else if (collision.CompareTag("Player") && trapped)
        {
            Invoke("UnTrap", 0.1f);
        }
    }
}
