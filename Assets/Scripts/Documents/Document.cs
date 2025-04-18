using UnityEngine;
using System.Collections;
using static GameManager;

public class Document : MonoBehaviour, IInteractable, IHighlightable
{
    [SerializeField] private GameObject documentCanvas;
    public CustomerLogic customerLogic;

    private float duration = 1f;
    private bool hasDurationPassed = false;
    [SerializeField] private bool isTutorialDocument = false;

    protected virtual void Start()
    {
        documentCanvas.gameObject.SetActive(false);
        StartCoroutine(WaitForDuration());
    }

    public void OnInteract(bool isInteracting)
    {
        if (isInteracting)
        {
            documentCanvas.gameObject.SetActive(true);
            if (isTutorialDocument)
            {
                GameManager.Instance.hasReadTutorialDocument = true;
            }
        }
        else
        {
            documentCanvas.gameObject.SetActive(false);
        }
    }

    public void OnHighlight(bool isHovering)
    {
        if (isHovering && !transform.CompareTag("Scroll"))
        {
            GetComponent<Outline>().enabled = true;
            GameManager.Instance.TogglePopup(PopupSide.LEFT, true);
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            GameManager.Instance.TogglePopup(PopupSide.LEFT, false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasDurationPassed)
        {
            GameManager.Instance.PlayDrop();
        }
    }

    private IEnumerator WaitForDuration()
    {
        yield return new WaitForSeconds(duration);
        hasDurationPassed = true;
    }
}
