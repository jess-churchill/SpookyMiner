using UnityEngine;

public class Bounce : MonoBehaviour
{
    public float speed = 5f;  // Speed at which the item moves
    private Rigidbody2D rb;   // Reference to Rigidbody2D
    private Vector2 direction;  // Direction the item is moving
    public Camera mainCamera;  // Reference to the main camera

    void Start()
    {
        // Try to find the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;  
        }

        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        rb.gravityScale = 0f;  // Disable gravity
        direction = new Vector2(1, 1).normalized;  // Set arbitrary initial direction
        rb.velocity = direction * speed;  // Apply initial velocity
    }

    void Update()
    {
        if (mainCamera == null) return;

        // Get the camera's orthographic size and aspect ratio
        float camWidth = mainCamera.orthographicSize * mainCamera.aspect;
        float camHeight = mainCamera.orthographicSize;

        // Get the item's position in world space
        Vector2 position = transform.position;

        // Bounce off top and bottom (Y axis)
        if (position.y > camHeight || position.y < -camHeight)
        {
            direction.y = -direction.y;  // Reverse Y direction
        }

        // Bounce off left and right (X axis)
        if (position.x > camWidth || position.x < -camWidth)
        {
            direction.x = -direction.x;  // Reverse X direction
        }

        // Ensure the item stays within camera bounds
        position.x = Mathf.Clamp(position.x, -camWidth, camWidth);
        position.y = Mathf.Clamp(position.y, -camHeight, camHeight);

        // Update the item's position after clamping
        transform.position = position;

        // Update velocity to reflect changes in direction
        rb.velocity = direction * speed;
    }
}
