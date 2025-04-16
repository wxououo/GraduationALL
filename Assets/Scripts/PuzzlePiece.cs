using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using UnityEngine.UI;

public enum PieceType
{
    Puzzle,
    PhotoAlbum
}

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public PieceType pieceType;  // 物件類型 (Puzzle or PhotoAlbum)
    private Vector3 startPosition;
    private Transform originalParent;
    private Camera playerCamera;
    public Item itemData;
    public GameObject targetObject;


    private int puzzleSlotLayer;

    public bool isPlacedCorrectly = false;
    private void Start()
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        playerCamera = Camera.main;
        puzzleSlotLayer = LayerMask.NameToLayer("PuzzleSlot");
        if (itemData == null)
        {
            TryFindItemData();
        }
        LoadPieceState();
    }
    public void PlacePiece(Vector3 targetPosition)
    {
        transform.position = targetPosition;
        GetComponent<Collider>().enabled = true;
        SavePieceState();

    }

    public void SetItemData(Item item)
    {
        if (item != null)
        {
            itemData = item;
            gameObject.name = item.itemName;
            //Debug.Log($"Item data set: {item.itemName}");
        }
        else
        {
            Debug.LogError("Attempting to set null item data");
        }
    }

    private void TryFindItemData()
    {
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("InventoryManager.Instance is null!");
            return;
        }

        string originalItemName = gameObject.name.Replace("(Clone)", "");

        Item foundItem = InventoryManager.Instance.allItems.Find(item =>
            item.itemName == originalItemName ||
            originalItemName.Contains(item.itemName)
        );

        if (foundItem != null)
        {
            itemData = foundItem;
            Debug.Log($"Successfully found item data: {itemData.itemName}");
        }
        else
        {
            // 如果找不到，打印所有可用的物品名稱，幫助診斷
            string availableItemNames = string.Join(", ",
                InventoryManager.Instance.allItems.Select(item => item.itemName));
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (itemData == null)
        {
            TryFindItemData();
        }
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(null);  // 讓物件不再依附於 Canvas，讓它可以移動到 3D 場景中
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point; // 物件跟隨射線的碰撞點移動
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = true; // 重新開啟交互

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (itemData == null)
        {
            ReturnToStartPosition();
            return;
        }

        if (pieceType == PieceType.Puzzle)
        {
            if (PuzzleManager.Instance == null)
            {
                Debug.LogError("PuzzleManager.Instance is null!");
                ReturnToStartPosition();
                return;
            }

            Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2.0f);
            bool placed = false;

            foreach (Collider hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Puzzle"))
                {
                    PuzzleSlot slot = hitCollider.GetComponent<PuzzleSlot>();
                    if (slot != null && !slot.IsOccupied() && slot.IsWithinPlacementZone(transform.position))
                    {
                        bool isPlacedCorrectly = PuzzleManager.Instance.PlacePuzzlePiece(this);
                        if (isPlacedCorrectly)
                        {
                            placed = true;
                            if (InventoryManager.Instance != null)
                            {
                                InventoryManager.Instance.Remove(itemData);
                                InventoryManager.Instance.ListItems();
                            }
                            break;
                        }
                    }
                }
            }
            if (!placed)
            {
                ReturnToStartPosition();
            }
        }
        else if (pieceType == PieceType.PhotoAlbum)
        {
            if (PhotoAlbumManager.Instance == null)
            {
                Debug.LogError("PhotoAlbumManager.Instance is null!");
                ReturnToStartPosition();
                return;
            }
            Debug.Log($"Mouse Position: {Input.mousePosition}");
            // 使用 GraphicRaycaster 對 UI 元素做射線
            PointerEventData pointerData = new PointerEventData(EventSystem.current)
            {
                position = Input.mousePosition
            };

            List<RaycastResult> results = new List<RaycastResult>();
            GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();

            if (raycaster != null)
            {
                raycaster.Raycast(pointerData, results);
                Debug.Log($"[DEBUG] GraphicRaycast 命中數量：{results.Count}");

                foreach (RaycastResult result in results)
                {
                    string objName = result.gameObject.name;
                    int objLayer = result.gameObject.layer;
                    bool isRaycastTarget = result.gameObject.TryGetComponent<Graphic>(out var graphic) && graphic.raycastTarget;

                    Debug.Log($"[DEBUG] 命中物件：{objName}, Layer: {objLayer}, RaycastTarget: {isRaycastTarget}");
                }
            }
            else
            {
                Debug.LogError("[DEBUG] 找不到 GraphicRaycaster！");
            }


            foreach (RaycastResult result in results)
            {
                Debug.Log($"GraphicRaycast hit: {result.gameObject.name}");
                PhotoSlot slot = result.gameObject.GetComponentInParent<PhotoSlot>();

                if (slot != null)
                {
                    Debug.Log("找到 PhotoSlot！");

                    bool isOccupied = slot.IsOccupied();
                    bool isWithinZone = slot.IsWithinPlacementZone(transform.position);
                    bool isValidPiece = slot.IsValidForPiece(this);

                    Debug.Log($" Slot 檢查：Occupied={isOccupied}, WithinZone={isWithinZone}, ValidPiece={isValidPiece}");

                    if (!isOccupied && isWithinZone && isValidPiece)
                    {
                        bool placed = PhotoAlbumManager.Instance.PlacePhotoPiece(this, slot);
                        if (placed)
                        {
                            Debug.Log("照片放置成功！");
                            if (InventoryManager.Instance != null)
                            {
                                InventoryManager.Instance.Remove(itemData);
                                InventoryManager.Instance.ListItems();
                            }
                            return;
                        }
                    }
                }
            }

            Debug.Log("沒有合適的照片槽位，物件退回原位");
            ReturnToStartPosition();
    }

}
    public void ResetPiece()
    {
        transform.position = startPosition; // 恢復到初始位置
        transform.SetParent(originalParent); // 恢復原始父物件
        isPlacedCorrectly = false; // 清除正確放置標記
        SavePieceState();
    }
    private void SavePieceState()
    {
        PlayerPrefs.SetInt($"PuzzlePiece_{itemData.id}_Placed", isPlacedCorrectly ? 1 : 0);
        PlayerPrefs.Save();
    }

    private void LoadPieceState()
    {
        if (PlayerPrefs.GetInt($"PuzzlePiece_{itemData.id}_Placed", 0) == 1)
        {
            gameObject.SetActive(false);
        }
        else
        {
            ResetPiece();
        }
    }

    private void HandleSuccessfulPlacement()
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

        if (targetObject != null)
        {
            targetObject.SetActive(true);  // 顯示目標物件
        }
    }

    public void ReturnToStartPosition()
    {
        transform.position = startPosition; // Return to original scene position
        transform.SetParent(originalParent);
        // 如果返回到UI中，重置RectTransform属性
        if (originalParent != null && originalParent.GetComponent<RectTransform>() != null)
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                rectTransform.localScale = Vector3.one;
            }
        }
    }
}
