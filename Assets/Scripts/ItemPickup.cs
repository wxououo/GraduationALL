using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public Item Item;
    public string pickupKey;
    private bool canBePickedUp = false; // 控制是否可以拾取
    public bool requiresZoom = false; // 是否需要鏡頭放大才能切換場景
    private MouseLook cameraController; // 用來檢查鏡頭是否已調整
    public GameObject itemToDisplayOnPickup; // Camera

    [Header("音效設定")]
    [SerializeField] private AudioClip pickupSound; // 撿取道具的音效
    private AudioSource audioSource; // 音效來源
    private bool isHolding = false; // 追蹤是否正在按住

    void Start()
    {
        // 獲取主相機上的 MouseLook 腳本
        cameraController = Camera.main.GetComponent<MouseLook>();
        // 獲取或添加 AudioSource 組件
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            Debug.Log("已添加 AudioSource 組件");
        }

        // 詳細檢查音效設定
        if (pickupSound == null)
        {
            Debug.LogWarning($"警告：未設定撿取音效！物件名稱：{gameObject.name}");
        }
        else
        {
            Debug.Log($"音效已設定：{pickupSound.name}");
            // 設定 AudioSource 的基本屬性
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 2D 音效
            audioSource.volume = 1f;
            audioSource.priority = 0; // 最高優先級
            audioSource.bypassEffects = true; // 跳過音效處理
            audioSource.bypassListenerEffects = true; // 跳過監聽器效果
            audioSource.bypassReverbZones = true; // 跳過混響區域
        }
    }
    public void UnlockItem()
    {
        // 在谜题完成后被调用，让道具可以拾取
        canBePickedUp = true;
        gameObject.SetActive(true); // 显示道具
    }
    public void ResetItem()
    {
        bool shouldBeVisible = PlayerPrefs.GetInt("IsUnlocked", 0) == 0; // 依據遊戲狀態設置顯示
        gameObject.SetActive(shouldBeVisible);
        Debug.Log($"ResetItem: {gameObject.name} visibility set to {shouldBeVisible}");
    }
    void Pickup()
    {
        // 播放撿取音效
        if (pickupSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        if (itemToDisplayOnPickup != null)
        {
            itemToDisplayOnPickup.SetActive(true);
            gameObject.SetActive(false);
        }
        // 檢查道具是否已經在道具欄中
        if (InventoryManager.Instance.Items.Contains(Item))
        {
            Debug.Log("Item already in inventory, no need to pick up again.");
            return;  // 如果物品已經存在於道具欄，則不進行撿取
        }

        // 將物品加入道具欄並摧毀遊戲物件
        if (Item != null && itemToDisplayOnPickup == null)
        {
            // 將物品加入道具欄並摧毀遊戲物件
            InventoryManager.Instance.Add(Item);
            PlayerPrefs.SetInt(pickupKey, 1);
            PlayerPrefs.Save();
            //Destroy(gameObject);
            gameObject.SetActive(false);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"點擊道具：{gameObject.name}");
        isHolding = true;

        // 開始播放音效
        if (pickupSound != null && audioSource != null)
        {
            Debug.Log($"開始播放音效：{pickupSound.name}");
            audioSource.clip = pickupSound;
            audioSource.loop = true; // 設定為循環播放
            audioSource.Play();
        }
        if (requiresZoom)
        {
            if (cameraController != null && cameraController.HasAdjustedCamera)
            {
                Pickup();
            }
            else
            {
                // 鏡頭未調整，無法切換場景
                Debug.Log("You need to zoom in before switching scenes!");
            }
        }
        else
        {
            // 不需要鏡頭放大，直接切換場景
            Pickup();
        }
    }
    private void OnMouseUp()
    {
        Debug.Log($"放開道具：{gameObject.name}");
        isHolding = false;

        // 停止音效播放
        if (audioSource != null)
        {
            audioSource.loop = false; // 取消循環播放
            audioSource.Stop();
        }
    }
}
