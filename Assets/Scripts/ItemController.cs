using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    public GameObject itemToAppear; // 要顯示的物品
    void Start()
    {
        int unlockState = PlayerPrefs.GetInt("IsUnlocked", 0);
        //Debug.Log("當前解鎖狀態：" + unlockState);

        if (unlockState == 1)
            {
                //Debug.Log("顯示物品");
                itemToAppear.SetActive(true); // 顯示物品
            }
        else
        {
            //Debug.Log("隱藏物品");
            itemToAppear.SetActive(false); // 隱藏物品
        }
    }

    
}
