using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textSpeed;

    private CustomerMovement customerMovement;
    private CustomerLogic customerLogic;

    private bool isTalking = false;
    private Coroutine talkingCoroutine;

    private void Start()
    {
        customerMovement = GetComponent<CustomerMovement>();
        customerLogic = GetComponent<CustomerLogic>();

        canvas.gameObject.SetActive(false);
        dialogueText.text = customerLogic.customerDialogue;
    }

    private void Update()
    {
        if (customerMovement.showDialogue)
        {
            StartOrResetDialogue(customerLogic.customerDialogue);
            customerMovement.showDialogue = false;
        }
    }

    public void StartOrResetDialogue(string newText)
    {
        // If a coroutine is already running, stop it
        if (isTalking)
        {
            StopCoroutine(talkingCoroutine);
            isTalking = false;
        }

        // Clear the existing text
        dialogueText.text = "";

        // Set the canvas to active
        canvas.gameObject.SetActive(true);

        // Start the new coroutine
        talkingCoroutine = StartCoroutine(TypeLine(newText));
    }

    private IEnumerator TypeLine(string line)
    {
        isTalking = true;
        dialogueText.text = "";
        StringBuilder sb = new StringBuilder();
        foreach (char c in line)
        {
            sb.Append(c);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(textSpeed);
        }
        isTalking = false;
    }

    public IEnumerator HideDialogue(float timeToHide)
    {
        yield return new WaitForSeconds(timeToHide);
        dialogueText.text = "";
        StopCoroutine(talkingCoroutine);
        canvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !customerMovement.firstDialogue)
        {
            StartOrResetDialogue(customerLogic.customerDialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HideDialogue(0f));
        }
    }
}
