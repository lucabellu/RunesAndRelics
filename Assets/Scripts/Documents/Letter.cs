using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Letter : Document
{
    [SerializeField] private string letterSender;
    [SerializeField] private string letterRecipient;
    [SerializeField] private string letterContent;
    [SerializeField] private string letterTitle;

    [SerializeField] private Sprite seal;

    [SerializeField] private TextMeshProUGUI senderText;
    [SerializeField] private TextMeshProUGUI recipientText;
    [SerializeField] private TextMeshProUGUI contentText;
    [SerializeField] private TextMeshProUGUI titleText;

    [SerializeField] private Image sealDisplay;

    private void Start()
    {
        senderText.text = letterSender;
        recipientText.text = letterRecipient;
        contentText.text = letterContent;
        titleText.text = letterTitle;
        sealDisplay.sprite = seal;
    }
}
