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

    [SerializeField] private CustomerDataSO customerData;

    public string customerName { get; private set; }
    public int customerLevel { get; private set; }
    public Race customerRace { get; private set; }
    public Kingdom customerKingdom { get; private set; }
    public Occupation customerOccupation { get; private set; }


    private void Start()
    {
        customerMovement = GetComponent<CustomerMovement>();
        canvas.gameObject.SetActive(false);
        dialogueText.text = customerData.customerDialogue;

        customerName = customerData.customerName;
        customerLevel = customerData.customerLevel;
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
        canvas.gameObject.SetActive(true);
    }

    public IEnumerator HideDialogue(float timeToHide)
    {
        yield return new WaitForSeconds(timeToHide);
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
            StartCoroutine(HideDialogue(0.5f));
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

            if (customerLevel > 0) // Assuming 0 means no level requirement
                flags |= RequirementFlags.Level;

            return flags;
        }
    }
}
