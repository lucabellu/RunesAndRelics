using UnityEngine;

public class Cobweb : MonoBehaviour, IInteractable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnInteract(bool on)
    {
        GameManager.Instance.cobwebs.Remove(this.gameObject);
        Destroy(this.gameObject);
        //play animation
        //destroy cobweb
    }
}
