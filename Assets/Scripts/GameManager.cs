using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        // Ensure only one instance of GameManager exists
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
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

    public List<CustomerLogic> customers;
    public CustomerLogic currentCustomer { get; private set; }
    public Transform customerSpawn;
    public Transform target;
    public int customerIndex { get; private set; }

    public GameObject leftPopup;
    public GameObject rightPopup;

    private AudioSource audioSource;

    public UnityEvent OnSale;

    public List<Transform> trinketSpawnPoints;

    public List<Trinket> day1Trinkets;
    public List<Trinket> day2Trinkets;
    public List<Trinket> day3Trinkets;
    public List<Trinket> day4Trinkets;
    public List<Trinket> day5Trinkets;

    private int currentDay = 0;

    public List<Trinket> currentTrinkets;

    public List<Transform> documentSpawnPoints;

    [SerializeField] private GameObject pauseMenu;

    private void Start()
    {
        customerIndex = 0;

        if (customers.Count > 0)
        {
            StartCoroutine(SpawnNextCustomer(customerIndex, 0));
        }

        audioSource = GetComponent<AudioSource>();

        StartNewDay();
    }

    private void SpawnNewTrinkets(List<Trinket> trinketList)
    {
        currentTrinkets.Clear();
        for (int i = 0; i < trinketSpawnPoints.Count; i++)
        {
            Trinket trinket = Instantiate(trinketList[i], trinketSpawnPoints[i].position, Quaternion.identity);
            currentTrinkets.Add(trinket);
        }
    }

    private void StartNewDay()
    {
        switch (currentDay)
        {
            case 0:
                SpawnNewTrinkets(day1Trinkets);
                break;
            case 1:
                SpawnNewTrinkets(day2Trinkets);
                break;
            case 2:
                SpawnNewTrinkets(day3Trinkets);
                break;
            case 3:
                SpawnNewTrinkets(day4Trinkets);
                break;
            case 4:
                SpawnNewTrinkets(day5Trinkets);
                break;
            default:
                Debug.LogError("Invalid day number");
                break;
        }

        currentDay++;
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
        GuildRank = 1 << 5    // 32
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

    public bool CheckRequirements(ItemRequirements itemRequirements, Race customerRace, Kingdom customerKingdom, Occupation customerOccupation, int customerAge, Guild customerGuild, GuildRank customerGuildRank)
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

    public IEnumerator SpawnNextCustomer(int index, float delay)
    {
        yield return new WaitForSeconds(delay);
        CustomerLogic customer = Instantiate(customers[index], customerSpawn.position, Quaternion.identity);
        currentCustomer = customer;
        customerIndex++;
        print("Customer spawned");
    }

    public void SetPlayerDocumentState(bool isInDocument)
    {
        if (isInDocument)
        {
            SetDocumentState(CursorLockMode.None, true, 0);
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

    public void PlayAudio(AudioResource audioResource, float volume)
    {
        if (audioResource != null)
        {
            audioSource.volume = volume;
            audioSource.Play();
        }
        else
        {
            Debug.LogWarning("AudioResource is not assigned to " + name);
        }
    }
    
    public void TogglePauseMenu(bool on)
    {
        if (on)
        {
            Time.timeScale = 0;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            pauseMenu.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            pauseMenu.gameObject.SetActive(false);
        }
    }
}
