using System;
using UnityEngine;
using static GameManager;

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

    public RequirementFlags ActiveRequirements
    {
        get
        {
            RequirementFlags flags = RequirementFlags.None;

            if (requiredRace != Race.NONE) // Assuming Race.None is a default/unset value
                flags |= RequirementFlags.Race;

            if (requiredKingdom != Kingdom.NONE) // Assuming Kingdom.None is a default/unset value
                flags |= RequirementFlags.Kingdom;

            if (requiredOccupation != Occupation.NONE) // Assuming Occupation.None is a default/unset value
                flags |= RequirementFlags.Occupation;

            if (requiredLevel > 0) // Assuming 0 means no level requirement
                flags |= RequirementFlags.Level;

            return flags;
        }
    }
}
