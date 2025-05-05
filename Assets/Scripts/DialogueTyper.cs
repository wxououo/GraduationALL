using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI; // �Y�� TextMeshPro �Ч�� TMP ����
using UnityEngine.SceneManagement;

public class DialogueTyper : MonoBehaviour
{
    public TMP_Text dialogueText; // �i�令 TMP_Text
    public GameObject dialogueUI; // �I�����O

    public float typingSpeed = 0.05f;
    private string fullText = "�o�̬O���̡K�K�H���Pı�ǩǪ��C";

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
