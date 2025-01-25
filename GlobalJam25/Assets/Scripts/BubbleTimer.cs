using UnityEngine;

public class BubbleTimer : MonoBehaviour
{
    public float bubbleLifeTime = 2.0f;

    public bool fastBubble;

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
        if (!collision.CompareTag("Enemy"))
        {
            AudioManager.instance.PlayBubble();
            Destroy(gameObject);
        }
    }
}
