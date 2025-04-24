using System.Collections;
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

    private GameObject pickUpObject;
    private Rigidbody pickUpRb;
    #endregion

    #region Held Object
    [Header("Held object")]
    [SerializeField] private Transform objectHoldPos;
    [SerializeField] private float objectMoveSpeed;
    [SerializeField] private float maxObjectMoveSpeed;

    private bool isHoldingObject = false;
    #endregion

    private bool isPlayerInCustomerRange = false;
    private Document currentDocument;
    private bool isInteractingWithDocument = false;

    [SerializeField] private AudioSource playFlipThrough;
    [SerializeField] private AudioClip flip;

    [SerializeField] private AudioSource playPickUp;
    [SerializeField] private AudioClip pickUp;

    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeIntensity;

    [SerializeField] private ShopDoor shopDoor;

    private void Update()
    {
        HandleInteractions();
        HighlightObject();
        UpdateOutline();
    }

    private void FixedUpdate()
    {
        if (isHoldingObject)
        {
            MoveObjectToHoldPos();
        }
    }

    private void HandleInteractions()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (pickUpObject == null)
            {
                TryInteractWithObject();
            }
            else
            {
                HandlePickUpObject();
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            HandleDocumentInteraction();
        }
    }

    private void TryInteractWithObject()
    {
        if (isInteractingWithDocument) return;

        Transform cameraTransform = playerCam.transform;

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, interactDistance, layerMask))
        {
            switch (hit.transform.tag)
            {
                case "Scroll":
                    return;
                case "Door":
                    HandleDoorInteraction(hit.transform.GetComponent<ShopDoor>());
                    break;
                case "Cobweb":
                    if (GameManager.Instance.canCleanCobwebs) HandleCobwebInteraction(hit.transform);
                    break;
                case "Customer":
                    if (GameManager.Instance.currentCustomer.GetComponent<CustomerMovement>().isAtCounter)
                    {
                        HandleCustomerInteraction(GameManager.Instance.currentCustomer.purchaseTrinket);
                    }
                    break;
                default:
                    PickUpObject(hit.transform);
                    break;
            }
        }
    }

    private void HandleDoorInteraction(ShopDoor door)
    {
        if (door != null && door.canInteract && !GameManager.Instance.bossDoor.transform.GetComponent<Dialogue>().firstBossDialogue)
        {
            GameManager.Instance.EndDay();
        }
    }

    private void HandleCobwebInteraction(Transform cobweb)
    {
        print("Interacting with cobweb");

        if (GameManager.Instance.canCleanCobwebs && cobweb.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            interactable.OnInteract(true);
        }
    }

    private void PickUpObject(Transform obj)
    {
        isHoldingObject = true;
        pickUpObject = obj.gameObject;
        pickUpRb = pickUpObject.GetComponent<Rigidbody>();
        pickUpRb.useGravity = false;
        pickUpRb.freezeRotation = true;
        pickUpRb.isKinematic = true;
        pickUpObject.transform.SetParent(objectHoldPos);
        playPickUp.PlayOneShot(pickUp);
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

    private void HandleDocumentInteraction()
    {
        if (isHoldingObject) return;

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

    private void TryInteractWithDocument()
    {
        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, interactDistance, layerMask) && !hit.transform.CompareTag("Cobweb"))
        {
            if (hit.transform.TryGetComponent<IInteractable>(out IInteractable interactable))
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
        if (isHoldingObject || Time.timeScale == 0) return;

        if (Physics.Raycast(playerCam.transform.position, playerCam.transform.forward, out RaycastHit hit, interactDistance, layerMask))
        {
            GameObject hitObject = hit.transform.gameObject;

            if (hitObject.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                if (hitObject.CompareTag("Cobweb") && GameManager.Instance.canCleanCobwebs)
                {
                    GameManager.Instance.TogglePopup(PopupSide.LEFT, true);
                }
                else if (!hitObject.CompareTag("Cobweb"))
                {
                    GameManager.Instance.TogglePopup(PopupSide.RIGHT, true);
                }

            }

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
        }
        else
        {
            UnhighlightCurrentObject();
            GameManager.Instance.TogglePopup(PopupSide.RIGHT, false);
            GameManager.Instance.TogglePopup(PopupSide.LEFT, false);
        }
    }

    private void UnhighlightCurrentObject()
    {
        if (highlightedObject != null)
        {
            if (highlightedObject.TryGetComponent<IHighlightable>(out IHighlightable highlightable))
            {
                highlightable.OnHighlight(false);
                GameManager.Instance.TogglePopup(PopupSide.RIGHT, false);
                GameManager.Instance.TogglePopup(PopupSide.LEFT, false);
            }

            GameManager.Instance.TogglePopup(PopupSide.RIGHT, false);
            GameManager.Instance.TogglePopup(PopupSide.LEFT, false);
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

    private void UpdateOutline()
    {
        if (pickUpObject != null && pickUpObject.TryGetComponent<Trinket>(out Trinket trinket) && trinket.inCustomerRange && isPlayerInCustomerRange)
        {
            GameManager.Instance.currentCustomer.GetComponent<Outline>().enabled = true;
        }
        else if (pickUpObject != null && !isPlayerInCustomerRange && GameManager.Instance.currentCustomer != null)
        {
            GameManager.Instance.currentCustomer.GetComponent<Outline>().enabled = false;
        }
    }

    private void MoveObjectToHoldPos()
    {
        pickUpObject.transform.position = objectHoldPos.position;
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
            print("handled successfully");
        }
        else
        {
            HandleFailedTransaction();
        }

        if (pickUpObject != null)
        {
            Destroy(pickUpObject);
            pickUpObject = null;
            isHoldingObject = false;
        }

        if (currentCustomer.hasDocuments)
        {
            foreach (GameObject document in currentCustomer.activeDocuments)
            {
                Destroy(document);
            }
        }

        currentCustomer.transform.GetComponent<CustomerMovement>().CustomerExit();
        StartCoroutine(StartNextTaskWithDelay(6f));
    }

    private void HandleFailedTransaction()
    {
        Debug.Log("Transaction failed");
        shopDoor.PlayFailSound();
        cameraShake.InduceStress(shakeIntensity);
        GameManager.Instance.mistakesMade++;
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

    private IEnumerator StartNextTaskWithDelay(float delay)
    {
        GameManager.Instance.currentTasks[GameManager.Instance.currentTaskIndex].CompleteTask();
        yield return new WaitForSeconds(delay);
        GameManager.Instance.StartNextTask();
    }

    public void CloseManifest()
    {
        currentDocument.OnInteract(false);
        GameManager.Instance.SetPlayerDocumentState(false);
        isInteractingWithDocument = false;
        currentDocument = null;
    }
}
