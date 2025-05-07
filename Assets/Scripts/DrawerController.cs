using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawerController : MonoBehaviour
{
    private bool isOpened = false;
    public bool requiresZoom = false; // 是否需要鏡頭放大才能切換場景
    private MouseLook cameraController; // 用來檢查鏡頭是否已調整

    [Header("音效設定")]
    public AudioClip openSound;    // 開抽屜音效
    public AudioClip closeSound;   // 關抽屜音效
    [Range(0.5f, 2f)]
    public float soundPitch = 2f;  // 音效播放速度，預設稍快
    private AudioSource audioSource;

    void Start()
    {
        cameraController = Camera.main.GetComponent<MouseLook>();
        // 確保有 AudioSource 組件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        // 設置音效播放速度
        audioSource.pitch = soundPitch;
    }

    void OnMouseDown()
    {
        if (requiresZoom)
        {
            if (cameraController != null && cameraController.HasAdjustedCamera)
            {
                ToggleDrawer();
            }
            else
            {
                // 鏡頭未調整，無法切換場景
                Debug.Log("You need to zoom in before switching scenes!");
            }
        }
        else
        {
            ToggleDrawer(); // 當抽屜被點擊時，切換抽屜狀態
        }
    }

    void ToggleDrawer()
    {
        isOpened = !isOpened; // 切換抽屜狀態

        // 播放對應的音效
        if (audioSource != null)
        {
            if (isOpened && openSound != null)
            {
                audioSource.PlayOneShot(openSound);
            }
            else if (!isOpened && closeSound != null)
            {
                audioSource.PlayOneShot(closeSound);
            }
        }

        // 根據抽屜狀態，設置抽屜的位置
        if (isOpened)
        {
            transform.Translate(-200f, 0f, 0f); // 向前移動一定距離
        }
        else
        {
            transform.Translate(200f, 0f, 0f); // 向後移動一定距離
        }
    }
}
