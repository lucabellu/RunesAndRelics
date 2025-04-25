using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Dialogue : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private float textSpeed;

    [SerializeField] private string dialogue;

    [SerializeField] private string cleanDialogue;
    public bool firstBossDialogue = true;

    private CustomerMovement customerMovement;

    private bool isTalking = false;
    private Coroutine talkingCoroutine;
    private Coroutine audioCoroutine;

    [SerializeField] private bool isCustomer;

    [SerializeField] private List<AudioClip> sfxList;
    [SerializeField] private float playDuration;
    [SerializeField] private float minDelayBetweenSFX;
    [SerializeField] private float maxDelayBetweenSFX;

    [SerializeField] private AudioSource audioSource;
    private bool isPlayingAudio = false;
    private bool isInPlayerRange = false;

    private void Start()
    {
        if (isCustomer)
        {
            customerMovement = GetComponentInParent<CustomerMovement>();
        }

        canvas.gameObject.SetActive(false);


        if (isCustomer)
        {
            dialogueText.text = dialogue;
        }
        else
        {
            dialogueText.text = cleanDialogue;
        }
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

    private void ResetDialogueState()
    {
        if (isTalking && talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            isTalking = false;
        }

        if (isPlayingAudio && audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioSource.Stop();
            isPlayingAudio = false;
        }
    }

    public void StartOrResetDialogue(string newText)
    {
        if (isPlayingAudio || isTalking) return;

        ResetDialogueState();

        dialogueText.text = "";
        canvas.gameObject.SetActive(true);
        talkingCoroutine = StartCoroutine(TypeLine(newText));
        audioCoroutine = StartCoroutine(PlayAudioForDuration());

        if (!isCustomer && firstBossDialogue)
        {
            GameManager.Instance.hasTalkedWithBossOnce = true;
        }
        else if (!isCustomer && !firstBossDialogue)
        {
            GameManager.Instance.hasTalkedWithBossTwice = true;
        }

        if (!isInPlayerRange)
        {
            StartCoroutine(HideDialogue(playDuration + 1f));
        }

        StartCoroutine(StopAudio());
    }

    private IEnumerator TypeLine(string line)
    {
        isTalking = true;
        dialogueText.text = "";
        foreach (char c in line)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
        isTalking = false;
    }

    private IEnumerator StopAudio()
    {
        yield return new WaitForSeconds(playDuration);
        audioSource.Stop();
    }

    public IEnumerator HideDialogue(float timeToHide)
    {
        yield return new WaitForSeconds(timeToHide);
        canvas.gameObject.SetActive(false);

        if (isTalking && talkingCoroutine != null)
        {
            StopCoroutine(talkingCoroutine);
            isTalking = false;
        }

        if (isPlayingAudio && audioCoroutine != null)
        {
            StopCoroutine(audioCoroutine);
            audioSource.Stop();
            isPlayingAudio = false;
        }
    }

    private IEnumerator PlayAudioForDuration()
    {
        isPlayingAudio = true;
        float startTime = Time.time;

        while (Time.time - startTime < playDuration)
        {
            AudioClip randomClip = sfxList[Random.Range(0, sfxList.Count)];
            audioSource.PlayOneShot(randomClip);
            yield return new WaitForSeconds(randomClip.length);

            float delay = Random.Range(minDelayBetweenSFX, maxDelayBetweenSFX);
            yield return new WaitForSeconds(delay);
        }

        isPlayingAudio = false;
        Debug.Log("Finished playing SFX for the specified duration.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            isInPlayerRange = true;

            if (isCustomer && !customerMovement.firstDialogue)
            {
                StartOrResetDialogue(dialogue);
            }

            else if (GameManager.Instance.canTalkWithBoss && firstBossDialogue)
            {
                StartOrResetDialogue(cleanDialogue);
            }
            else if (GameManager.Instance.canTalkWithBoss && !firstBossDialogue)
            {
                StartOrResetDialogue(dialogue);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInPlayerRange = false;

            StartCoroutine(HideDialogue(0f));
        }

        if (!isCustomer)
        {
            if (other.CompareTag("Player") && GameManager.Instance.canTalkWithBoss)
            {
                StartCoroutine(HideDialogue(1f));
            }
        }
    }
}
