using UnityEngine;

public class Slide : MonoBehaviour
{
    public float speed = 5f;  // Speed of sliding
    private Rigidbody2D rb;   // Reference to Rigidbody2D
    private Vector2 direction;  // Direction the item is moving

    public Camera mainCamera;  // Reference to the main camera
    private float startX;      // Starting X position of the item
    private float endX;        // Ending X position of the item

    void Start()
    {
        // Try to find the main camera if not assigned
        if (mainCamera == null)
        {
            mainCamera = Camera.main;  
        }

        rb = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        rb.gravityScale = 0f;  // Disable gravity
        rb.velocity = new Vector2(speed, 0f);  // Start moving right

        // Set the horizontal bounds to slide from
        startX = -mainCamera.orthographicSize * mainCamera.aspect;  // Left edge of the screen
        endX = mainCamera.orthographicSize * mainCamera.aspect;  // Right edge of the screen
    }

    void Update()
    {
        // Move the item along the X axis
        Vector2 position = transform.position;

        // If the item reaches the edge, reverse direction
        if (position.x >= endX || position.x <= startX)
        {
            speed = -speed;
            rb.velocity = new Vector2(speed, 0f);
        }

        // Apply clamping to prevent the item from moving off-screen
        position.x = Mathf.Clamp(position.x, startX, endX);

        // Update the position after clamping
        transform.position = position;
    }
}
