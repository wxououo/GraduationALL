using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // 若用 TextMeshPro 請改用 TMP 版本
using UnityEngine.SceneManagement;

public class DialogueTyper : MonoBehaviour
{
    public TMP_Text dialogueText; // 可改成 TMP_Text
    public GameObject dialogueUI; // 背景面板

    public float typingSpeed = 0.05f;
    private string fullText = "這裡是哪裡……？怎麼感覺怪怪的。";

    private static bool hasShownDialogue = false;

    private bool isTyping = false;
    private bool skipTyping = false;

    void Start()
    {
        if (!hasShownDialogue)
        {
            StartCoroutine(ShowDialogue());
            hasShownDialogue = true;
        }
        else
        {
            dialogueUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isTyping && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            skipTyping = true;
        }
        else if (!isTyping && dialogueUI.activeSelf && (Input.anyKeyDown || Input.GetMouseButtonDown(0)))
        {
            dialogueUI.SetActive(false);
        }
    }

    IEnumerator ShowDialogue()
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
