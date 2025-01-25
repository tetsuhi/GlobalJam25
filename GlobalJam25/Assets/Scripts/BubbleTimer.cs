using UnityEngine;

public class BubbleTimer : MonoBehaviour
{
    public float bubbleLifeTime = 2.0f;

    private void OnEnable()
    {
        Invoke("DestroyBubble", bubbleLifeTime);
    }

    private void DestroyBubble()
    {
        Destroy(gameObject);
    }
}
