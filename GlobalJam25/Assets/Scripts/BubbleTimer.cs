using UnityEngine;

public class BubbleTimer : MonoBehaviour
{
    public float bubbleLifeTime = 5.0f;

    public bool fastBubble;
    public bool bigBubble;

    private Animator animator;

    private void OnEnable()
    {
        Invoke("DestroyAnim", bubbleLifeTime);
        animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Enemy") && !collision.CompareTag("Player"))
        {
            if(bigBubble)
            {
                GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            }
            else
            {
                DestroyAnim();
            }
        }

        if (collision.CompareTag("BubbleBreak"))
        {
            DestroyBubble();
        }
    }

    public void DestroyAnim()
    {
        AudioManager.instance.PlayBubble();

        if (bigBubble)
        {
            animator.Play("ShootMedium");
        }
        else
        {
            animator.Play("ShootSmall");
        }
    }

    private void DestroyBubble()
    {
        Destroy(gameObject);
    }
}
