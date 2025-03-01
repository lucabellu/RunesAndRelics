using UnityEngine;

public class ShopDoor : MonoBehaviour
{
    public void HighlightDoor()
    {
        GetComponent<Outline>().enabled = true;
    }

    public void UnhighlightDoor()
    {
        GetComponent<Outline>().enabled = false;
    }
}
