using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildID : Document
{

    [SerializeField] private Sprite customerImage;
    [SerializeField] private Sprite seal;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guildRankText;
    [SerializeField] private TextMeshProUGUI kingdomText;

    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private Image sealDisplay;

    private CustomerDataSO customerData;

    private void Start()
    {
        nameText.text = customerData.customerName;
        guildRankText.text = customerData.guildRank.ToString();
        kingdomText.text = customerData.kingdom.ToString();

        customerImageDisplay.sprite = customerImage;
        sealDisplay.sprite = seal;
    }
}
