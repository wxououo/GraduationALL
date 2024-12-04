using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition; // Original position to return to if not placed correctly
    private Transform originalParent;
    private Camera playerCamera; // 攝影機，用於發射 Raycast
    public Item itemData;
    //public bool isPlacedCorrectly = false;

    public void Initialize(Item item)
    {
        if (item == null)
        {
            string pieceName = gameObject.name;
            Debug.Log($"Attempting to find item for piece name: {pieceName}");
            // First, try finding by name
            item = InventoryManager.Instance.allItems.Find(i => i.itemName == pieceName);
            if (item == null)
            {
                // Try extracting an ID from the name
                int.TryParse(pieceName.Replace("Wedding_", ""), out int itemId);
                item = InventoryManager.Instance.FindItemByID(itemId);
            }

            // If still not found, provide a default item
            if (item == null)
            {
                Debug.LogError($"Cannot initialize PuzzlePiece: Absolute item data failure for {gameObject.name}");
                item = new Item { itemName = "Default", id = -1 };
            }
        }
        if (item == null)
        {
            Debug.LogError($"Cannot initialize PuzzlePiece: Absolute item data failure for {gameObject.name}");

            return;
        }
        itemData = item;
        gameObject.name = $"Wedding_{item.itemName}";
        Debug.Log($"Successfully initialized PuzzlePiece with Item: {item.itemName}");
    }
    private int ExtractItemIdFromName()
    {
        // Implement a method to extract item ID from the piece's name or other identifier
        // For example:
        string pieceName = gameObject.name;
        int itemId;
        if (int.TryParse(pieceName.Replace("PuzzlePiece_", ""), out itemId))
        {
            return itemId;
        }
        return -1;
    }
    // public string GetItemName()
    // {
    //     return itemData != null ? itemData.itemName : "Unknown";
    // }

    // public Item GetItemData()
    // {
    //     return itemData;
    // }

    private void Start()
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        playerCamera = Camera.main; // 找到主攝影機
        //InventoryManager.Instance.RegisterPuzzlePiece(this.gameObject, itemData);
        if (itemData == null)
        {
            Initialize(null);
        }
        // if (itemData == null)
        // {
        //     string pieceName = gameObject.name;
        //     int itemId;
        //     if (int.TryParse(pieceName.Replace("PuzzlePiece_", ""), out itemId))
        //     {
        //         itemData = InventoryManager.Instance.FindItemByID(itemId);
        //     }

        //     if (itemData == null)
        //     {
        //         Debug.LogError($"Cannot find item data for {gameObject.name}");
        //     }
        // }
    }

    // private void PlacePieceAtSavedPosition()
    // {
    //     foreach (Transform slot in PuzzleManager.Instance.puzzleSlots)
    //     {
    //         if (PlayerPrefs.GetInt($"PuzzleSlot_{slot.GetInstanceID()}", 0) == itemData.id)
    //         {
    //             PlacePiece(slot.position);
    //             transform.SetParent(slot);
    //             isPlacedCorrectly = true;
    //             break;
    //         }
    //     }
    // }
    public void OnBeginDrag(PointerEventData eventData)
    {
        //if (isPlacedCorrectly) return;
        GetComponent<CanvasGroup>().blocksRaycasts = false; // Allows dragging through UI
        transform.SetParent(null);  // 讓拼圖碎片不再隸屬 Canvas，使其能夠移動到 3D 場景中
    }

    public void OnDrag(PointerEventData eventData)
    {
        //if (isPlacedCorrectly) return;
        // 將鼠標的屏幕坐標轉換為場景中的 3D 坐標
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // 如果 Raycast 碰到 3D 物體，則讓拼圖碎片跟隨
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;  // 讓拼圖碎片跟隨 Raycast 碰撞點移動
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        GetComponent<CanvasGroup>().blocksRaycasts = true; // Re-enable interaction
        if (PuzzleManager.Instance == null)
        {
            Debug.LogError("PuzzleManager.Instance is null!");
            ReturnToStartPosition();
            return;
        }
        if (itemData == null)
        {
            Debug.LogError("itemData is null for this puzzle piece!");
            ReturnToStartPosition();
            return;
        }
        // if (itemData == null)
        // {
        //     Debug.LogError($"OnEndDrag: {gameObject.name} has no item data");
        //     ReturnToStartPosition();
        //     return;
        // }
        // bool isPlacedCorrectly = PuzzleManager.Instance.PlacePuzzlePiece(this);
        // if (!isPlacedCorrectly)
        // {
        //     ReturnToStartPosition();
        // }
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit) && hit.collider.CompareTag("Puzzle"))
        {
            bool isPlacedCorrectly = PuzzleManager.Instance.PlacePuzzlePiece(this);

            if (isPlacedCorrectly)
            {
                if (InventoryManager.Instance != null)
                {
                    InventoryManager.Instance.Remove(itemData);
                    InventoryManager.Instance.ListItems();
                }
                else
                {
                    Debug.LogError("InventoryManager.Instance is null!");
                }
            }
            else
            {
                ReturnToStartPosition();
            }
        }
        else
        {
            ReturnToStartPosition();
        }
    }

    // Method to snap the piece into the correct slot
    public void PlacePiece(Vector3 targetPosition)
    {
        // if (itemData == null)
        // {
        //     Debug.LogError($"Attempting to place piece {gameObject.name} with null item data");
        //     return;
        // }
        transform.position = targetPosition;
        //     if (originalParent != null)
        // {
        //     transform.SetParent(originalParent);
        // }
        // else
        // {
        //     Debug.LogWarning($"OriginalParent is null for {name}. Ensure it is set correctly.");
        // }
        //isPlacedCorrectly = true;
        GetComponent<CanvasGroup>().blocksRaycasts = true; // Prevent further dragging
        //PlayerPrefs.SetInt($"PuzzlePiece_{itemData.id}_Placed", 1);
        //PlayerPrefs.SetInt($"PuzzleSlot_{transform.parent.GetInstanceID()}", itemData.id);
        //PlayerPrefs.Save();
    }

    public void ResetPiece()
    {
        transform.position = startPosition; // 恢復到初始位置
        transform.SetParent(originalParent); // 恢復原始父物件
        //isPlacedCorrectly = false; // 清除正確放置標記
        //GetComponent<CanvasGroup>().blocksRaycasts = true; // 允許再次拖動
        //PlayerPrefs.SetInt($"PuzzlePiece_{itemData.id}_Placed", 0);
        PlayerPrefs.Save();
    }
    // Method to return the piece to its original position if not placed correctly
    public void ReturnToStartPosition()
    {

        transform.position = startPosition; // Return to original scene position

    }
}
