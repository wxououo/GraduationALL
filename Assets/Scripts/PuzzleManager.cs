using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager Instance;

    public List<Transform> puzzleSlots;  // List of puzzle slots
    public float snapThreshold = 2f;     // Distance for snapping puzzle pieces to slots
    private bool isPuzzleCompleted = false;

    private void Awake()
    {
        Instance = this;
        LoadPuzzleState();
    }

    private void LoadPuzzleState()
    {

        // 遍歷所有拼圖插槽
        foreach (Transform slot in puzzleSlots)
        {
            if (slot == null) continue;

            PuzzleSlot slotComponent = slot.GetComponent<PuzzleSlot>();
            if (slotComponent == null)
            {
                Debug.LogError($"No PuzzleSlot component found on {slot.name}");
                continue;
            }
            slotComponent.EnsureInitialized();
            // 檢查這個插槽是否在之前已被佔據
            bool wasOccupied = PlayerPrefs.GetInt($"Slot_{slotComponent.slotID}_Occupied", 0) == 1;

            if (wasOccupied)
            {
                // 找到對應ID的拼圖碎片
                PuzzlePiece matchingPiece = FindMatchingPuzzlePiece(slotComponent.slotID);

                if (matchingPiece != null)
                {
                    // 將拼圖碎片放置到插槽
                    matchingPiece.PlacePiece(slot.position);
                    matchingPiece.transform.SetParent(slot);
                    slotComponent.SetToOccupied(true);
                    ChangeSlotAppearance(slot); // 變成彩色

                    // 從物品欄移除
                    if (InventoryManager.Instance != null)
                    {
                        InventoryManager.Instance.Remove(matchingPiece.itemData);
                    }
                }
            }
        }
    }

    private PuzzlePiece FindMatchingPuzzlePiece(int slotID)
    {
        // 尋找匹配插槽ID的拼圖碎片
        PuzzlePiece[] allPieces = FindObjectsOfType<PuzzlePiece>();
        foreach (PuzzlePiece piece in allPieces)
        {
            if (piece.itemData != null && piece.itemData.id == slotID)
            {
                return piece;
            }
        }
        return null;
    }
    public bool PlacePuzzlePiece(PuzzlePiece puzzlePiece)
    {
        if (puzzlePiece == null)
        {
            Debug.LogError("PlacePuzzlePiece received null PuzzlePiece!");
            return false;
        }

        Debug.Log($"Attempting to place puzzle piece: {puzzlePiece.name}");
        // 檢查拼圖是否可以被放置在槽位中
        if (CheckPuzzlePiecePlacement(puzzlePiece))
        {
            SaveSlotState();
            return true;
        }
        else
        {
            puzzlePiece.ReturnToStartPosition();  // 如果沒放好，返回原位置
            return false;
        }
    }
    private void SaveSlotState()
    {
        // 遍歷所有插槽並保存其佔據狀態
        foreach (Transform slot in puzzleSlots)
        {
            PuzzleSlot slotComponent = slot.GetComponent<PuzzleSlot>();
            if (slotComponent != null)
            {
                // 保存插槽是否被佔據的狀態
                PlayerPrefs.SetInt($"Slot_{slotComponent.slotID}_Occupied", slotComponent.IsOccupied() ? 1 : 0);
            }
        }
        PlayerPrefs.Save();
    }
    public bool CheckPuzzlePiecePlacement(PuzzlePiece puzzlePiece)
    {
        foreach (Transform slot in puzzleSlots)
        {

            PuzzleSlot slotComponent = slot.GetComponent<PuzzleSlot>();

            if (slotComponent == null)
            {
                Debug.LogError($"Slot {slot.name} is missing PuzzleSlot component");
                continue;
            }

            // 使用更精確的距離計算
            float distance = Vector3.Distance(puzzlePiece.transform.position, slot.position);


            if (distance < snapThreshold)
            {
                Debug.Log($"Checking placement - SlotID: {slotComponent.slotID}");
                if (slotComponent == null)
                {
                    Debug.LogError($"Slot {slot.name} is missing a PuzzleSlot component.");
                    continue;
                }

                Debug.Log($"Comparing SlotID: {slotComponent.slotID} ");

                // 確認該拼圖槽是否已經有拼圖碎片
                if (slotComponent.IsOccupied())
                {
                    Debug.Log("Puzzle slot already occupied.");
                    return false; // 如果槽已經被佔據，不允許放置
                }
                if (slotComponent.slotID == puzzlePiece.itemData.id)
                {
                    puzzlePiece.PlacePiece(slot.position);
                    //Debug.Log("Puzzle piece fits into slot");

                    puzzlePiece.transform.SetParent(slot);
                    slotComponent.SetToOccupied(true);
                    // 保存狀態
                    //SavePuzzleState(puzzlePiece, slot);

                    ChangeSlotAppearance(slot);

                    CheckPuzzleCompletion();
                    return true;
                }
            }
        }

        return false;
    }

    private void ChangeSlotAppearance(Transform slot)
    {
        PuzzleSlot imageComponent = slot.GetComponent<PuzzleSlot>();
        if (imageComponent != null)
        {
            imageComponent.SetToColor();  // Calls the method to switch to the colored appearance
        }
    }

    private void CheckPuzzleCompletion()
    {
        foreach (Transform slot in puzzleSlots)
        {
            PuzzleSlot slotComponent = slot.GetComponent<PuzzleSlot>();
            if (!slotComponent.IsOccupied()) // 檢查是否有空槽
            {
                return;  // 如果有空槽，拼圖尚未完成
            }

            PuzzlePiece placedPiece = slot.GetComponentInChildren<PuzzlePiece>();

        }

        isPuzzleCompleted = true;
        Debug.Log("Puzzle completed successfully!");
        // 觸發拼圖完成後的事件，例如開門或顯示成功訊息
        OnPuzzleCompleted();
    }

    private void OnPuzzleCompleted()
    {
        // 拼圖完成後觸發的邏輯，可以是動畫、開門等
        Debug.Log("Trigger puzzle completion event.");
    }

}
