using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    public Transform crane;
    public Transform hook;
    private LineRenderer lineRenderer; // LineRenderer to render the rope

    void Start()
    {
        // Initialize line renderer
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        // Update the start and end positions of the line renderer to match the crane and hook
        lineRenderer.SetPosition(0, crane.position); // Set start position (crane)
        lineRenderer.SetPosition(1, hook.position);  // Set end position (hook)
    }
}
