using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static GameManager;
using System.Collections;

public class Manifest : Document
{
    [SerializeField] private TextMeshProUGUI trinketNameLeft;
    [SerializeField] private TextMeshProUGUI trinketDescriptionLeft;
    [SerializeField] private Image trinketImageLeft;
    [SerializeField] private TextMeshProUGUI trinketRequirementsLeft;

    [SerializeField] private TextMeshProUGUI trinketNameRight;
    [SerializeField] private TextMeshProUGUI trinketDescriptionRight;
    [SerializeField] private Image trinketImageRight;
    [SerializeField] private TextMeshProUGUI trinketRequirementsRight;

    private List<Trinket> trinkets;
    private int currentIndex = 0;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pageTurn;

    private bool doOnce = true;

    protected override void Start()
    {
        base.Start();
    }

    private void Update()
    {
        if (GameManager.Instance.hasGotSpawnPositions && doOnce)
        {
            trinkets = GameManager.Instance.currentTrinkets;

            if (trinkets != null && trinkets.Count >= 2)
            {
                // Display the first two trinkets
                SetUIFromTrinket(trinkets[0], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
                SetUIFromTrinket(trinkets[1], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
                currentIndex = 0;
            }
            else if (trinkets != null && trinkets.Count == 1)
            {
                // Only one trinket, display it on the left page
                SetUIFromTrinket(trinkets[0], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
                ClearRightPage();
            }
            else
            {
                Debug.LogError("Trinkets list is null or empty");
            }

            doOnce = false; // Reset the flag
        }
    }

    public void FlipRight()
    {
        // Check if there are at least 2 more trinkets to display
        if (currentIndex + 2 < trinkets.Count)
        {
            SetUIFromTrinket(trinkets[currentIndex + 2], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);

            // Check if there is a trinket for the right page
            if (currentIndex + 3 < trinkets.Count)
            {
                SetUIFromTrinket(trinkets[currentIndex + 3], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            }
            else
            {
                ClearRightPage();
            }

            currentIndex += 2;
            Debug.Log("Current index: " + currentIndex);
            audioSource.PlayOneShot(pageTurn);
        }
        else
        {
            Debug.Log("No more trinkets to display");
        }
    }

    public void FlipLeft()
    {
        // Check if there are at least 2 previous trinkets to display
        if (currentIndex - 2 >= 0)
        {
            SetUIFromTrinket(trinkets[currentIndex - 2], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            SetUIFromTrinket(trinkets[currentIndex - 1], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            currentIndex -= 2;
            Debug.Log("Current index: " + currentIndex);
            audioSource.PlayOneShot(pageTurn);
        }
        else
        {
            Debug.Log("No more trinkets to display");
        }
    }

    private void SetUIFromTrinket(Trinket trinket, TextMeshProUGUI name, TextMeshProUGUI desc, Image image, TextMeshProUGUI requirements)
    {
        // Display trinket information in UI
        name.text = trinket.trinketName;
        desc.text = trinket.trinketDescription;
        image.sprite = trinket.trinketImage;

        requirements.text = "REQUIREMENTS\n\n";

        if (trinket.requiredRace != Race.NONE)
        {
            requirements.text += "RACE: " + trinket.requiredRace.ToString() + "\n";
        }
        if (trinket.requiredKingdom != Kingdom.NONE)
        {
            requirements.text += trinket.requiredKingdom.ToString() + " KINGDOM" + "\n";
        }
        if (trinket.requiredOccupation != Occupation.NONE)
        {
            requirements.text += "OCCUPATION: " + trinket.requiredOccupation.ToString() + "\n";
        }
        if (trinket.requiredAge != 0)
        {
            requirements.text += "AGE: " + trinket.requiredAge.ToString() + "\n";
        }
        if (trinket.requiredGuild != Guild.NONE)
        {
            requirements.text += "GUILD: " + trinket.requiredGuild.ToString() + "\n";
        }
        if (trinket.requiredGuildRank != GuildRank.NONE)
        {
            requirements.text += "RANK: " + trinket.requiredGuildRank.ToString() + "\n";
        }
    }

    private void ClearRightPage()
    {
        // Clear the right page UI
        trinketNameRight.text = "";
        trinketDescriptionRight.text = "";
        trinketImageRight.sprite = null;
        trinketRequirementsRight.text = "";
    }
}