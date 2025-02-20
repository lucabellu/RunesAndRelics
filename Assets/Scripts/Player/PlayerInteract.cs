using UnityEngine;
using static GameManager;

public class PlayerInteract : MonoBehaviour
{
    #region Object Detection
    [Header("Object detection")]
    [Tooltip("Minimum distance between player and object needed to interact.")]
    [SerializeField] private float interactDistance;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Transform playerCam;
    private GameObject highlightedObject;
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

    private bool isHoldingObject = false;
    private Vector3 velocitySmoothDamp;
    #endregion

    private bool isPlayerInCustomerRange = false;

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
                    if (trinket.inCustomerRange && isPlayerInCustomerRange)
                    {
                        CustomerLogic currentCustomer = GameManager.Instance.currentCustomer;

                        ItemRequirements itemRequirements = new ItemRequirements
                        {
                            ActiveRequirements = trinket.ActiveRequirements,
                            RequiredRace = trinket.requiredRace,
                            RequiredKingdom = trinket.requiredKingdom,
                            RequiredOccupation = trinket.requiredOccupation,
                            RequiredLevel = trinket.requiredLevel
                        };

                        bool meetsRequirements = GameManager.Instance.CheckRequirements
                            (itemRequirements, 
                            currentCustomer.customerRace, 
                            currentCustomer.customerKingdom, 
                            currentCustomer.customerOccupation, 
                            currentCustomer.customerLevel);

                        if (meetsRequirements)
                        {
                            Debug.Log("Customer meets the trinket's requirements!");
                        }
                        else
                        {
                            Debug.Log("Customer does not meet the trinket's requirements.");
                        }
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

        if (!isHoldingObject)
        {
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject != highlightedObject)
                {
                    if (highlightedObject != null)
                    {
                        highlightedObject.GetComponent<IHighlightable>().OnHighlight(false);
                    }
                }

                if (hitObject.TryGetComponent<IHighlightable>(out IHighlightable highlightable))
                {
                    highlightable.OnHighlight(true);
                    highlightedObject = hitObject;
                }
            }
            else
            {
                if (highlightedObject != null)
                {
                    highlightedObject.GetComponent<IHighlightable>().OnHighlight(false);
                    highlightedObject = null;
                }
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
            isHoldingObject = true;

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
            isHoldingObject = false;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            isPlayerInCustomerRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            isPlayerInCustomerRange = false;
        }
    }
}
