using UnityEngine;

public class BubbleTimer : MonoBehaviour
{
    public float bubbleLifeTime = 4.0f;

    public bool fastBubble;
    public bool bigBubble;

    private void OnEnable()
    {
        Invoke("DestroyBubble", bubbleLifeTime);
    }

    private void DestroyBubble()
    {
        Destroy(gameObject);
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
                AudioManager.instance.PlayBubble();
                Destroy(gameObject);
            }
        }
    }
}
