using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using static GameManager;

public class Manifest : MonoBehaviour, IHighlightable, IInteractable
{
    //objectives
    //
    //get list of all trinkets in scene from game manager
    //display trinket information in UI
    //display 2 trinkets at a time

    [SerializeField] private GameObject manifestCanvas;

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

    private void Start()
    {
        trinkets = GameManager.Instance.currentTrinkets;

        if (trinkets != null && trinkets.Count >= 2)
        {
            SetUIFromTrinket(trinkets[0], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            SetUIFromTrinket(trinkets[1], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            currentIndex = 0;
        }
        else if (trinkets != null && trinkets.Count == 1)
        {
            SetUIFromTrinket(trinkets[0], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            trinketNameRight.text = "";
            trinketDescriptionRight.text = "";
            trinketImageRight.sprite = null;
            trinketRequirementsRight.text = "";
        }
        else
        {
            Debug.LogError("Trinkets list is null or empty");
        }

    }

    public void OnInteract(bool isInteracting)
    {
        if (isInteracting)
        {
            manifestCanvas.gameObject.SetActive(true);
        }
        else
        {
            manifestCanvas.gameObject.SetActive(false);
        }
    }

    public void OnHighlight(bool isHovering)
    {
        if (isHovering)
        {
            GetComponent<Outline>().enabled = true;
            GameManager.Instance.TogglePopup(true, true);
        }
        else
        {
            GetComponent<Outline>().enabled = false;
            GameManager.Instance.TogglePopup(true, false);
        }
    }

    public void FlipRight()
    {
        //display next 2 trinkets
        if (trinkets.Count > currentIndex + 2)
        {
            SetUIFromTrinket(trinkets[currentIndex + 1], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            SetUIFromTrinket(trinkets[currentIndex + 2], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            currentIndex += 2;
            Debug.Log("Current index: " + currentIndex);
        }
        else if (trinkets.Count > currentIndex + 1)
        {
            SetUIFromTrinket(trinkets[currentIndex + 1], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            trinketNameRight.text = "";
            trinketDescriptionRight.text = "";
            trinketImageRight.sprite = null;
            trinketRequirementsRight.text = "";
            currentIndex += 2;
            Debug.Log("Current index: " + currentIndex);
        }
        else
        {
            Debug.Log("No more trinkets to display");
        }
    }

    public void FlipLeft()
    {
        //display previous 2 trinkets
        if (currentIndex - 2 >= 0)
        {
            SetUIFromTrinket(trinkets[currentIndex - 2], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            SetUIFromTrinket(trinkets[currentIndex - 1], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            currentIndex -= 2;
            Debug.Log("Current index: " + currentIndex);
        }

        else if (currentIndex - 1 >= 0)
        {
            SetUIFromTrinket(trinkets[currentIndex - 2], trinketNameLeft, trinketDescriptionLeft, trinketImageLeft, trinketRequirementsLeft);
            SetUIFromTrinket(trinkets[currentIndex - 1], trinketNameRight, trinketDescriptionRight, trinketImageRight, trinketRequirementsRight);
            currentIndex -= 2;
            Debug.Log("Current index: " + currentIndex);
        }
        else
        {
            Debug.Log("No more trinkets to display");
        }
    }

    private void SetUIFromTrinket(Trinket trinket, TextMeshProUGUI name, TextMeshProUGUI desc, Image image, TextMeshProUGUI requirements)
    {
        //display trinket information in UI
        name.text = trinket.trinketName;
        desc.text = trinket.trinketDescription;
        image.sprite = trinket.trinketImage;

        requirements.text = "Requirements:\n";

        if (trinket.requiredRace != Race.NONE)
        {
            requirements.text += trinket.requiredRace.ToString() + "\n";
        }
        if (trinket.requiredKingdom != Kingdom.NONE)
        {
            requirements.text += trinket.requiredKingdom.ToString() + "\n";
        }
        if (trinket.requiredOccupation != Occupation.NONE)
        {
            requirements.text += trinket.requiredOccupation.ToString() + "\n";
        }
        if (trinket.requiredAge != 0)
        {
            requirements.text += trinket.requiredAge.ToString() + "\n";
        }
        if (trinket.requiredGuild != Guild.NONE)
        {
            requirements.text += trinket.requiredGuild.ToString() + "\n";
        }
        if (trinket.requiredGuildRank != GuildRank.NONE)
        {
            requirements.text += trinket.requiredGuildRank.ToString() + "\n";
        }


    }
}
