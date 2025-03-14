using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : Document
{
    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI recipientText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private Image sealDisplay;

    [SerializeField] private bool isCustomSet = false;

    [Header("Custom Data")]
    [SerializeField] private string customSender;
    [SerializeField] private string customRecipient;
    [SerializeField] private string customContent;
    [SerializeField] private string customTitle;
    [SerializeField] private Sprite customSeal;

    protected override void Start()
    {
        base.Start();

        if (!isCustomSet)
        {
            senderText.text = customerLogic.letterSender;
            recipientText.text = customerLogic.letterRecipient;
            contentText.text = customerLogic.letterContent;
            titleText.text = customerLogic.letterTitle;

            sealDisplay.sprite = customerLogic.letterSeal;
        }
        else
        {
            senderText.text = customSender;
            recipientText.text = customRecipient;
            contentText.text = customContent;
            titleText.text = customTitle;
            sealDisplay.sprite = customSeal;
        }
    }
}
