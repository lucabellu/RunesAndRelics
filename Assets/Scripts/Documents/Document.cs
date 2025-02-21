using UnityEngine;
// I want to create a c# script that stores the data for different documents but the document type can be chosen via an enum and the required data for each document depends on the enum type

public class Document : MonoBehaviour, IInteractable, IHighlightable
{
    //objectives
    //
    //create a parent class for document types to derive from
    //base class requires a canvas reference

    private Canvas documentCanvas;
    private void Start()
    {
        documentCanvas = GetComponentInChildren<Canvas>();
    }

    public void OnInteract(bool isInteracting)
    {
        if (isInteracting)
        {
            documentCanvas.enabled = true;
        }
        else
        {
            documentCanvas.enabled = false;
        }
    }

    public void OnHighlight(bool isHovering)
    {
        if (isHovering)
        {
            GetComponent<Outline>().enabled = true;
        }
        else
        {
            GetComponent<Outline>().enabled = false;
        }
    }
}
