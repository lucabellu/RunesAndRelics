using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<Document> documents;
    public bool hasDocuments;

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

    public void SpawnDocuments()
    {
        if (documents.Count > 0)
        {
            foreach (Document doc in documents)
            {
                GameObject spawnedDocument = Instantiate(doc.gameObject, GameManager.Instance.documentSpawnPoints[documents.IndexOf(doc)].position, Quaternion.identity);
                Document document = spawnedDocument.GetComponent<Document>();
                document.customerData = customerData;
            }
        }
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
