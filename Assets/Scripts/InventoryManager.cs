using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    private Dictionary<GameObject, Item> objectItemMap = new Dictionary<GameObject, Item>();
    private List<GameObject> inventoryUIItems = new List<GameObject>(); // 改為追蹤UI物件

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
    public TextMeshProUGUI itemDescription;

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
        StartCoroutine(UpdateInventoryUI());
        itemDescription.text = "";
        SaveInventory();
    }
    private IEnumerator UpdateInventoryUI()
    {
        yield return null;
        ListItems();
    }
    public void ListItems()
    {
        foreach (Transform item in ItemContent)
        {
            Destroy(item.gameObject);
        }
        objectItemMap.Clear();
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
            itemDescription.text = "";


            // 當點擊取出按鈕時，生成該物品
            takeOutButton.onClick.RemoveAllListeners();
            takeOutButton.onClick.RemoveAllListeners();
            takeOutButton.onClick.AddListener(() => TakeOutItem(item));
            objectItemMap[obj] = item;
            AddDragFunctionality(obj, item);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(ItemContent.GetComponent<RectTransform>());
    }
    private void AddDragFunctionality(GameObject obj, Item item)
    {
        EventTrigger eventTrigger = obj.GetComponent<EventTrigger>() ?? obj.AddComponent<EventTrigger>();

        AddEvent(eventTrigger, EventTriggerType.BeginDrag, (data) => OnBeginDrag(obj));
        AddEvent(eventTrigger, EventTriggerType.Drag, (data) => OnDrag(obj));
        AddEvent(eventTrigger, EventTriggerType.EndDrag, (data) => OnEndDrag(obj, item));

        PuzzlePiece puzzlePiece = obj.GetComponent<PuzzlePiece>() ?? obj.AddComponent<PuzzlePiece>();
        puzzlePiece.SetItemData(item);

        // 新增這段：根據 item 判斷是拼圖或相片
        if (item.isPuzzlePiece)
        {
            puzzlePiece.pieceType = PieceType.Puzzle;
        }
        else
        {
            puzzlePiece.pieceType = PieceType.PhotoAlbum;
        }

        //Debug.Log($"[AddDragFunctionality] {item.itemName} isPuzzlePiece: {item.isPuzzlePiece}, assigned pieceType: {puzzlePiece.pieceType}");
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
        else
        {
            obj.transform.position = playerCamera.transform.position + ray.direction * 2f;
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

        Collider[] hitColliders = Physics.OverlapSphere(obj.transform.position, 2.0f);
        bool validPlacement = false;

        foreach (Collider hitCollider in hitColliders)
        {
            Debug.Log($"Hit object: {hitCollider.gameObject.name}");
            if (hitCollider.CompareTag("Puzzle")) // Replace with your correct slot tag
            {
                obj.transform.position = hitCollider.transform.position;

                PuzzlePiece puzzlePiece = obj.GetComponent<PuzzlePiece>();

                if (puzzlePiece != null && PuzzleManager.Instance.PlacePuzzlePiece(puzzlePiece))
                {
                    Remove(item); // Remove from inventory if placed correctly
                    ListItems();   // Refresh the inventory display
                    validPlacement = true;
                    break;
                }
            }
            else if (hitCollider.CompareTag("Interactable"))
            {
                InteractableManager.Instance.HandleInteraction(item, hitCollider.gameObject);

                Remove(item);
                ListItems();  // 更新道具欄顯示
                validPlacement = true;
                break;
            }
            //else if (hitCollider.CompareTag("PhotoSlot"))
            //{
            //    Remove(item);
            //    ListItems();
            //    validPlacement = true;
            //    break;
            //}
        }
        if (!validPlacement)
        {
            Debug.Log("No valid slot detected, returning to inventory.");
            ReturnToInventory(obj, item);
            ListItems();
        }
    }

    public void TakeOutItem(Item item)
    {
        SetSpawnPointAtCameraCenter(); // 設定物品生成點

        if (item.prefab != null)
        {
            // 生成物件
            GameObject spawnedItem = Instantiate(item.prefab, spawnPoint.position, spawnPoint.rotation);
            spawnedItem.name = item.itemName;

            PuzzlePiece puzzlePiece = spawnedItem.GetComponent<PuzzlePiece>();
            Debug.Log($"Spawning {item.itemName}, isPuzzlePiece: {item.isPuzzlePiece}");

            // ⭐ 設定 pieceType：你可以根據 item 的屬性判斷
            if (item.isPuzzlePiece)  // 假設你在 Item 類別裡有 isPhoto 這樣的欄位
            {
                puzzlePiece.pieceType = PieceType.Puzzle;
            }
            else
            {
                puzzlePiece.pieceType = PieceType.PhotoAlbum;
            }

            Remove(item);
            ListItems();
            StartCoroutine(DelayedDescriptionUpdate(item));
            itemName.text = item.itemName;
            itemDescription.text = item.description; // 顯示物品敘述
            itemDescription.ForceMeshUpdate();
        }
    }
    private IEnumerator DelayedDescriptionUpdate(Item item)
    {
        yield return null; // 等待一幀，確保 UI 先更新
        itemName.text = item.itemName;
        itemDescription.text = item.description; // 顯示物品敘述
        itemDescription.ForceMeshUpdate();
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
        if (!objectItemMap.ContainsKey(obj))
        {
            objectItemMap.Add(obj, item);
        }
    }

    public void ReturnToInventory(GameObject obj, Item item)
    {
        if (!Items.Contains(item))
        {
            Add(item);

        }

        obj.transform.SetParent(ItemContent);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        obj.transform.localRotation = Quaternion.identity;
      
        ListItems();
    }

    public void SaveInventory()
    {
        PlayerPrefs.SetInt("ItemCount", Items.Count); // 保存道具數量

        for (int i = 0; i < Items.Count; i++)
        {
            PlayerPrefs.SetInt("Item_" + i, Items[i].id);
            // 根據需求儲存更多道具屬性，例如 icon, quantity 等等
        }
        PlayerPrefs.Save();
    }
    public void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("ItemCount", 0); // 獲取保存的道具數量
        Items.Clear(); // 清除當前的道具清單

        for (int i = 0; i < itemCount; i++)
        {
            int itemID = PlayerPrefs.GetInt("Item_" + i);
            Item newItem = FindItemByID(itemID);
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
        ClearDuplicateSceneObjects(); // 移到這裡，在所有道具載入後執行
    }
    private void ClearDuplicateSceneObjects()
    {
        foreach (Item inventoryItem in Items)
        {
            Debug.Log($"Looking for objects with base name: {inventoryItem.prefab.name}");

            GameObject[] untaggedObjects = GameObject.FindObjectsOfType<GameObject>();


            foreach (var obj in untaggedObjects)
            {
                if (obj != null && obj.name.Contains(inventoryItem.prefab.name) && !obj.transform.IsChildOf(ItemContent))
                {
                    Destroy(obj);
                }
            }
        }
    }
    public Item FindItemByID(int id)
    {
        return allItems.Find(item => item.id == id);
    }
}
