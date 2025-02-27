using UnityEngine;
using UnityEngine.Events;
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
    private Document currentDocument;
    private bool isInteractingWithDocument = false;

    private void Start()
    {
        InitializeLineRenderer();
    }

    private void Update()
    {
        HandleObjectInteraction();
        HandleDocumentInteraction();
        HighlightObject();

        if (isHoldingObject)
        {
            GameManager.Instance.TogglePopup(true, false);
            GameManager.Instance.TogglePopup(false, false);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                GameManager.Instance.TogglePauseMenu(true);
            }
            else
            {
                GameManager.Instance.TogglePauseMenu(false);
            }
        }
    }

    private void FixedUpdate()
    {
        if (isHoldingObject)
        {
            MoveObjectToHoldPos();
        }
    }

    private void InitializeLineRenderer()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.startColor = Color.green;
    }

    private void HandleObjectInteraction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pickUpObject == null)
            {
                TryPickUpObject();
            }
            else
            {
                HandlePickUpObject();
            }
        }
    }

    private void HandleDocumentInteraction()
    {
        if (Input.GetMouseButtonDown(1))
        {
            if (!isHoldingObject)
            {
                if (currentDocument == null)
                {
                    TryInteractWithDocument();
                }
                else
                {
                    currentDocument.OnInteract(false);
                    GameManager.Instance.SetPlayerDocumentState(false);
                    isInteractingWithDocument = false;
                    currentDocument = null;
                }
            }
        }
    }

    private void HighlightObject()
    {
        if (!isHoldingObject)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                GameObject hitObject = hit.transform.gameObject;

                if (hitObject != highlightedObject)
                {
                    UnhighlightCurrentObject();
                    HighlightNewObject(hitObject);
                }

                if (hitObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    GameManager.Instance.TogglePopup(false, true);
                }
            }
            else
            {
                GameManager.Instance.TogglePopup(false, false);
                UnhighlightCurrentObject();
            }
        }
    }

    private void TryPickUpObject()
    {
        if (!isInteractingWithDocument)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                isHoldingObject = true;
                pickUpObject = hit.transform.gameObject;
                pickUpRb = pickUpObject.GetComponent<Rigidbody>();
                pickUpRb.useGravity = false;
                pickUpRb.freezeRotation = true;
                pickUpObject.transform.SetParent(objectHoldPos);
                pickUpObject.transform.localPosition = Vector3.zero;
            }
        }
    }

    private void HandlePickUpObject()
    {
        if (pickUpObject.TryGetComponent<Trinket>(out Trinket trinket))
        {
            if (trinket.inCustomerRange && isPlayerInCustomerRange)
            {
                HandleCustomerInteraction(trinket);
            }
            else
            {
                DropObject();
            }
        }
        else
        {
            DropObject();
        }
    }

    private void HandleCustomerInteraction(Trinket trinket)
    {
        CustomerLogic currentCustomer = GameManager.Instance.currentCustomer;

        ItemRequirements itemRequirements = new ItemRequirements
        {
            ActiveRequirements = trinket.ActiveRequirements,
            RequiredRace = trinket.requiredRace,
            RequiredKingdom = trinket.requiredKingdom,
            RequiredOccupation = trinket.requiredOccupation,
            RequiredAge = trinket.requiredAge
        };

        bool meetsRequirements = GameManager.Instance.CheckRequirements
            (itemRequirements,
            currentCustomer.customerRace,
            currentCustomer.customerKingdom,
            currentCustomer.customerOccupation,
            currentCustomer.customerAge);

        Debug.Log(meetsRequirements ? "Customer meets the trinket's requirements!" : "Customer does not meet the trinket's requirements.");
        Destroy(pickUpObject);
        pickUpObject = null;
        isHoldingObject = false;

        GameManager.Instance.OnSale.Invoke();
        StartCoroutine(GameManager.Instance.SpawnNextCustomer(GameManager.Instance.customerIndex, 7f));
    }

    private void TryInteractWithDocument()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
        {
            IInteractable interactable = hit.transform.GetComponent<IInteractable>();
            if (interactable != null)
            {
                currentDocument = hit.transform.GetComponent<Document>();
                interactable.OnInteract(true);
                GameManager.Instance.SetPlayerDocumentState(true);
                isInteractingWithDocument = true;
            }
        }
    }

    private void UnhighlightCurrentObject()
    {
        if (highlightedObject != null)
        {
            highlightedObject.GetComponent<IHighlightable>().OnHighlight(false);
            highlightedObject = null;
        }
    }

    private void HighlightNewObject(GameObject hitObject)
    {
        if (hitObject.TryGetComponent<IHighlightable>(out IHighlightable highlightable))
        {
            highlightable.OnHighlight(true);
            highlightedObject = hitObject;
        }
    }

    private void MoveObjectToHoldPos()
    {
        Vector3 direction = (objectHoldPos.position - pickUpObject.transform.position);
        Vector3 targetVelocity = direction * objectMoveSpeed;

        pickUpRb.linearVelocity = Vector3.SmoothDamp(pickUpRb.linearVelocity, targetVelocity, ref velocitySmoothDamp, smoothTime);
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
