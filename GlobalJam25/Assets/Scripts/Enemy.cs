using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;

    private int currentPointIndex = 0;
    private int direction = 1;

    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (points.Length == 0) return;

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
}
