using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject); // Persist across scenes
            Time.timeScale = 1;
        }
        else
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    private void OnDestroy()
    {
        // Clear the reference to prevent holding onto stale objects
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public CustomerLogic currentCustomer;
    public Transform customerSpawn;
    public Transform target;
    public int customerIndex;

    public GameObject leftPopup;
    public GameObject rightPopup;

    [SerializeField] private AudioSource playDropSound;
    [SerializeField] private AudioSource playEnterSound;

    public List<Transform> trinketSpawnPoints;

    public List<Trinket> day1Trinkets;
    public List<Trinket> day2Trinkets;
    public List<Trinket> day3Trinkets;
    public List<Trinket> day4Trinkets;
    public List<Trinket> day5Trinkets;

    public List<CustomerLogic> day1Customers;
    public List<CustomerLogic> day2Customers;
    public List<CustomerLogic> day3Customers;
    public List<CustomerLogic> day4Customers;
    public List<CustomerLogic> day5Customers;

    public List<Task> day1Tasks;
    public List<Task> currentTasks;
    public int currentTaskIndex = 0;

    public List<GameObject> cobwebs;
    public bool canCleanCobwebs = false;

    public int currentDay { get; private set; } = 0;

    public List<Trinket> currentTrinkets;
    public List<CustomerLogic> currentCustomers;


    public List<Transform> documentSpawnPoints;

    [SerializeField] private GameObject pauseMenu;

    public bool canTalkWithBoss { get; private set; } = false;
    public bool hasTalkedWithBoss = false;

    [SerializeField] private ShopDoor shopDoor;
    [SerializeField] private ShopDoor bossDoor;

    [SerializeField] private Texture2D cursorTexture;
    public GameObject crosshair;

    public bool hasReadTutorialDocument = false;
    private bool doOnce = true;
    [SerializeField] private GameObject tutorialDocument;

    public bool hasGotSpawnPositions { get; private set; } = false;

    [SerializeField] private FadeTransition fadeTransition;
    public bool transitionInProgress = false;

    public bool isInPauseMenu { get; private set; } = false;
    public bool hasLost { get; private set; } = false;

    public int mistakesMade = 0;
    [SerializeField] private GameObject resumeButtom;
    [SerializeField] private GameObject restartButtom;
    [SerializeField] private GameObject loseText;

    private bool highlightDoorOnce = true;

    private void Start()
    {
        GetTrinketSpawnpoints();
        GetCobwebs();

        customerIndex = 0;
        IncrementDay();
        tutorialDocument.GetComponent<Outline>().enabled = true;
    }

    private void Update()
    {
        if (canTalkWithBoss && highlightDoorOnce)
        {
            bossDoor.HighlightDoor();
            highlightDoorOnce = false;
        }


        if (hasTalkedWithBoss)
        {
            bossDoor.UnhighlightDoor();
            shopDoor.HighlightDoor();
            canTalkWithBoss = false;
        }

        if (hasReadTutorialDocument && doOnce)
        {
            if (day1Customers.Count > 0)
            {
                StartNextTask();
            }

            doOnce = false;
            tutorialDocument.GetComponent<Outline>().enabled = false;
        }

        if (transitionInProgress)
        {
            crosshair.SetActive(false);
        }
        else
        {
            crosshair.SetActive(true);
        }

        if (Input.GetKeyDown(KeyCode.Escape) && !hasLost)
        {
            if (!isInPauseMenu)
            {
                TogglePauseMenu(true);
            }
            else
            {
                TogglePauseMenu(false);
            }
        }

        if (mistakesMade >= 3)
        {
            hasLost = true;
            TogglePauseMenu(true);
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (cobwebs.Count <= 0 && canCleanCobwebs)
        {
            currentTasks[currentTaskIndex].CompleteTask();
            canCleanCobwebs = false;
            StartNextTask();
        }
    }

    private void GetTrinketSpawnpoints()
    {
        foreach (GameObject spawnpoint in GameObject.FindGameObjectsWithTag("Spawnpoint"))
        {
            trinketSpawnPoints.Add(spawnpoint.transform);
        }

        hasGotSpawnPositions = true;
    }

    private void GetCobwebs()
    {
        foreach (GameObject cobweb in GameObject.FindGameObjectsWithTag("Cobweb"))
        {
            cobwebs.Add(cobweb);
        }
    }


    private void SpawnNewTrinkets(List<Trinket> trinketList)
    {
        if (currentDay > 1)
        {
            currentTrinkets.Clear();
        }


        for (int i = 0; i < trinketList.Count; i++)
        {
            Trinket trinket = Instantiate(trinketList[i], trinketSpawnPoints[i].position, Quaternion.identity);
            currentTrinkets.Add(trinket);
        }
    }

    public void StartNextTask()
    {
        if (currentTaskIndex < day1Tasks.Count)
        {
            currentTasks[currentTaskIndex].StartTask();
        }
        else
        {
            canTalkWithBoss = true;
            Debug.Log("All tasks completed for today!");
        }
    }

    private void IncrementDay()
    {
        switch (currentDay)
        {
            case 0:
                SpawnNewTrinkets(day1Trinkets);
                currentCustomers = day1Customers;
                currentTasks = day1Tasks;
                break;
            case 1:
                SpawnNewTrinkets(day2Trinkets);
                currentCustomers = day2Customers;
                break;
            case 2:
                SpawnNewTrinkets(day3Trinkets);
                currentCustomers = day3Customers;
                break;
            case 3:
                SpawnNewTrinkets(day4Trinkets);
                currentCustomers = day4Customers;
                break;
            case 4:
                SpawnNewTrinkets(day5Trinkets);
                currentCustomers = day5Customers;
                break;
            default:
                Debug.LogError("Invalid day number");
                break;
        }

        currentDay++;
        StartCoroutine(fadeTransition.FadeInOut());
    }

    public void EndDay()
    {
        foreach (Trinket trinket in currentTrinkets)
        {
            Destroy(trinket.gameObject);
        }

        shopDoor.UnhighlightDoor();
        shopDoor.canInteract = false;
        customerIndex = 0;
        IncrementDay();
        canTalkWithBoss = false;
        hasTalkedWithBoss = false;

        if (currentCustomers.Count > 0)
        {
            StartNextTask();
        }
    }

    [Flags]
    public enum RequirementFlags
    {
        None = 0,
        Race = 1 << 0,         // 1
        Kingdom = 1 << 1,      // 2
        Occupation = 1 << 2,   // 4
        Age = 1 << 3,         // 8
        Guild = 1 << 4,       // 16
        GuildRank = 1 << 5,    // 32
        PurchaseItem = 1 << 6  // 64

    }

    public class ItemRequirements
    {
        public RequirementFlags ActiveRequirements { get; set; }
        public Race RequiredRace { get; set; }
        public Kingdom RequiredKingdom { get; set; }
        public Occupation RequiredOccupation { get; set; }
        public int RequiredAge { get; set; }
        public Guild RequiredGuild { get; set; }
        public GuildRank RequiredGuildRank { get; set; }
    }

    public bool CheckRequirements
        (ItemRequirements itemRequirements, 
        Race customerRace, 
        Kingdom customerKingdom, 
        Occupation customerOccupation, 
        int customerAge, 
        Guild customerGuild, 
        GuildRank customerGuildRank)
    {
        // Check Race requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Race) &&
            itemRequirements.RequiredRace != customerRace)
        {
            return false;
        }

        // Check Kingdom requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Kingdom) &&
            itemRequirements.RequiredKingdom != customerKingdom)
        {
            return false;
        }

        // Check Occupation requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Occupation) &&
            itemRequirements.RequiredOccupation != customerOccupation)
        {
            return false;
        }

        // Check Level requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Age) &&
            itemRequirements.RequiredAge > customerAge)
        {
            return false;
        }

        // Check Guild requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Guild) &&
            itemRequirements.RequiredGuild != customerGuild)
        {
            return false;
        }

        // Check GuildRank requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.GuildRank) &&
            itemRequirements.RequiredGuildRank != customerGuildRank)
        {
            return false;
        }

        // All active requirements are met
        return true;
    }

    public void SetPlayerDocumentState(bool isInDocument)
    {
        if (isInDocument)
        {
            SetDocumentState(CursorLockMode.None, true, 0);
            TogglePopup(true, false);
            TogglePopup(false, false);
        }
        else
        {
            SetDocumentState(CursorLockMode.Locked, false, 1);
        }
    }

    private void SetDocumentState(CursorLockMode cursorLockMode, bool visible, float timeScale)
    {
        Cursor.lockState = cursorLockMode;
        Cursor.visible = visible;
        Time.timeScale = timeScale;
        Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.ForceSoftware);
    }

    public void TogglePopup(bool isLeft, bool on)
    {
        if (isLeft)
        {
            if (on)
            {
                leftPopup.SetActive(true);
            }
            else
            {
                leftPopup.SetActive(false);
            }
        }
        else
        {
            if (on)
            {
                rightPopup.SetActive(true);
            }
            else
            {
                rightPopup.SetActive(false);
            }
        }
    }

    public void PlayDrop()
    {
        playDropSound.Play();
    }

    public void PlayEnterAudio()
    {
        playEnterSound.Play();
    }
    
    public void TogglePauseMenu(bool on)
    {
        if (on)
        {
            isInPauseMenu = true;

            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            pauseMenu.gameObject.SetActive(true);

            TogglePopup(true, false);
            TogglePopup(false, false);

            if (hasLost)
            {
                resumeButtom.SetActive(false);
                restartButtom.SetActive(true);
                loseText.SetActive(true);
            }

            crosshair.SetActive(false);
        }
        else
        {
            isInPauseMenu = false;

            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pauseMenu.gameObject.SetActive(false);

            crosshair.SetActive(true);
        }
    }
}
