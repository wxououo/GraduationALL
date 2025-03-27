using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class RotaryDialController : MonoBehaviour
{
    public Transform dial; // 輪盤的Transform
    public float[] numberAngles = {1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f }; // 每個數字的角度位置
    private string currentInput = ""; // 當前輸入的數字序列
    public string correctCode = "1109"; // 密碼
    public float rotationSpeed = 400f; // 旋轉速度
    public float waitTime = 1f; // 等待時間
    public string originalSceneName; // 原始場景的名稱
    public List<Button> numberButtons;

    public AudioClip rotateSound; // 音效剪輯
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
        float startAngle = dial.localEulerAngles.z;

        // 確保撥號旋轉只能往逆時針方向
        float targetAngle = startAngle + Mathf.Abs(Mathf.DeltaAngle(startAngle, numberAngles[number]));

        // 播放旋轉音效
        if (rotateSound != null && audioSource != null)
        {
            audioSource.clip = rotateSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        // 逆時針旋轉到目標角度
        while (Mathf.Abs(Mathf.DeltaAngle(dial.localEulerAngles.z, targetAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(dial.localEulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime);
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
        float returnAngle = startAngle - Mathf.Abs(Mathf.DeltaAngle(dial.localEulerAngles.z, startAngle));

        while (Mathf.Abs(Mathf.DeltaAngle(dial.localEulerAngles.z, returnAngle)) > 0.1f)
        {
            float angle = Mathf.MoveTowardsAngle(dial.localEulerAngles.z, returnAngle, rotationSpeed * Time.deltaTime);
            dial.localEulerAngles = new Vector3(0, 0, angle);
            yield return null;
        }

        // 停止音效
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }

        EnableButtons();
    }



    private void CheckCode()
    {
        if (currentInput == correctCode)
        {
            Unlock();
        }
        else
        {
            ResetInput();
        }
    }

    private void Unlock()
    {
        PlayerPrefs.SetInt("IsUnlocked", 1); // 保存解鎖狀態
        PlayerPrefs.Save(); // 確保狀態被保存
        SceneManager.LoadScene(originalSceneName);
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
