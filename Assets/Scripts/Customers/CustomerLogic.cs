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

    private CustomerMovement customerMovement;
    [SerializeField] private CustomerDataSO customerData;


    public string customerName { get; private set; }
    public int customerAge { get; private set; }
    public Race customerRace { get; private set; }
    public Kingdom customerKingdom { get; private set; }
    public Occupation customerOccupation { get; private set; }


    private void Start()
    {
        customerMovement = GetComponent<CustomerMovement>();

        customerName = customerData.customerName;
        customerAge = customerData.customerAge;
        customerRace = customerData.customerRace;
        customerKingdom = customerData.kingdom;
        customerOccupation = customerData.occupation;
    }

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
}
