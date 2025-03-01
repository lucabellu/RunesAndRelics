using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textSpeed;
    [SerializeField] private string dialogue;

    private CustomerMovement customerMovement;

    private bool isTalking = false;
    private Coroutine talkingCoroutine;

    [SerializeField] private bool isCustomer;

    private void Start()
    {
        if (isCustomer)
        {
            customerMovement = GetComponent<CustomerMovement>();
        }

        canvas.gameObject.SetActive(false);
        dialogueText.text = dialogue;
    }

    private void Update()
    {
        if (isCustomer)
        {
            if (customerMovement.showDialogue)
            {
                StartOrResetDialogue(dialogue);
                customerMovement.showDialogue = false;
            }
        }
    }

    public void StartOrResetDialogue(string newText)
    {
        // If a coroutine is already running, stop it
        if (isTalking)
        {
            if (talkingCoroutine != null)
            {
                StopCoroutine(talkingCoroutine);
            }

            isTalking = false;
        }

        // Clear the existing text
        dialogueText.text = "";

        // Set the canvas to active
        canvas.gameObject.SetActive(true);

        // Start the new coroutine
        talkingCoroutine = StartCoroutine(TypeLine(newText));

        if (!isCustomer)
        {
            GameManager.Instance.hasTalkedWithBoss = true;
        }
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

        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
        }

        canvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCustomer)
        {
            if (other.CompareTag("Player") && !customerMovement.firstDialogue)
            {
                StartOrResetDialogue(dialogue);
            }
        }

        if (other.CompareTag("Player") && GameManager.Instance.canTalkWithBoss)
        {
            StartOrResetDialogue(dialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && GameManager.Instance.canTalkWithBoss)
        {
            StartCoroutine(HideDialogue(0f));
        }
    }
}
