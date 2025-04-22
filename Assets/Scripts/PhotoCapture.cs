using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotoCapture : MonoBehaviour
{
    [Header("Photo Taker")]
    [SerializeField] private Image photoDisplayArea;
    [SerializeField] private GameObject photoFrame;
    [SerializeField] private GameObject cameraObject;
    [SerializeField] private InventoryManager inventoryManager;


    private bool viewPhoto;
    private HashSet<Transform> photographedObjects = new HashSet<Transform>(); // 存储已拍摄过的物体
    private List<Item> capturedPhotos = new List<Item>(); // 存储已保存的照片

    // 自定义照片宽高
    public int photoWidth = 800;
    public int photoHeight = 600;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (cameraObject != null && cameraObject.activeInHierarchy)
            {
                photoFrame.SetActive(false);
                if (!viewPhoto)
                {
                    StartCoroutine(CapturePhoto());
                }
                else
                {
                    RemovePhoto();
                }
            }
        }
    }

    IEnumerator CapturePhoto()
    {
        viewPhoto = true;
        cameraObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        // 检测相机前方是否有目标物体
        RaycastHit hit;
        bool itemDetected = Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, 5000.0f);

        // 创建一个新的 Texture2D 来存储每次拍摄的照片
        Texture2D screenCapture = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
        Rect regionToRead = new Rect((Screen.width - photoWidth) / 2, (Screen.height - photoHeight) / 2, photoWidth, photoHeight);
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        cameraObject.SetActive(true);
        ShowPhoto(screenCapture);

        // 如果检测到目标物体且该物体有 PhotoTarget 组件，并且该物体尚未拍摄过照片
        if (itemDetected)
        {
            Debug.Log("Item Detected");

            // 取得被 Raycast 擊中的物件 transform
            Transform targetTransform = hit.transform;

            // ✅ 搜索該物件周圍是否有 PuzzleSlot
            float searchRadius = 20.0f; // 可調參數
            Collider[] nearbyColliders = Physics.OverlapSphere(targetTransform.position, searchRadius);
            List<PuzzleSlot> nearbyPuzzleSlots = new List<PuzzleSlot>();

            foreach (var collider in nearbyColliders)
            {
                Debug.Log("Overlap 物件：" + collider.name);
                PuzzleSlot slot = collider.GetComponentInChildren<PuzzleSlot>();

                if (slot != null)
                {
                    nearbyPuzzleSlots.Add(slot);
                    Debug.Log($"發現 PuzzleSlot：{slot.name}，是否被佔用：{slot.IsOccupied()}");
                }
            }

            if (nearbyPuzzleSlots.Count > 0)
            {
                // 如果有 PuzzleSlot，但有任何一個未被佔用，就禁止拍照
                bool allOccupied = nearbyPuzzleSlots.TrueForAll(slot => slot.IsOccupied());
                if (!allOccupied)
                {
                    Debug.Log("有 PuzzleSlot 尚未放入拼圖，無法拍照！");
                    yield break; // 直接跳出，不執行拍照保存
                }
            }
            var target = hit.transform.GetComponent<PhotoTarget>();
            if (target != null)
            {
                Debug.Log("Hit has PhotoTarget component");

                if (!photographedObjects.Contains(hit.transform))
                {
                    Debug.Log("This object has not been photographed before");

                    string photoName = target.photoName;
                    string resourceName = target.overridePhotoResourceName;
                    //Texture2D specifiedPhoto = Resources.Load<Texture2D>("Photos/marry");
                    Texture2D specifiedPhoto = Resources.Load<Texture2D>(resourceName);
                    if (specifiedPhoto != null)
                    {
                        SavePhotoAsItem(specifiedPhoto, hit.transform, photoName);
                        Debug.Log("指定素材已儲存為照片：" + photoName);
                    }
                    else
                    {
                        Debug.LogError("找不到指定圖片素材！");
                    }
                }
                else
                {
                    Debug.Log("這張照片已經拍過了！");
                }
            }
            else
            {
                Debug.Log("Hit 的物件上沒有 PhotoTarget");
            }
        }
        else
        {
            Debug.Log("itemDetected 為 false，沒有偵測到目標");
        }

    }

    void ShowPhoto(Texture2D photoTexture)
    {
        Sprite photoSprite = Sprite.Create(photoTexture, new Rect(0.0f, 0.0f, photoTexture.width, photoTexture.height), new Vector2(0.5f, 0.5f), photoHeight);
        photoDisplayArea.sprite = photoSprite;
        photoFrame.SetActive(true);
    }

    void RemovePhoto()
    {
        viewPhoto = false;
        photoFrame.SetActive(false);
        cameraObject.SetActive(true);
    }

    void SavePhotoAsItem(Texture2D photoTexture, Transform targetObject, string photoName)
    {
        // 创建一个新照片道具并设置其名称和图标
        Item photoItem = new Item();
        photoItem.name = photoName; // 使用传入的物体名称
        photoItem.icon = Sprite.Create(photoTexture, new Rect(0.0f, 0.0f, photoTexture.width, photoTexture.height), new Vector2(0.5f, 0.5f), 100.0f);

        // 設定物品名稱
        photoItem.itemName = photoName;

        // 将照片道具添加到清单
        capturedPhotos.Add(photoItem);
        inventoryManager.Add(photoItem);  // 保存到 Inventory

        // 记录已经拍摄过该物体，避免再次拍照
        photographedObjects.Add(targetObject);

        Debug.Log("Photo saved as item: " + photoName);
    }

}
