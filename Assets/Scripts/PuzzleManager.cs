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
        //LoadPuzzleState();
    }


    //     private void LoadPuzzleState()
    // {
    //     foreach (Transform slot in puzzleSlots)
    //     {
    //         int savedItemId = PlayerPrefs.GetInt($"PuzzleSlot_{slot.GetInstanceID()}", 0);

    //         if (savedItemId != 0)
    //         {
    //             bool foundPiece = false;

    //             foreach (PuzzlePiece piece in FindObjectsOfType<PuzzlePiece>())
    //             {
    //                 if (piece.itemData != null && piece.itemData.id == savedItemId)
    //                 {
    //                     piece.PlacePiece(slot.position);
    //                     piece.transform.SetParent(slot);
    //                     ChangeSlotAppearance(slot);
    //                     Debug.Log($"Loaded PuzzlePiece {piece.itemData.itemName} in slot {slot.name}.");
    //                     foundPiece = true;
    //                     break;
    //                 }
    //             }

    //             if (!foundPiece)
    //             {
    //                 Debug.LogWarning($"No matching puzzle piece found for slot {slot.name} with saved ID {savedItemId}.");
    //             }
    //         }
    //     }
    // }

    // private void SavePuzzleState(PuzzlePiece piece, Transform slot)
    // {
    //     PlayerPrefs.SetInt($"PuzzleSlot_{slot.GetInstanceID()}", piece.itemData.id);
    //     PlayerPrefs.SetInt($"PuzzlePiece_{piece.itemData.id}", 1); // 標記此拼圖已放置
    //     PlayerPrefs.Save();
    //     Debug.Log($"Saved PuzzlePiece {piece.itemData.itemName} in slot {slot.name}.");
    // }
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
            return true;
        }
        else
        {
            puzzlePiece.ReturnToStartPosition();  // 如果沒放好，返回原位置
            return false;
        }
    }

    public bool CheckPuzzlePiecePlacement(PuzzlePiece puzzlePiece)
    {
        // if (puzzlePiece.itemData == null)
        // {
        //     Debug.LogError($"PuzzlePiece {puzzlePiece.name} has a null itemData.");
        //     return false;
        // }
        // Loop through each puzzle slot to see if the piece is close enough to snap
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
                {    puzzlePiece.PlacePiece(slot.position);
                    Debug.Log("Puzzle piece fits into slot");

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
            // if (slot.childCount == 0) // 檢查是否有空槽
            // {
            //     return;  // 如果有空槽，拼圖尚未完成
            // }
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
        // Example: DoorManager.Instance.OpenDoor();
    }

    // public Transform GetSlotForPiece(PuzzlePiece piece)
    // {
    //     foreach (Transform slot in puzzleSlots)
    //     {
    //         string slotKey = $"PuzzleSlot_{slot.GetInstanceID()}_Occupied";
    //         if (PlayerPrefs.GetInt(slotKey, 0) == 1)
    //         {
    //             return slot;
    //         }
    //     }
    //     return null;
    // }

}
