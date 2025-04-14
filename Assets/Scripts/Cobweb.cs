using UnityEngine;

public class Cobweb : MonoBehaviour, IInteractable
{
    public void OnInteract(bool on)
    {
        GameManager.Instance.TogglePopup(GameManager.PopupSide.LEFT, false);
        GameManager.Instance.cobwebs.Remove(this.gameObject);
        Destroy(this.gameObject);
        //play animation
    }

}
