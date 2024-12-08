using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private Dictionary<GameObject, Item> objectItemMap = new Dictionary<GameObject, Item>();
    //private Dictionary<int, PuzzlePiece> puzzlePieces = new Dictionary<int, PuzzlePiece>();

    public List<Item> Items = new List<Item>();

    public Transform ItemContent;
    public GameObject InventoryItem;

    public Transform spawnPoint; // 物品生成的位置

    public Camera playerCamera;  // 玩家攝影機
    public float spawnDistance = 100f; // 物品與攝影機的距離

    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public Button takeOutButton;
    public List<Item> allItems;

    private void Awake()
    {

        Instance = this;

    }

    private void OnEnable()
    {
        LoadInventory();  // 場景加載時自動加載道具
    }

    public void Add(Item item)
    {
        Items.Add(item);
        ListItems();
        SaveInventory();
    }

    public void Remove(Item item)
    {
        Items.Remove(item);

        SaveInventory();
    }

    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }

        foreach (var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);

            var itemName = obj.transform.Find("ItemName").GetComponent<TextMeshProUGUI>();
            RectTransform nameRect = itemName.GetComponent<RectTransform>();
            //nameRect.anchoredPosition = new Vector2(40, -80);
            var itemIcon = obj.transform.Find("ItemIcon").GetComponent<Image>();
            var takeOutButton = obj.transform.Find("TakeOutButton").GetComponent<Button>();
            CanvasGroup buttonCanvasGroup = takeOutButton.gameObject.GetComponent<CanvasGroup>();
            if (buttonCanvasGroup == null)
            {
                buttonCanvasGroup = takeOutButton.gameObject.AddComponent<CanvasGroup>();
            }
            buttonCanvasGroup.alpha = 0; // 使按鈕完全透明
            buttonCanvasGroup.interactable = true; // 確保按鈕仍然可互動
            buttonCanvasGroup.blocksRaycasts = true; // 確保仍然可以點擊

            itemName.text = item.itemName;
            itemIcon.sprite = item.icon;

            // 當點擊取出按鈕時，生成該物品
            takeOutButton.onClick.RemoveAllListeners();
            takeOutButton.onClick.AddListener(() => TakeOutItem(item));

            AddDragFunctionality(obj, item);

        }
    }
    private void AddDragFunctionality(GameObject obj, Item item)
    {
        EventTrigger eventTrigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();

        AddEvent(eventTrigger, EventTriggerType.BeginDrag, (data) => OnBeginDrag(obj));
        AddEvent(eventTrigger, EventTriggerType.Drag, (data) => OnDrag(obj));
        AddEvent(eventTrigger, EventTriggerType.EndDrag, (data) => OnEndDrag(obj, item));

        PuzzlePiece puzzlePiece = obj.GetComponent<PuzzlePiece>() ?? obj.AddComponent<PuzzlePiece>();
        puzzlePiece.SetItemData(item);
    }

    private void AddEvent(EventTrigger trigger, EventTriggerType type, System.Action<BaseEventData> action)
    {
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener((data) => action(data));
        trigger.triggers.Add(entry);
    }
    private void OnBeginDrag(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false; // Allow dragging through UI elements
        obj.transform.SetParent(null);
    }
    private void OnDrag(GameObject obj)
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // Update the object's position to the hit point in world space
            obj.transform.position = hit.point;
        }
    }
    private void OnEndDrag(GameObject obj, Item item)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = true; // 允許拖曳穿過其他 UI 元素

        // 檢測是否與其他物件發生互動
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log($"Hit object: {hit.collider.gameObject.name}");
            if (hit.collider.CompareTag("Puzzle")) // Replace with your correct slot tag
            {
                Debug.Log("Puzzle slot detected.");
                // Snap the object into place and finalize placement
                obj.transform.position = hit.collider.transform.position;

                PuzzlePiece puzzlePiece = obj.GetComponent<PuzzlePiece>();

                if (puzzlePiece != null)
                {
                    // Attempt to place the piece; if correctly placed, remove from inventory
                    if (PuzzleManager.Instance.PlacePuzzlePiece(puzzlePiece))
                    {
                        Debug.Log("Puzzle piece correct");
                        Remove(item); // Remove from inventory if placed correctly
                        ListItems();   // Refresh the inventory display
                        return;
                    }
                    else
                    {
                        Debug.Log($"Puzzle piece not placed correctly, returning to inventory.");
                        ReturnToInventory(obj, item); // Return to inventory if placement was invalid
                    }
                }

            }
            else if (hit.collider.CompareTag("Interactable"))
            {
                Debug.Log("Interactable detected.");
                InteractableManager.Instance.HandleInteraction(item, hit.collider.gameObject);

                // 將物品從道具欄移除
                Remove(item);
                ListItems();  // 更新道具欄顯示
            }
            else
            {
                Debug.Log("No valid slot detected, returning to inventory.");
                // Return the object back to the inventory
                ReturnToInventory(obj, item);
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything, returning to inventory.");
            ReturnToInventory(obj, item); // Return to inventory if no valid slot was detected
        }
        Remove(item);  // Remove from the inventory
        ListItems();   // Refresh the UI
    }
    // void TakeOutItem(Item item)
    // {

    //     SetSpawnPointAtCameraCenter(); // 設置 spawnPoint 在攝影機前方
    //     if (item.prefab != null)
    //     {
    //         Instantiate(item.prefab, spawnPoint.position, spawnPoint.rotation); // 生成 prefab
    //         Remove(item);  // 從道具欄移除該物品
    //         ListItems();  // 更新道具欄介面
    //     }
    // }

    public void TakeOutItem(Item item)
    {
        SetSpawnPointAtCameraCenter(); // 設定物品生成點

        if (item.prefab != null)
        {
            // 生成物件
            GameObject spawnedItem = Instantiate(item.prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedItem.name = item.itemName;

            PuzzlePiece puzzlePiece = spawnedItem.GetComponent<PuzzlePiece>();
            if (puzzlePiece == null)
            {
                puzzlePiece = spawnedItem.AddComponent<PuzzlePiece>();
            }

            // 從道具欄中移除
            Remove(item);
            ListItems();
        }
    }
    void SetSpawnPointAtCameraCenter()
    {
        // 將 spawnPoint 設置在攝影機正前方的某個距離
        if (playerCamera != null && spawnPoint != null)
        {
            spawnPoint.position = playerCamera.transform.position + playerCamera.transform.forward * spawnDistance;
            spawnPoint.rotation = Quaternion.LookRotation(playerCamera.transform.forward); // 朝向與攝影機一致
        }
    }


    public void RegisterPuzzlePiece(GameObject obj, Item item)
    {
        // Add the GameObject and its associated Item to the dictionary
        if (!objectItemMap.ContainsKey(obj))
        {
            objectItemMap.Add(obj, item);
        }
    }

    public void ReturnToInventory(GameObject obj, Item item)
    {
        obj.transform.SetParent(ItemContent);  // Make it a child of the inventory content panel
        obj.transform.localPosition = Vector3.zero;  // Reset its local position

        // Optionally reset the scale and rotation if needed
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;

        // Re-add the item to the inventory system
        Add(item); // Add back to inventory list
        ListItems();
    }

    public void SaveInventory()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            PlayerPrefs.SetInt("Item_" + i, Items[i].id);
            // 根據需求儲存更多道具屬性，例如 icon, quantity 等等
        }
        PlayerPrefs.SetInt("ItemCount", Items.Count); // 保存道具數量
        PlayerPrefs.Save();
    }
    public void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("ItemCount", 0); // 獲取保存的道具數量
        Items.Clear(); // 清除當前的道具清單

        for (int i = 0; i < itemCount; i++)
        {
            int itemID = PlayerPrefs.GetInt("Item_" + i);
            // 假設這裡有一個方法可以根據名稱找到對應的 Item，可能是一個道具資料庫
            Item newItem = FindItemByID(itemID); // 需要你自行實現這個方法
            if (newItem != null)
            {
                Items.Add(newItem);
            }
            else
            {
                Debug.LogError($"Missing item data for ID {itemID}. Ensure the item exists in allItems.");
            }
        }

        ListItems(); // 更新道具介面顯示
    }

    // public PuzzlePiece GetPuzzlePieceById(int id)
    // {
    //     if (puzzlePieces.TryGetValue(id, out PuzzlePiece puzzlePiece))
    //     {
    //         return puzzlePiece;
    //     }
    //     Debug.LogWarning($"PuzzlePiece with ID {id} not found.");
    //     return null;
    // }
    public Item FindItemByID(int id)
    {
        return allItems.Find(item => item.id == id);
    }
}
