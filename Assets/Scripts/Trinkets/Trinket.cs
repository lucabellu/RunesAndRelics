using UnityEngine;

public class Trinket : MonoBehaviour, IHighlightable
{
    //objectives
    //
    //can be interacted with by player
    //can be given to customer

    public Race requiredRace;
    public Kingdom requiredKingdom;
    public Occupation requiredOccupation;
    public int requiredLevel;

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

    public void MakeSale()
    {

    }
}
