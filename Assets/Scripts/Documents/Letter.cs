using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : Document
{
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI titleText;

    protected override void Start()
    {
        base.Start();

        senderText.text = "To " + customerLogic.letterSender;
        contentText.text = customerLogic.letterContent;
        titleText.text = customerLogic.letterTitle;
    }
}
