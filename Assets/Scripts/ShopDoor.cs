using UnityEngine;
using static GameManager;

public class ShopDoor : MonoBehaviour
{
    public bool canInteract = false;

    [SerializeField] private AudioSource audioSource;

    public void HighlightDoor()
    {
        GetComponent<Outline>().enabled = true;
        canInteract = true;
    }

    public void UnhighlightDoor()
    {
        GetComponent<Outline>().enabled = false;
        GameManager.Instance.TogglePopup(PopupSide.LEFT, false);
    }

    public void PlayFailSound()
    {
        audioSource.Play();
    }
}
