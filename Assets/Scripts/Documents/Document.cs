using UnityEngine;
// I want to create a c# script that stores the data for different documents and create varients as a child class

public class Document : MonoBehaviour, IInteractable, IHighlightable
{
    private GameObject documentCanvas;

    private void Start()
    {
        documentCanvas = transform.GetChild(0).gameObject;
        documentCanvas.gameObject.SetActive(false);
    }

    public void OnInteract(bool isInteracting)
    {
        if (isInteracting)
        {
            documentCanvas.gameObject.SetActive(true);
        }
        else
        {
            documentCanvas.gameObject.SetActive(false);
        }
    }

    public void OnHighlight(bool isHovering)
    {
        if (isHovering)
        {
            GetComponent<Outline>().enabled = true;
            GameManager.Instance.TogglePopup(true, true);
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            GameManager.Instance.TogglePopup(true, false);
        }
    }
}
