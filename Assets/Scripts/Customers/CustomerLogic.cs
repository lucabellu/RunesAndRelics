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

    [SerializeField] private List<Document> documents;
    public bool hasDocuments;

    public string customerName;
    public int customerAge;

    public Race customerRace;
    public Kingdom customerKingdom;
    public Occupation customerOccupation;
    public Guild customerGuild;
    public GuildRank customerGuildRank;
    public Sprite customerPortrait;

    [Header("Letter")]
    public string letterSender;
    public string letterRecipient;
    public string letterContent;
    public string letterTitle;
    public Sprite letterSeal;





    public void SpawnDocuments()
    {
        if (documents.Count > 0)
        {
            foreach (Document doc in documents)
            {
                GameObject spawnedDocument = Instantiate(doc.gameObject, GameManager.Instance.documentSpawnPoints[documents.IndexOf(doc)].position, Quaternion.identity);
                Document document = spawnedDocument.GetComponent<Document>();
                document.customerLogic = this;
            }
        }
    }
}

public enum Race
{
    HUMAN,
    ELF,
    ORC,
    DWARF,
    DEMON,
    FAIRY,
    UNDEAD,
    NONE
}

public enum Kingdom
{
    NORTH,
    EAST,
    SOUTH,
    WEST,
    NONE
}

public enum Occupation
{
    BARD,
    WARRIOR,
    CLERIC,
    DRUID,
    RANGER,
    WIZARD,
    NONE
}

public enum Guild
{
    HOLY,
    ASSASSIN,
    MERCHANT,
    MAGE,
    NATURE,
    NONE
}

public enum GuildRank
{
    NOVICE,
    APPRENTICE,
    JOURNEYMAN,
    VICEMASTER,
    MASTER,
    NONE
}
