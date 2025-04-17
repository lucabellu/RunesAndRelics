using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuildID : Document
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI guildText;
    [SerializeField] private TextMeshProUGUI guildRankText;
    [SerializeField] private TextMeshProUGUI kingdomText;

    [SerializeField] private Image customerPortrait;
    [SerializeField] private Image guildIcon;
    [SerializeField] private Image background;


    [SerializeField] private List<GameObject> cardinalDirections;
    [SerializeField] private List<Sprite> guildIcons;
    [SerializeField] private List<GameObject> backgroundImages;
    [SerializeField] private List<Color> IdColour;

    protected override void Start()
    {
        base.Start();

        nameText.text = customerLogic.customerName;
        guildText.text = customerLogic.customerGuild.ToString() + " GUILD";
        guildRankText.text = customerLogic.customerGuildRank.ToString();
        kingdomText.text = customerLogic.customerKingdom.ToString();

        customerPortrait.sprite = customerLogic.customerPortrait;

        switch (customerLogic.customerGuild)
        {
            case Guild.HOLY:
                SetGuildData(0);
                break;
            case Guild.ASSASSINS:
                SetGuildData(1);
                break;
            case Guild.MERCHANTS:
                SetGuildData(2);
                break;
            case Guild.MAGES:
                SetGuildData(3);
                break;
            case Guild.DRUIDS:
                SetGuildData(4);
                break;
        }
    }

    private void SetGuildData(int guildType)
    {
        guildIcon.sprite = guildIcons[guildType];
        backgroundImages[guildType].GetComponent<Image>().color = IdColour[0] + new Color(0, 0, 0, 0.5f);
        background.color = IdColour[guildType];

        SetBackgroundImage(guildType);
        SetKingdom();
    }

    private void SetBackgroundImage(int backgroundType)
    {
        backgroundImages[0].SetActive(false);
        backgroundImages[1].SetActive(false);
        backgroundImages[2].SetActive(false);
        backgroundImages[3].SetActive(false);
        backgroundImages[4].SetActive(false);

        backgroundImages[backgroundType].SetActive(true);
    }

    private void SetKingdom()
    {
        switch (customerLogic.customerKingdom)
        {
            case Kingdom.NORTH:
                SetKingdomState(0);
                break;
            case Kingdom.EAST:
                SetKingdomState(1);
                break;
            case Kingdom.SOUTH:
                SetKingdomState(2);
                break;
            case Kingdom.WEST:
                SetKingdomState(3);
                break;
        }
    }

    private void SetKingdomState(int kingdomType)
    {
        cardinalDirections[0].SetActive(false);
        cardinalDirections[1].SetActive(false);
        cardinalDirections[2].SetActive(false);
        cardinalDirections[3].SetActive(false);

        cardinalDirections[kingdomType].SetActive(true);
    }


}
