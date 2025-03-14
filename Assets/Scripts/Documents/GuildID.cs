using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildID : Document
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guildText;
    [SerializeField] private TextMeshProUGUI guildRankText;
    [SerializeField] private TextMeshProUGUI kingdomText;

    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private Image sealDisplay;

    [SerializeField] private bool isCustomSet = false;

    [Header("Custom Data")]
    [SerializeField] private string customName;
    [SerializeField] private Guild customGuild;
    [SerializeField] private GuildRank customGuildRank;
    [SerializeField] private Kingdom customKingdom;
    [SerializeField] private Sprite customPortrait;
    [SerializeField] private Sprite customSeal;

    protected override void Start()
    {
        base.Start();

        if (!isCustomSet)
        {
            nameText.text = "Name: " + customerLogic.customerName;
            guildText.text = "Guild: " + customerLogic.customerGuild.ToString();
            guildRankText.text = "Rank: " + customerLogic.customerGuildRank.ToString();
            kingdomText.text = "Kingdom: " + customerLogic.customerKingdom.ToString();

            customerImageDisplay.sprite = customerLogic.customerPortrait;
            sealDisplay.sprite = customerLogic.letterSeal;
        }
        else
        {
            nameText.text = "Name: " + customName;
            guildText.text = "Guild: " + customGuild.ToString();
            guildRankText.text = "Rank: " + customGuildRank.ToString();
            kingdomText.text = "Kingdom: " + customKingdom.ToString();
            customerImageDisplay.sprite = customPortrait;
            sealDisplay.sprite = customSeal;
        }
    }
}
