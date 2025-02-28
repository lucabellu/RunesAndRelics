using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildID : Document
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guildText;
    [SerializeField] private TextMeshProUGUI guildRankText;
    [SerializeField] private TextMeshProUGUI kingdomText;


    [SerializeField] private Sprite customerImage;
    [SerializeField] private Sprite seal;

    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private Image sealDisplay;

    [SerializeField] private bool isCustomSet = false;

    [Header("Custom Data")]
    [SerializeField] private string customName;
    [SerializeField] private GuildRank customGuildRank;
    [SerializeField] private Kingdom customKingdom;
    [SerializeField] private Sprite customPortrait;
    [SerializeField] private Sprite customSeal;

    private void Start()
    {
        if (!isCustomSet)
        {
            nameText.text = customerData.customerName;
            guildText.text = customerData.guild.ToString();
            guildRankText.text = customerData.guildRank.ToString();
            kingdomText.text = customerData.kingdom.ToString();

            customerImageDisplay.sprite = customerImage;
            sealDisplay.sprite = seal;
        }
        else
        {
            nameText.text = customName;
            guildRankText.text = customGuildRank.ToString();
            kingdomText.text = customKingdom.ToString();
            customerImageDisplay.sprite = customPortrait;
            sealDisplay.sprite = customSeal;
        }
    }
}
