using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LockController : MonoBehaviour
{
    public float rotationSpeed = 100f;  // 旋轉速度
    public int currentNumber = 0;       // 當前數字
    public int buttonIndex; // 按鈕在密碼中的索引
    //private float currentRotation = 0f;
    public Transform wheelTransform;    // 確保已正確引用
    public BoxController boxController;

    public AudioClip rotationSound; // 滾輪轉動音效
    private AudioSource audioSource;   // AudioSource 元件
    private bool isRotating = false;        // 追踪是否正在旋轉
    private Quaternion targetRotation;      // 目標旋轉


    void Start()
    {
        // 初始化 AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 檢查引用是否正確賦值
        if (wheelTransform == null)
        {
            Debug.LogError("wheelTransform is not assigned!");
        }
        // 初始化目標旋轉為當前旋轉
        targetRotation = wheelTransform.localRotation;
    }
    void Update()
    {
        if (isRotating)
        {
            // 使用 Quaternion.RotateTowards 進行平滑旋轉
            wheelTransform.localRotation = Quaternion.RotateTowards(
                wheelTransform.localRotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );

            // 檢查是否完成旋轉
            if (Quaternion.Angle(wheelTransform.localRotation, targetRotation) < 0.1f)
            {
                wheelTransform.localRotation = targetRotation;
                isRotating = false;
                if (audioSource.isPlaying)
                {
                    audioSource.Stop();
                }
            }
        }
    }
    void OnMouseDown()
    {
        if (!isRotating)
        {
            // 每次點擊增加一個數字（向上旋轉）
            currentNumber = (currentNumber + 1) % 8;

            // 計算新的目標旋轉
            targetRotation = wheelTransform.localRotation * Quaternion.Euler(45f, 0f, 0f);


            // 開始旋轉
            isRotating = true;

            // 播放音效
            if (rotationSound != null && !audioSource.isPlaying)
            {
                audioSource.clip = rotationSound;
                audioSource.loop = false;  // 改為不循環播放
                audioSource.Play();
            }

            // 更新 BoxController
            if (boxController != null)
            {
                boxController.SetDigit(buttonIndex, currentNumber);
            }
            else
            {
                Debug.LogError("BoxController 未正確分配。");
            }

            Debug.Log("Current Number: " + currentNumber);
        }
    }

}
