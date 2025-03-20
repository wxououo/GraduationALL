using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetController : MonoBehaviour
{

    private const string BoxLidStateKey = "BoxLidOpen"; // PlayerPrefs key for box lid state

    public void ResetGame()
    {
        PlayerPrefs.SetInt("IsUnlocked", 0);
        PlayerPrefs.SetInt("IsTVPlayed", 0);
        PlayerPrefs.SetInt(BoxLidStateKey, 0);
        ResetInventory();

        foreach (var itemPickup in FindObjectsOfType<ItemPickup>())
        {
            itemPickup.ResetItem(); // 回到初始狀態
        }


        ResetPuzzlePieces();
        ResetPuzzleSlots();

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

    private void ResetPuzzlePieces()
    {
        // Find all puzzle pieces in the scene
        PuzzlePiece[] puzzlePieces = FindObjectsOfType<PuzzlePiece>();

        foreach (PuzzlePiece piece in puzzlePieces)
        {
            // 直接回到場景中的起始位置
            piece.ReturnToStartPosition();
        }
    }
    private void ResetPuzzleSlots()
    {
        // Find all puzzle slots in the scene
        PuzzleSlot[] puzzleSlots = FindObjectsOfType<PuzzleSlot>();

        foreach (PuzzleSlot slot in puzzleSlots)
        {
            // Reset slot to grayscale and mark as unoccupied
            slot.SetToGrayscale();

            // Remove any child puzzle pieces
            foreach (Transform child in slot.transform)
            {
                if (child.GetComponent<PuzzlePiece>() != null)
                {
                    Destroy(child.gameObject);
                }
            }

            // Update the slot's occupied state
            slot.SetToOccupied(false);

            // Clear any saved slot states
            PlayerPrefs.DeleteKey($"Slot_{slot.slotID}_Occupied");
        }

        // 清除拼圖狀態
        PlayerPrefs.DeleteKey("PuzzleCompleted");
    }
}
