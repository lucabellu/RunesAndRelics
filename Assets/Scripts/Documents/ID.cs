using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ID : Document
{
    [SerializeField] private string customerName;
    [SerializeField] private int level;
    [SerializeField] private Race race;
    [SerializeField] Kingdom kingdom;
    [SerializeField] Occupation occupation;

    [SerializeField] private Sprite customerImage;
    [SerializeField] private Sprite seal;

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private TextMeshProUGUI raceText;
    [SerializeField] private TextMeshProUGUI kingdomText;
    [SerializeField] private TextMeshProUGUI occupationText;

    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private Image sealDisplay;

    private void Start()
    {
        nameText.text = customerName;
        levelText.text = level.ToString();
        raceText.text = race.ToString();
        kingdomText.text = kingdom.ToString();
        occupationText.text = occupation.ToString();

        customerImageDisplay.sprite = customerImage;
        sealDisplay.sprite = seal;
    }
}
