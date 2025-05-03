using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RotaryDialController : MonoBehaviour
{
    public Transform dial; // 輪盤的Transform
    public float[] numberAngles = { -30f, 300f, 270f, 240f, 210f, 180f, 150f, 120f, 60f, 30f }; // 每個數字的角度位置
    private string currentInput = ""; // 當前輸入的數字序列
    public string correctCode = "1109"; // 密碼
    public float rotationSpeed = 400f; // 旋轉速度
    public float waitTime = 1f; // 等待時間
    public string originalSceneName; // 原始場景的名稱
    public List<Button> numberButtons;

    public AudioClip rotateSound; // 旋轉音效剪輯
    public AudioClip unlockSound; // 解鎖音效剪輯
    private AudioSource audioSource; // AudioSource 元件

    private void Start()
    {
        // 初始化 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    // 當某個數字按鈕被點擊時調用
    public void OnNumberButtonClick(int number)
    {
        if (currentInput.Length < correctCode.Length)
        {
            currentInput += number.ToString();
            DisableButtons();
            // 開始旋轉協程
            StartCoroutine(RotateDialToNumberAndBack(number));

            // 如果輸入完成，檢查密碼
            if (currentInput.Length == correctCode.Length)
            {
                CheckCode();
            }
        }
    }

    private IEnumerator RotateDialToNumberAndBack(int number)
    {
        float targetAngle = numberAngles[number];
        float startAngle = dial.localEulerAngles.z;
        // 確保始終逆時針旋轉
        float adjustedTargetAngle = startAngle > targetAngle ? targetAngle : targetAngle - 360f;

        // 播放旋轉音效
        if (rotateSound != null && audioSource != null)
        {
            audioSource.clip = rotateSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 逆時針旋轉到目標角度
        while (Mathf.Abs(Mathf.DeltaAngle(dial.localEulerAngles.z, adjustedTargetAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(dial.localEulerAngles.z, adjustedTargetAngle, rotationSpeed * Time.deltaTime);
            dial.localEulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }

        // 停止音效
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        yield return new WaitForSeconds(waitTime);

        // 播放旋轉回初始位置的音效
        if (rotateSound != null && audioSource != null)
        {
            audioSource.clip = rotateSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 確保回到初始位置時只能順時針旋轉
        while (Mathf.Abs(Mathf.DeltaAngle(dial.localEulerAngles.z, startAngle - 360f)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(dial.localEulerAngles.z, startAngle - 360f, rotationSpeed * Time.deltaTime);
            dial.localEulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }

        // 停止音效
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        EnableButtons();

        // 如果這是最後一個數字且密碼正確，等待一小段時間後播放解鎖音效
        if (currentInput.Length == correctCode.Length && currentInput == correctCode)
        {
            yield return new WaitForSeconds(0.5f); // 等待 0.5 秒
            Unlock();
        }
    }

    private void CheckCode()
    {
        if (currentInput == correctCode)
        {
            // 移除這裡的 Unlock 調用，因為我們現在在旋轉完成後調用
        }
        else
        {
            ResetInput();
        }
    }

    private void Unlock()
    {
        try
        {
            if (audioSource != null && unlockSound != null)
            {
                audioSource.PlayOneShot(unlockSound);
                StartCoroutine(LoadOriginalSceneAfterSound());
            }
            else
            {
                Debug.LogWarning("解鎖音效未設置，直接切換場景");
                LoadOriginalScene();
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"播放解鎖音效時發生錯誤: {e.Message}");
            LoadOriginalScene();
        }
    }

    private void LoadOriginalScene()
    {
        PlayerPrefs.SetInt("IsUnlocked", 1); // 保存解鎖狀態
        PlayerPrefs.Save(); // 確保狀態被保存
        SceneManager.LoadScene(originalSceneName);
    }

    private IEnumerator LoadOriginalSceneAfterSound()
    {
        if (unlockSound != null)
        {
            yield return new WaitForSeconds(unlockSound.length);
        }
        LoadOriginalScene();
    }

    private void ResetInput()
    {
        currentInput = "";
    }

    private void DisableButtons()
    {
        foreach (Button button in numberButtons)
        {
            button.interactable = false;
        }
    }

    private void EnableButtons()
    {
        foreach (Button button in numberButtons)
        {
            button.interactable = true;
        }
    }
}