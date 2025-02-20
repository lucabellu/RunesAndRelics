using UnityEngine;

public class Trinket : MonoBehaviour, IHighlightable
{
    //objectives
    //
    //can be interacted with by player
    //can be given to customer

    public bool inCustomerRange { get; private set; } = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            inCustomerRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            inCustomerRange = false;
        }
    }
}
