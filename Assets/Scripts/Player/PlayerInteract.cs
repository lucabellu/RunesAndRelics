using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform playerCam;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;

            if(Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                print("Hit");
                DrawRay(playerCam.transform.position, playerCam.transform.forward * interactDistance, Color.red);
            }
            else
            {
                print("No Hit");
                DrawRay(playerCam.transform.position, playerCam.transform.forward * interactDistance, Color.green);
            }

            
        }
    }

    private void DrawRay(Vector3 start, Vector3 end, Color colour)
    {
        lineRenderer.enabled = true;

        lineRenderer.enabled = true; // Enable the LineRenderer
        lineRenderer.SetPosition(0, start); // Set the start point
        lineRenderer.SetPosition(1, end); // Set the end point
        lineRenderer.startColor = colour; // Set the start color
        lineRenderer.endColor = colour;

        Invoke("DisableLineRenderer", 5f); // Disable after 1 second

    }

    private void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}
