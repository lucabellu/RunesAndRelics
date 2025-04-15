using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Time.timeScale = 1;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
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


    public int mistakesMade = 0;
    [SerializeField] private const int MaxMistakesAllowed = 3;

    [Serializable]
    public class DayData
    {
        public List<Trinket> trinkets;
        public List<CustomerLogic> customers;
        public List<Task> tasks;
    }

    public List<DayData> days = new();
    public List<Task> currentTasks;
    public int currentTaskIndex = 0;

    public List<Trinket> currentTrinkets;
    public List<CustomerLogic> currentCustomers;

    public int currentDay { get; private set; } = 0;

    public List<Transform> documentSpawnPoints;

    [SerializeField] private GameObject pauseMenu;

    public bool canTalkWithBoss = false;
    public bool hasTalkedWithBoss = false;
    public bool highlightBossDoorOnce = true;

    [SerializeField] private ShopDoor shopDoor;

    public ShopDoor bossDoor;

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

    [SerializeField] private GameObject resumeButtom;
    [SerializeField] private GameObject restartButtom;
    [SerializeField] private GameObject loseText;

    public List<Transform> trinketSpawnPoints;

    [SerializeField] private AudioSource playDropSound;
    [SerializeField] private AudioSource playEnterSound;


    public List<GameObject> cobwebs;
    public bool canCleanCobwebs = false;

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
        HandleDoorHighlighting();
        HandleTutorialProgress();
        HandlePauseToggle();
        HandleRestartInput();
        CheckCobwebCompletion();
    }

    private void HandleDoorHighlighting()
    {
        if (canTalkWithBoss && highlightBossDoorOnce)
        {
            bossDoor.HighlightDoor();
            highlightBossDoorOnce = false;
        }

        if (hasTalkedWithBoss && !bossDoor.transform.GetComponent<Dialogue>().firstBossDialogue)
        {
            bossDoor.UnhighlightDoor();
            shopDoor.HighlightDoor();
            canTalkWithBoss = false;
        }
        else if (hasTalkedWithBoss)
        {
            bossDoor.UnhighlightDoor();
            canTalkWithBoss = false;
        }
    }

    private void HandleTutorialProgress()
    {
        if (hasReadTutorialDocument && doOnce)
        {
            if (currentCustomers.Count > 0)
            {
                StartNextTask();
            }

            doOnce = false;
            tutorialDocument.GetComponent<Outline>().enabled = false;
        }
    }

    private void HandlePauseToggle()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !hasLost)
        {
            TogglePauseMenu(!isInPauseMenu);
        }

        if (mistakesMade >= MaxMistakesAllowed)
        {
            hasLost = true;
            TogglePauseMenu(true);
        }
    }

    private void HandleRestartInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            foreach (GameObject cobweb in cobwebs)
            {
                Destroy(cobweb);
            }
        }
    }

    private void CheckCobwebCompletion()
    {
        if (cobwebs.Count <= 0 && canCleanCobwebs)
        {
            currentTasks[currentTaskIndex].CompleteTask();
            canCleanCobwebs = false;
            StartNextTask();
        }

        crosshair.SetActive(!transitionInProgress);
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
        if (currentTaskIndex < currentTasks.Count)
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
        if (currentDay >= days.Count)
        {
            Debug.LogError("Invalid day number");
            return;
        }

        DayData data = days[currentDay];

        SpawnNewTrinkets(data.trinkets);
        currentCustomers = data.customers;
        currentTasks = data.tasks;

        foreach (Task task in data.tasks)
        {
            print(task);
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

    public enum PopupSide { LEFT, RIGHT }

    public void TogglePopup(PopupSide side, bool on)
    {
        GameObject popup = side == PopupSide.LEFT ? leftPopup : rightPopup;
        popup.SetActive(on);
    }

    public void SetPlayerDocumentState(bool isInDocument)
    {
        if (isInDocument)
        {
            SetDocumentState(CursorLockMode.None, true, 0);
            TogglePopup(PopupSide.LEFT, false);
            TogglePopup(PopupSide.RIGHT, false);
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

    public void PlayDrop() => playDropSound.Play();
    public void PlayEnterAudio() => playEnterSound.Play();

    public void TogglePauseMenu(bool on)
    {
        isInPauseMenu = on;
        Time.timeScale = on ? 0 : 1;
        Cursor.lockState = on ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = on;

        pauseMenu.SetActive(on);

        if (on)
        {
            TogglePopup(PopupSide.LEFT, false);
            TogglePopup(PopupSide.RIGHT, false);

            if (hasLost)
            {
                resumeButtom.SetActive(false);
                restartButtom.SetActive(true);
                loseText.SetActive(true);
            }
        }

        crosshair.SetActive(!on);
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
}
