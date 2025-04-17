using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ID : Document
{

    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI ageText;
    [SerializeField] private TextMeshProUGUI raceOccupationText;

    [SerializeField] private Image customerPortrait;

    protected override void Start()
    {
        base.Start();

        nameText.text = customerLogic.customerName;
        ageText.text = customerLogic.customerAge.ToString();
        raceOccupationText.text = customerLogic.customerRace.ToString() + ", " + customerLogic.customerOccupation.ToString(); ;
        customerPortrait.sprite = customerLogic.customerPortrait;
    }
}
