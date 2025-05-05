using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    public GameObject dialogueUI;
    public TMP_Text dialogueText;
    public float typingSpeed = 0.05f;

    private Coroutine currentTyping;
    private bool isTyping = false;
    private bool skipTyping = false;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        dialogueUI.SetActive(false);
    }

    void Update()
    {
        if (!dialogueUI.activeSelf) return;

        if (isTyping && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            skipTyping = true;
        }
        else if (!isTyping && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            dialogueUI.SetActive(false);
        }
    }

    public void ShowDialogue(string text)
    {
        if (currentTyping != null)
            StopCoroutine(currentTyping);

        currentTyping = StartCoroutine(TypeText(text));
    }

    private IEnumerator TypeText(string fullText)
    {
        dialogueUI.SetActive(true);
        dialogueText.text = "";
        isTyping = true;
        skipTyping = false;

        for (int i = 0; i < fullText.Length; i++)
        {
            if (skipTyping)
            {
                dialogueText.text = fullText;
                break;
            }

            dialogueText.text += fullText[i];
            yield return new WaitForSeconds(typingSpeed);
        }

        isTyping = false;
    }
}
