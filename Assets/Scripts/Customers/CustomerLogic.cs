using System.Collections;
using UnityEngine;

public class CustomerLogic : MonoBehaviour
{
    //objectives
    //
    //show dialogue on entry, then hide
    //show dialogue if player enters collision
    //

    private CustomerMovement customerMovement;
    [SerializeField] private Canvas canvas;

    private void Start()
    {
        customerMovement = GetComponent<CustomerMovement>();
        canvas.gameObject.SetActive(false);
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
}
