using System.Collections;
using UnityEngine;


public class CustomerMovement : MonoBehaviour
{
    private Transform target;
    private Vector3 spawnPos;
    [SerializeField] private float moveSpeed;
    private Vector3 movementDirection;

    private Dialogue customerDialogue;
    private CustomerLogic customerLogic;

    public bool showDialogue = false;
    public bool isAtCounter { get; private set; } = false;
    public bool firstDialogue { get; private set; } = true;

    private bool hasMadeSale = false;

    [SerializeField] private Animator animator;

    [SerializeField] private AudioClip footstepSound;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float footstepDelay;
    private Coroutine footstepCoroutine;

    private bool doOnce = true;

    private void Start()
    {
        customerDialogue = GetComponentInChildren<Dialogue>();
        customerLogic = GetComponent<CustomerLogic>();

        spawnPos = GameManager.Instance.customerSpawn.position;
        target = GameManager.Instance.target;

    }

    private void Update()
    {
        HandleCustomerAnimation();
        HandleFootsteps();

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
                }

                if (customerLogic.hasDocuments && doOnce)
                {
                    customerLogic.SpawnDocuments();
                    doOnce = false;
                }
            }
        }
        else
        {
            customerDialogue.HideDialogue(0f);

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

    private IEnumerator PlayFootstepSoundRandomPitch()
    {
        while (true)
        {
            yield return new WaitForSeconds(footstepDelay);
            audioSource.pitch = Random.Range(0.9f, 1.1f);
            audioSource.PlayOneShot(footstepSound);
        }
    }

    private void HandleFootsteps()
    {
        if (Vector3.Distance(transform.position, target.position) > 1f)
        {
            if (footstepCoroutine == null)
            {
                footstepCoroutine = StartCoroutine(PlayFootstepSoundRandomPitch());
            }
        }
        else
        {
            if (footstepCoroutine != null)
            {
                StopCoroutine(footstepCoroutine);
                footstepCoroutine = null;
            }
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
