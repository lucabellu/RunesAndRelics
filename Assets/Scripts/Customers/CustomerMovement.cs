using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CustomerMovement : MonoBehaviour
{
    //objectives
    //
    //on spawn in from manager, move to counter
    // on sale finish, move to exit

    private Transform target;
    private Vector3 spawnPos;
    [SerializeField] private float moveSpeed;

    private CustomerDialogue customerDialogue;
    private CustomerLogic customerLogic;

    public bool showDialogue = false;
    public bool firstDialogue { get; private set; } = true;

    private bool hasMadeSale = false;

    private Quaternion targetRotation;
    private bool isRotating = true;

    private void Start()
    {
        customerDialogue = GetComponent<CustomerDialogue>();
        customerLogic = GetComponent<CustomerLogic>();

        spawnPos = GameManager.Instance.customerSpawn.position;
        target = GameManager.Instance.target;

        targetRotation = transform.rotation * Quaternion.Euler(0, 180, 0);
    }

    private void Update()
    {

        if (!hasMadeSale)
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(target.position.x, currentPosition.y, target.position.z);

            // Move the object smoothly towards the target on the XZ plane
            transform.position = Vector3.Lerp(currentPosition, targetPosition, moveSpeed * Time.deltaTime);

            if (Vector3.Distance(currentPosition, targetPosition) < 0.01f)
            {
                if (firstDialogue)
                {
                    showDialogue = true;
                    firstDialogue = false;
                    customerDialogue.StartCoroutine(customerDialogue.HideDialogue(4f));
                }

                if (customerLogic.hasDocuments)
                {
                    customerLogic.SpawnDocuments();
                    customerLogic.hasDocuments = false;
                }
            }
        }
        else
        {
            StartCoroutine(customerDialogue.HideDialogue(0f));

            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(spawnPos.x, currentPosition.y, spawnPos.z);

            // Move the object smoothly towards the target on the XZ plane
            transform.position = Vector3.Lerp(currentPosition, targetPosition, moveSpeed * 0.25f * Time.deltaTime);

            if (isRotating)
            {
                // Smoothly interpolate towards the target rotation
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 2f);

                // Stop rotating when close to the target
                if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
                {
                    transform.rotation = targetRotation; // Snap to the target rotation
                    isRotating = false;
                }
            }
            Destroy(gameObject, 5f);
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSale.AddListener(CustomerExit);
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnSale.RemoveListener(CustomerExit);
        }
    }

    public void CustomerExit()
    {
        hasMadeSale = true;
    }
}
