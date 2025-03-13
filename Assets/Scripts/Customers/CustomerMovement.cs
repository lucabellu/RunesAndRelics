using TMPro;
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
    private Vector3 movementDirection;

    private CustomerDialogue customerDialogue;
    private CustomerLogic customerLogic;

    public bool showDialogue = false;
    public bool isAtCounter { get; private set; } = false;
    public bool firstDialogue { get; private set; } = true;

    private bool hasMadeSale = false;

    [SerializeField] private Animator animator;

    private void Start()
    {
        customerDialogue = GetComponentInChildren<CustomerDialogue>();
        customerLogic = GetComponent<CustomerLogic>();

        spawnPos = GameManager.Instance.customerSpawn.position;
        target = GameManager.Instance.target;

    }

    private void Update()
    {
        HandleCustomerAnimation();

        // Determine target position based on sale state
        Vector3 targetPosition;

        if (!hasMadeSale)
        {
            targetPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
        }
        else
        {
            targetPosition = new Vector3(spawnPos.x, transform.position.y, spawnPos.z);
        }

        // Update movement direction
        movementDirection = (targetPosition - transform.position).normalized;

        // Move the NPC toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Handle logic when the NPC reaches the counter
        if (!hasMadeSale)
        {
            if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
            {
                isAtCounter = true;

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

            isAtCounter = false;

            Destroy(gameObject, 5f); // Delayed destroy
        }

        // Rotate toward movement direction
        if (!isAtCounter)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(movementDirection), 0.1f);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
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

    private void HandleCustomerAnimation()
    {
        if (isAtCounter)
        {
            animator.SetBool("isIdle", false);
        }
        else
        {
            animator.SetBool("isIdle", true);
        }
    }

    public void CustomerExit()
    {
        hasMadeSale = true;
    }
}
