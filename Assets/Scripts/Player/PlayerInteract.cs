using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    #region Object Detection
    [Header("Object detection")]
    [Tooltip("Minimum distance between player and object needed to interact.")]
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform playerCam;
    private LineRenderer lineRenderer;

    private GameObject pickUpObject;
    private Rigidbody pickUpRb;
    #endregion

    #region Held Object
    [Header("Held object")]
    [SerializeField] private Transform objectHoldPos;
    [SerializeField] private float objectMoveSpeed;
    [SerializeField] private float linearDampingValue;
    [SerializeField] private float smoothTime = 0.1f;
    private Vector3 velocitySmoothDamp;
    #endregion

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
    }

    private void Update()
    {
        RaycastHit hit;

        if (Input.GetMouseButtonDown(0))
        {
            if (pickUpObject == null)
            {
                PickUpObject();
            }
            else
            {
                if (pickUpObject.TryGetComponent<Trinket>(out Trinket trinket))
                {
                    if (trinket.inCustomerRange)
                    {
                        Destroy(trinket.gameObject);
                    }
                    else
                    {
                        DropObject();
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                IInteractable interactable = hit.transform.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.OnInteract();
                }
            }
        }

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
        {
            IHighlightable highlightable = hit.transform.GetComponent<IHighlightable>();
            if (highlightable != null)
            {
                highlightable.OnHighlight();
            }
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
        pickUpRb.linearVelocity = Vector3.SmoothDamp(pickUpRb.linearVelocity, targetVelocity, ref velocitySmoothDamp, smoothTime);

        //pickUpRb.linearVelocity += (objectHoldPos.position - pickUpObject.transform.position) * objectMoveSpeed * Time.fixedDeltaTime;
    }
}
