using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance;

    [Header("Dialogue UI")]
    public GameObject dialogueBox;
    public TMP_Text dialogueText;
    public Button nextButton;
    public GameObject inputBlocker;

    private string[] messages;
    private int index = 0;
    private bool typing = false;

    public static bool InputLocked = false;

    [Header("Typing Sound")]
    public AudioSource typingAudio;
    public AudioClip typingClip;
    public AudioClip PopupClip;

    Coroutine typingCoroutine;

    private void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if(typingAudio == null) 
            typingAudio = GetComponent<AudioSource>();

        dialogueBox.SetActive(false);
    }

   public void StartDialogue(string[] newMessages)
    {
        if (newMessages == null || newMessages.Length == 0)
            return;

        messages = newMessages;
        index = 0;

        InputLocked = true;

        if (HintController.Instance != null)
            HintController.Instance.HideHint();

        inputBlocker.SetActive(true);
        nextButton.interactable = false;

        // IMPORTANT: dialogueBox stays hidden for now
        dialogueBox.SetActive(false);

        StartCoroutine(DialogueOpenRoutine());
    }


    IEnumerator DialogueOpenRoutine()
    {
        //Play sound FIRST
        if (PopupClip != null && typingAudio != null)
            typingAudio.PlayOneShot(PopupClip);

        //Small anticipation pause
        yield return new WaitForSeconds(0.3f);

        //Show dialogue box
        dialogueBox.SetActive(true);

        //Start typing
        StartTyping(messages[index]);
    }

     void StartTyping(string text)
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        typing = true;
        dialogueText.text = "";
        nextButton.interactable = false;

        // Start typing sound
        if (typingClip != null)
        {
            typingAudio.clip = typingClip;
            typingAudio.loop = true;
            typingAudio.pitch = Random.Range(0.95f, 1.05f);
            typingAudio.Play();
        }

         float charDelay = typingClip != null
        ? typingClip.length * 0.85f
        : 0.03f;

        foreach (char c in text)
        {
            dialogueText.text += c;

            // Slight pause for punctuation (feels more natural)
            if (c == '.' || c == ',' || c == '!' || c == '?')
                yield return new WaitForSeconds(0.06f);
            if(c  == ' ')
                yield return new WaitForSeconds(charDelay * 0.5f);
            else
                yield return new WaitForSeconds(0.02f);
        }

        StopTypingSound();

        typing = false;
        nextButton.interactable = true;
    }

    public void NextMessage()
    {
        // If user clicks while text is still typing → skip typing
        if (typing)
        {
            StopTypingInstantly();
            return;
        }

        index++;

        if (index >= messages.Length)
        {
            EndDialogue();
        }
        else
        {
            nextButton.interactable = false;
            StartTyping(messages[index]);
        }
    }
     void StopTypingInstantly()
    {
        if (typingCoroutine != null)
            StopCoroutine(typingCoroutine);

        dialogueText.text = messages[index];
        StopTypingSound();

        typing = false;
        nextButton.interactable = true;
    }

    void StopTypingSound()
    {
        if (typingAudio != null && typingAudio.isPlaying)
            typingAudio.Stop();
    }

    private void EndDialogue()
    {
        StopTypingSound();

        InputLocked = false;

        dialogueBox.SetActive(false);
        inputBlocker.SetActive(false);

        if (HintController.Instance == null)
        {
            Debug.LogError("❌ HintController.Instance is NULL in EndDialogue!");
        }
        else
        {
            Debug.Log("✅ HintController.Instance FOUND: " + HintController.Instance.name);
            HintController.Instance.ShowSceneHint();
        }
    }

    public void ShowTooManyBeansWarning()
    {
        StartDialogue(new string[]
        {
            "Oops! You added more beans than this recipe needs.",
            "Don't worry — you can pick up the beans from the tin cup to remove the extra!"
        });
    }

}
