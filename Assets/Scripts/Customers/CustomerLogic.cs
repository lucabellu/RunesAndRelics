using System.Collections;
using TMPro;
using UnityEngine;
using static GameManager;

public class CustomerLogic : MonoBehaviour
{
    //objectives
    //
    //show dialogue on entry, then hide
    //show dialogue if player enters collision
    //

    private CustomerMovement customerMovement;

    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textSpeed;
    [SerializeField] private CustomerDataSO customerData;

    private bool isTalking = false;

    public string customerName { get; private set; }
    public int customerAge { get; private set; }
    public Race customerRace { get; private set; }
    public Kingdom customerKingdom { get; private set; }
    public Occupation customerOccupation { get; private set; }


    private void Start()
    {
        customerMovement = GetComponent<CustomerMovement>();
        canvas.gameObject.SetActive(false);
        dialogueText.text = customerData.customerDialogue;

        customerName = customerData.customerName;
        customerAge = customerData.customerAge;
        customerRace = customerData.customerRace;
        customerKingdom = customerData.kingdom;
        customerOccupation = customerData.occupation;
    }

    private void Update()
    {
        if (customerMovement.showDialogue)
        {
            ShowDialogue();
        }
    }

    public void ShowDialogue()
    {
        if (isTalking) return;
        canvas.gameObject.SetActive(true);
        StartCoroutine(TypeLine(customerData.customerDialogue));
    }

    private IEnumerator TypeLine(string line)
    {
        isTalking = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTalking = false;
    }

    public IEnumerator HideDialogue(float timeToHide)
    {
        yield return new WaitForSeconds(timeToHide);
        dialogueText.text = "";
        StopCoroutine(TypeLine(customerData.customerDialogue));
        canvas.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ShowDialogue();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HideDialogue(0f));
        }
    }

    public RequirementFlags customerFields
    {
        get
        {
            RequirementFlags flags = RequirementFlags.None;

            if (customerRace != Race.NONE) // Assuming Race.None is a default/unset value
                flags |= RequirementFlags.Race;

            if (customerKingdom != Kingdom.NONE) // Assuming Kingdom.None is a default/unset value
                flags |= RequirementFlags.Kingdom;

            if (customerOccupation != Occupation.NONE) // Assuming Occupation.None is a default/unset value
                flags |= RequirementFlags.Occupation;

            if (customerAge > 0) // Assuming 0 means no age requirement
                flags |= RequirementFlags.Age;

            return flags;
        }
    }
}
