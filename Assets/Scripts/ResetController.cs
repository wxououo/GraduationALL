using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetController : MonoBehaviour
{

    private const string BoxLidStateKey = "BoxLidOpen"; // PlayerPrefs key for box lid state

    public void ResetGame()
    {
        PlayerPrefs.SetInt("IsUnlocked", 0);
        PlayerPrefs.SetInt(BoxLidStateKey, 0);
        ResetInventory();
        
        foreach (var itemPickup in FindObjectsOfType<ItemPickup>())
        {
            itemPickup.ResetItem(); // 回到初始狀態
        }

        foreach (var puzzlePiece in FindObjectsOfType<PuzzlePiece>())
        {
            puzzlePiece.ResetPiece();
        }

        // 重置箱子蓋子
        foreach (var boxLid in FindObjectsOfType<BoxLidController>())
        {
            boxLid.ResetLidState(); // 假設 ResetLidState 是控制箱子蓋子重置的方法
        }
        PlayerPrefs.Save();
        Debug.Log("遊戲已重置！");
    }


    private void ResetInventory()
    {
        int itemCount = PlayerPrefs.GetInt("ItemCount", 0);

        for (int i = 0; i < itemCount; i++)
        {
            PlayerPrefs.DeleteKey("Item_" + i);
        }

        PlayerPrefs.DeleteKey("ItemCount");
    }

}
