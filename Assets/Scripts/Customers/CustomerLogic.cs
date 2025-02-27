using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using static GameManager;

public class CustomerLogic : MonoBehaviour
{
    //objectives
    //
    //show dialogue on entry, then hide
    //show dialogue if player enters collision
    //

    [SerializeField] private CustomerDataSO customerData;

    public int customerAge { get; private set; }
    public Race customerRace { get; private set; }
    public Kingdom customerKingdom { get; private set; }
    public Occupation customerOccupation { get; private set; }
    public Guild customerGuild { get; private set; }
    public GuildRank customerGuildRank { get; private set; }



    private void Start()
    {
        customerAge = customerData.customerAge;
        customerRace = customerData.customerRace;
        customerKingdom = customerData.kingdom;
        customerOccupation = customerData.occupation;
        customerGuild = customerData.guild;
        customerGuildRank = customerData.guildRank;
    }

    /*
    public RequirementFlags customerFields
    {
        get
        {
            RequirementFlags flags = RequirementFlags.None;

            if (customerRace != Race.NONE) // Assuming Race.None is a default/unset value
                flags |= RequirementFlags.Race;

            if (customerKingdom != Kingdom.NONE) // Assuming Kingdom.None is a default/unset value
                flags |= RequirementFlags.Kingdom;

            if (customerOccupation != Occupation.NONE) // Assuming Occupation.None is a default/unset value
                flags |= RequirementFlags.Occupation;

            if (customerAge > 0) // Assuming 0 means no age requirement
                flags |= RequirementFlags.Age;

            return flags;
        }
    }
    */
}
