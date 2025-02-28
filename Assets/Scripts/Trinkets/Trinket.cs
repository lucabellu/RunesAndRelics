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
    public int requiredAge;
    public Guild requiredGuild;
    public GuildRank requiredGuildRank;

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
            GameManager.Instance.TogglePopup(true, true);
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            GameManager.Instance.TogglePopup(true, false);
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

            if (requiredAge > 0) // Assuming 0 means no age requirement
                flags |= RequirementFlags.Age;

            if (requiredGuild != Guild.NONE) // Assuming Guild.None is a default/unset value
                flags |= RequirementFlags.Guild;

            if (requiredGuildRank != GuildRank.NONE) // Assuming GuildRank.None is a default/unset value
                flags |= RequirementFlags.GuildRank;

            return flags;
        }
    }
}
