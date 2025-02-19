using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform playerCam;
    private LineRenderer lineRenderer;

    private GameObject pickUpObject;
    private Rigidbody pickUpRb;
    [SerializeField] private Transform objectHoldPos;
    [SerializeField] private float objectMoveSpeed;
    [SerializeField] private float linearDampingValue;

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
            if (pickUpObject == null)
            {
                PickUpObject();
            }
            else
            {
                DropObject();
            }

            /* Put this in getmousedown 1 to check with raycast then open any ui element for it for example for a script/book
             * 
             * IInteractable interactable = transform.GetComponent<IInteractable>();
             * if (interactable != null)
                {
                    interactable.OnInteract();
                }
             */
        }
    }

    private void FixedUpdate()
    {
        if (pickUpObject != null)
        {
            MoveObjectToHoldPos();
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

    private void PickUpObject()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
        {
            pickUpObject = hit.transform.gameObject;
            pickUpRb = pickUpObject.GetComponent<Rigidbody>();
            //pickUpRb.linearDamping = linearDampingValue;

            pickUpRb.useGravity = false;
            pickUpRb.freezeRotation = true;

            pickUpObject.transform.SetParent(objectHoldPos);
            pickUpObject.transform.localPosition = Vector3.zero;
        }


    }

    private void DropObject()
    {
        if (pickUpObject != null)
        {
            pickUpRb.useGravity = true;
            pickUpRb.freezeRotation = false;

            pickUpObject.transform.SetParent(null);
            pickUpObject = null;
            pickUpRb = null;
        }
    }

    private void MoveObjectToHoldPos()
    {
        // Calculate the direction and distance to the target position
        Vector3 direction = (objectHoldPos.position - pickUpObject.transform.position);
        Vector3 targetVelocity = direction * objectMoveSpeed;

        // Apply damping to reduce oscillation
        pickUpRb.linearVelocity = Vector3.Lerp(pickUpRb.linearVelocity, targetVelocity, linearDampingValue * Time.fixedDeltaTime);

        //pickUpRb.linearVelocity += (objectHoldPos.position - pickUpObject.transform.position) * objectMoveSpeed * Time.fixedDeltaTime;
    }
}
