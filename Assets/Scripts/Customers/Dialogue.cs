using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class CustomerDialogue : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textSpeed;
    [SerializeField] private string dialogue;

    private CustomerMovement customerMovement;

    private bool isTalking = false;
    private Coroutine talkingCoroutine;
    private Coroutine audioCoroutine;

    [SerializeField] private bool isCustomer;

    [SerializeField] private List<AudioClip> sfxList; // List of SFX clips
    [SerializeField] private float playDuration; // Total duration to play SFX
    [SerializeField] private float minDelayBetweenSFX; // Minimum delay between SFX
    [SerializeField] private float maxDelayBetweenSFX; // Maximum delay between SFX

    private AudioSource audioSource;
    private bool isPlaying = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void Start()
    {
        if (isCustomer)
        {
            customerMovement = GetComponent<CustomerMovement>();
        }

        canvas.gameObject.SetActive(false);
        dialogueText.text = dialogue;
    }

    private void Update()
    {
        if (isCustomer)
        {
            if (customerMovement.showDialogue)
            {
                StartOrResetDialogue(dialogue);
                customerMovement.showDialogue = false;
            }
        }
    }

    public void StartOrResetDialogue(string newText)
    {
        // If a coroutine is already running, stop it
        if (isTalking)
        {
            if (talkingCoroutine != null)
            {
                StopCoroutine(talkingCoroutine); 
            }

            isTalking = false;
        }

        if (isPlaying)
        {
            if (audioCoroutine != null)
            {
                StopCoroutine(audioCoroutine);
            }

            isPlaying = false;
        }

        // Clear the existing text
        dialogueText.text = "";

        // Set the canvas to active
        canvas.gameObject.SetActive(true);

        // Start the new coroutine
        talkingCoroutine = StartCoroutine(TypeLine(newText));

        if (!isPlaying)
        {
            audioCoroutine = StartCoroutine(PlayAudioForDuration());
        }

        if (!isCustomer)
        {
            GameManager.Instance.hasTalkedWithBoss = true;
        }
    }

    private IEnumerator TypeLine(string line)
    {
        isTalking = true;
        dialogueText.text = "";
        StringBuilder sb = new StringBuilder();
        foreach (char c in line)
        {
            sb.Append(c);
            dialogueText.text = sb.ToString();
            yield return new WaitForSeconds(textSpeed);
        }
        isTalking = false;
    }

    public IEnumerator HideDialogue(float timeToHide)
    {
        yield return new WaitForSeconds(timeToHide);
        dialogueText.text = "";

        if (talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            StopCoroutine(audioCoroutine);
        }

        canvas.gameObject.SetActive(false);
    }

    private IEnumerator PlayAudioForDuration()
    {
        isPlaying = true;
        float startTime = Time.time;

        while (Time.time - startTime < playDuration)
        {
            // Play a random SFX
            AudioClip randomClip = sfxList[Random.Range(0, sfxList.Count)];
            audioSource.PlayOneShot(randomClip);

            // Wait for the clip to finish playing
            yield return new WaitForSeconds(randomClip.length);

            // Wait for a random delay before playing the next SFX
            float delay = Random.Range(minDelayBetweenSFX, maxDelayBetweenSFX);
            yield return new WaitForSeconds(delay);
        }

        isPlaying = false;
        Debug.Log("Finished playing SFX for the specified duration.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isCustomer)
        {
            if (other.CompareTag("Player") && !customerMovement.firstDialogue)
            {
                StartOrResetDialogue(dialogue);
            }
        }

        if (other.CompareTag("Player") && GameManager.Instance.canTalkWithBoss)
        {
            StartOrResetDialogue(dialogue);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(HideDialogue(0f));
        }

        if (!isCustomer)
        {
            if (other.CompareTag("Player") && GameManager.Instance.canTalkWithBoss)
            {
                StartCoroutine(HideDialogue(0f));
            }
        }
    }
}
