using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CustomerMovement : MonoBehaviour
{
    //objectives
    //
    //on spawn in from manager, move to counter
    // on sale finish, move to exit

    [SerializeField] private Transform target;
    private Vector3 spawnPos;
    [SerializeField] private float moveSpeed;

    private CustomerLogic customerLogic;

    public bool showDialogue { get; private set; } = false;
    private bool firstDialogue = true;

    private bool hasMadeSale = false;

    private void Start()
    {
        customerLogic = GetComponent<CustomerLogic>();
        spawnPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            hasMadeSale = true;
        }

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
                    customerLogic.ShowDialogue();
                    StartCoroutine(customerLogic.HideDialogue(4f));
                    firstDialogue = false;
                }
            }
        }
        else
        {
            Vector3 currentPosition = transform.position;
            Vector3 targetPosition = new Vector3(spawnPos.x, currentPosition.y, spawnPos.z);

            // Move the object smoothly towards the target on the XZ plane
            transform.position = Vector3.Lerp(currentPosition, targetPosition, moveSpeed * 0.25f * Time.deltaTime);
            Destroy(gameObject, 5f);
        }
    }


}
