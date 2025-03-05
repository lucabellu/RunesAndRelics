using UnityEngine;

public class ShopDoor : MonoBehaviour
{
    public bool canInteract = false;

    public void HighlightDoor()
    {
        GetComponent<Outline>().enabled = true;
    }

    public void UnhighlightDoor()
    {
        GetComponent<Outline>().enabled = false;
    }
}
