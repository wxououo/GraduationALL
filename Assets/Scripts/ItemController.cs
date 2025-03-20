using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemToAppear; // 要顯示的物品
    public Item linkedItem;         // 關聯的 Item

    void Start()
    {
        int unlockState = PlayerPrefs.GetInt("IsUnlocked", 0);

        // 檢查道具欄是否已有此物品
        bool isInInventory = InventoryManager.Instance != null && InventoryManager.Instance.Items.Contains(linkedItem);
        // 檢查 TV 是否已播放過
        bool isTVPlayed = PlayerPrefs.GetInt("IsTVPlayed", 0) == 1;


        // 如果物品已解鎖，且不在道具欄中，且 TV 未播放過，才顯示
        if (unlockState == 1 && !isInInventory && !isTVPlayed)
        {
            itemToAppear.SetActive(true); // 顯示物品
        }
        else
        {
            itemToAppear.SetActive(false); // 隱藏物品
        }

        PlayerPrefs.Save();
    }
    public void MarkTVAsPlayed()
    {
        PlayerPrefs.SetInt("IsTVPlayed", 1);
        PlayerPrefs.Save();
    }

}
