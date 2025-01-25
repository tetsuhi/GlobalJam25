using UnityEngine;

public class Prallax : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector2 parallaxMultiplier;

    private Vector3 startPosition;
    private Vector3 lastCameraPosition;

    private void Start()
    {
        startPosition = transform.position;
        lastCameraPosition = cameraTransform.position;
    }

    private void Update()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;

        Vector3 newPosition = startPosition + new Vector3(
            (cameraTransform.position.x - startPosition.x) * parallaxMultiplier.x,
            (cameraTransform.position.y - startPosition.y) * parallaxMultiplier.y + 3,
            transform.position.z
        );

        transform.position = newPosition;

        lastCameraPosition = cameraTransform.position;
    }
}