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

    void Start()
    {
        // 獲取主相機上的 MouseLook 腳本
        cameraController = Camera.main.GetComponent<MouseLook>();
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
}
