using UnityEngine;
using UnityEngine.Diagnostics;
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
    [SerializeField] private float maxObjectMoveSpeed;
    [SerializeField] private float linearDampingValue;
    [SerializeField] private float smoothTime;

    private bool isHoldingObject = false;
    private Vector3 velocitySmoothDamp;
    #endregion

    private bool isPlayerInCustomerRange = false;
    private Document currentDocument;
    private bool isInteractingWithDocument = false;

    [SerializeField] private AudioSource playFlipThrough;
    [SerializeField] private AudioClip flip;

    [SerializeField] private AudioSource playPickUp;
    [SerializeField] private AudioClip pickUp;

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

        
        if (pickUpObject != null && pickUpObject.TryGetComponent<Trinket>(out Trinket trinket) && trinket.inCustomerRange && isPlayerInCustomerRange)
        {
            GameManager.Instance.currentCustomer.GetComponent<Outline>().enabled = true;
        }
        else if (pickUpObject != null && !isPlayerInCustomerRange && GameManager.Instance.currentCustomer != null)
        {
            GameManager.Instance.currentCustomer.GetComponent<Outline>().enabled = false;
        }

    }

    private void FixedUpdate()
    {
        if (isHoldingObject)
        {
            MoveObjectToHoldPos();
        }
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

    private void TryPickUpObject()
    {
        if (!isInteractingWithDocument)
        {
            RaycastHit hit;
            if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out hit, interactDistance, layerMask))
            {
                if (hit.transform.CompareTag("Door"))
                {
                    if (hit.transform.GetComponent<ShopDoor>().canInteract)
                    {
                        GameManager.Instance.EndDay();
                    }
                   
                }
                else if (hit.transform.CompareTag("Customer") && GameManager.Instance.currentCustomer.GetComponent<CustomerMovement>().isAtCounter)
                {
                    HandleCustomerInteraction(Instance.currentCustomer.purchaseTrinket);
                }
                else
                {
                    if (hit.transform.CompareTag("Customer")) return;

                    isHoldingObject = true;
                    pickUpObject = hit.transform.gameObject;
                    pickUpRb = pickUpObject.GetComponent<Rigidbody>();
                    pickUpRb.useGravity = false;
                    pickUpRb.freezeRotation = true;
                    pickUpRb.isKinematic = true;
                    pickUpObject.transform.SetParent(objectHoldPos);
                    playPickUp.PlayOneShot(pickUp);
                }
                
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
            RequiredAge = trinket.requiredAge,
            RequiredGuild = trinket.requiredGuild,
            RequiredGuildRank = trinket.requiredGuildRank
        };

        bool meetsRequirements = GameManager.Instance.CheckRequirements
            (itemRequirements,
            currentCustomer.customerRace,
            currentCustomer.customerKingdom,
            currentCustomer.customerOccupation,
            currentCustomer.customerAge,
            currentCustomer.customerGuild,
            currentCustomer.customerGuildRank);

        if (isHoldingObject && meetsRequirements || !isHoldingObject && !meetsRequirements)
        {
            //HandleSuccessfulTransaction();
            print("handled successfully");
        }
        else
        {
            //HandleFailedTransaction();
            print("handled unsuccessfully");
        }

        if (pickUpObject != null)
        {
            Destroy(pickUpObject);
            pickUpObject = null;
            isHoldingObject = false;
        }

        if (currentCustomer.hasDocuments)
        {
            print("Has documents");
            foreach (GameObject document in currentCustomer.activeDocuments)
            {
                Destroy(document);
            }
        }

        GameManager.Instance.OnSale.Invoke();
        StartCoroutine(GameManager.Instance.SpawnNextCustomer(7f, GameManager.Instance.currentCustomers));
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

                if (hit.transform.CompareTag("Manifest"))
                {
                    playFlipThrough.PlayOneShot(flip);
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

                if (hitObject.CompareTag("Customer") && !GameManager.Instance.currentCustomer.GetComponent<CustomerMovement>().isAtCounter)
                {
                    UnhighlightCurrentObject();
                    return;
                }

                if (hitObject != highlightedObject)
                {
                    UnhighlightCurrentObject();
                    HighlightNewObject(hitObject);
                }

                if (hitObject.TryGetComponent<IInteractable>(out IInteractable interactable))
                {
                    GameManager.Instance.TogglePopup(false, true);
                }

                if (hitObject.TryGetComponent<IHighlightable>(out IHighlightable highlightable))
                {
                    GameManager.Instance.TogglePopup(true, true);
                }

                if (hitObject.CompareTag("Door") && GameManager.Instance.hasTalkedWithBoss)
                {
                    GameManager.Instance.TogglePopup(true, true);
                }
            }
            else
            {
                UnhighlightCurrentObject();
            }
        }
    }

    private void UnhighlightCurrentObject()
    {
        if (highlightedObject != null)
        {
            if (highlightedObject.CompareTag("Door"))
            {
                GameManager.Instance.TogglePopup(true, false);
                highlightedObject = null;
            }
            else
            {
                GameManager.Instance.TogglePopup(true, false);
                GameManager.Instance.TogglePopup(false, false);
                highlightedObject.GetComponent<IHighlightable>().OnHighlight(false);
                highlightedObject = null;
            }
        }
    }

    private void HighlightNewObject(GameObject hitObject)
    {
        if (hitObject.TryGetComponent<IHighlightable>(out IHighlightable highlightable))
        {
            highlightable.OnHighlight(true);
            highlightedObject = hitObject;
        }

        if (!hitObject.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            GameManager.Instance.TogglePopup(false, false);
        }

        if (hitObject.CompareTag("Door"))
        {
            highlightedObject = hitObject;
        }
    }

    private void MoveObjectToHoldPos()
    {
        /*
        Vector3 direction = (objectHoldPos.position - pickUpObject.transform.position).normalized;
        Vector3 targetVelocity = Vector3.ClampMagnitude(direction * objectMoveSpeed, maxObjectMoveSpeed);

        if (Vector3.Distance(pickUpObject.transform.position, objectHoldPos.position) > 0.1f)
        {
            pickUpRb.linearVelocity = Vector3.SmoothDamp(pickUpRb.linearVelocity, targetVelocity, ref velocitySmoothDamp, smoothTime);

        }
        else
        {
            pickUpRb.linearVelocity = Vector3.zero;
        }
        */
        

        pickUpObject.transform.position = objectHoldPos.position;

        /*
        Vector3 targetPosition = pickUpObject.transform.position;
        Vector3 moveDirection = (targetPosition - pickUpObject.transform.position);
        pickUpRb.linearVelocity = moveDirection * objectMoveSpeed;
        */
    }

    private void DropObject()
    {
        if (pickUpObject != null)
        {
            isHoldingObject = false;
            pickUpRb.useGravity = true;
            pickUpRb.freezeRotation = false;
            pickUpRb.isKinematic = false;
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
