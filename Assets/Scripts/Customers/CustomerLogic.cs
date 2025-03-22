using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomerLogic : MonoBehaviour, IHighlightable
{
    //objectives
    //
    //show dialogue on entry, then hide
    //show dialogue if player enters collision
    //

    public List<Document> documents;
    public List<GameObject> activeDocuments;
    public bool hasDocuments;

    public string customerName;
    public int customerAge;

    public Race customerRace;
    public Kingdom customerKingdom;
    public Occupation customerOccupation;
    public Guild customerGuild;
    public GuildRank customerGuildRank;
    public Sprite customerPortrait;

    public Trinket purchaseTrinket;

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
                activeDocuments.Add(document.gameObject);
            }
        }
    }

    public void OnHighlight(bool isHovering)
    {
        if (isHovering)
        {
            GetComponent<Outline>().enabled = true;
            GameManager.Instance.TogglePopup(true, true);

            GameManager.Instance.crosshair.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 45);
            GameManager.Instance.crosshair.GetComponent<Image>().color = Color.red;
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            GameManager.Instance.TogglePopup(true, false);

            GameManager.Instance.crosshair.GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 0);
            GameManager.Instance.crosshair.GetComponent<Image>().color = Color.white;
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
    ASSASSINS,
    MERCHANTS,
    MAGES,
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
