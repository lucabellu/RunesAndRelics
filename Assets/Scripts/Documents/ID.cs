using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ID : Document
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI raceText;
    [SerializeField] private TextMeshProUGUI occupationText;

    [SerializeField] private Image customerImageDisplay;
    [SerializeField] private Image sealDisplay;

    [SerializeField] private bool isCustomSet = false;

    [Header("Custom Data")]
    [SerializeField] private string customName;
    [SerializeField] private int customAge;
    [SerializeField] private string customRaceText;
    [SerializeField] private Occupation customOccupation;
    [SerializeField] private Sprite customPortrait;
    [SerializeField] private Sprite customSeal;

    protected override void Start()
    {
        base.Start();

        if (!isCustomSet)
        {
            nameText.text = "Name: " + customerLogic.customerName;
            ageText.text = "Age: " + customerLogic.customerAge.ToString();
            raceText.text = "Race: " + customerLogic.customerRace.ToString();
            occupationText.text = "Occupation: " + customerLogic.customerOccupation.ToString();

            customerImageDisplay.sprite = customerLogic.customerPortrait;
            sealDisplay.sprite = customerLogic.letterSeal;
        }
        else
        {
            nameText.text = "Name: " + customName;
            ageText.text = "Guild: " + customAge.ToString();
            raceText.text = "Race: " + customRaceText;
            occupationText.text = "Occupation: " + customOccupation.ToString();

            customerImageDisplay.sprite = customPortrait;
            sealDisplay.sprite = customSeal;
        }
    }
}
