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

    private void Start()
    {
        customerIndex = 0;

        if (customers.Count > 0)
        {
            StartCoroutine(SpawnNextCustomer(customerIndex, 0));
        }

        audioSource = GetComponent<AudioSource>();
    }

    [Flags]
    public enum RequirementFlags
    {
        None = 0,
        Race = 1 << 0,         // 1
        Kingdom = 1 << 1,      // 2
        Occupation = 1 << 2,   // 4
        Level = 1 << 3         // 8
    }

    public class ItemRequirements
    {
        public RequirementFlags ActiveRequirements { get; set; }
        public Race RequiredRace { get; set; }
        public Kingdom RequiredKingdom { get; set; }
        public Occupation RequiredOccupation { get; set; }
        public int RequiredLevel { get; set; }
    }

    public bool CheckRequirements(ItemRequirements itemRequirements, Race playerRace, Kingdom playerKingdom, Occupation playerOccupation, int playerLevel)
    {
        // Check Race requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Race) &&
            itemRequirements.RequiredRace != playerRace)
        {
            return false;
        }

        // Check Kingdom requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Kingdom) &&
            itemRequirements.RequiredKingdom != playerKingdom)
        {
            return false;
        }

        // Check Occupation requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Occupation) &&
            itemRequirements.RequiredOccupation != playerOccupation)
        {
            return false;
        }

        // Check Level requirement
        if (itemRequirements.ActiveRequirements.HasFlag(RequirementFlags.Level) &&
            itemRequirements.RequiredLevel > playerLevel)
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
}
