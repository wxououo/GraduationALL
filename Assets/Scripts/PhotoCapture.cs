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

    public string itemName;
    public Sprite itemIntroductionImage;
    public GameObject rewardPrefab; // 可以是 null，null就用拍照的
    public string overridePhotoResourceName; // 如果 rewardPrefab 是空的，用這個載入照片


    private bool viewPhoto;
    private HashSet<Transform> photographedObjects = new HashSet<Transform>(); // 存储已拍摄过的物体
    private List<Item> capturedPhotos = new List<Item>(); // 存储已保存的照片

    // 自定义照片宽高
    public int photoWidth = 800;
    public int photoHeight = 600;

    [SerializeField] private CanvasGroup photoCanvasGroup;
[SerializeField] private float fadeDuration = 1.0f;
[SerializeField] private float displayTime = 3.0f;
    private void Awake()
    {
        if (photoFrame.activeInHierarchy)
        {
            photoCanvasGroup = photoFrame.GetComponent<CanvasGroup>();
        }
        else
        {
            photoFrame.SetActive(true);
            photoCanvasGroup = photoFrame.GetComponent<CanvasGroup>();
            photoFrame.SetActive(false);  // 還原
        }
    }


    // 給 UI Button 呼叫這個方法
    public void TakePhotoButton()
    {
        Debug.Log("👉 拍照按鈕被點了");
        if (!viewPhoto && cameraObject.activeInHierarchy)
        {
            StartCoroutine(CapturePhoto());
        }
        else
        {
            RemovePhoto(); // 再次點擊按鈕則關閉照片
        }
    }

    IEnumerator CapturePhoto()
    {
        viewPhoto = true;
        cameraObject.SetActive(false);
        yield return new WaitForEndOfFrame();

        // 拍照截圖（無論有沒有拍到 PhotoTarget 都執行）
        Texture2D screenCapture = new Texture2D(photoWidth, photoHeight, TextureFormat.RGB24, false);
        Rect regionToRead = new Rect((Screen.width - photoWidth) / 2, (Screen.height - photoHeight) / 2, photoWidth, photoHeight);
        screenCapture.ReadPixels(regionToRead, 0, 0, false);
        screenCapture.Apply();

        ShowPhoto(screenCapture);
        cameraObject.SetActive(true);

        // 嘗試偵測目標物件
        int layerMask = LayerMask.GetMask("PhotoTargetLayer");
        RaycastHit hit;
        bool itemDetected = Physics.Raycast(cameraObject.transform.position, cameraObject.transform.forward, out hit, 5000.0f, layerMask);

        if (!itemDetected) yield break;

        var target = hit.collider.GetComponentInParent<PhotoTarget>();
        if (target == null || photographedObjects.Contains(hit.transform)) yield break;

        // 搜尋附近拼圖狀態
        float searchRadius = 50.0f;
        Collider[] nearbyColliders = Physics.OverlapSphere(hit.transform.position, searchRadius);
        List<PuzzleSlot> nearbySlots = new List<PuzzleSlot>();
        foreach (var col in nearbyColliders)
        {
            PuzzleSlot slot = col.GetComponentInChildren<PuzzleSlot>();
            if (slot != null) nearbySlots.Add(slot);
        }

        if (nearbySlots.Count > 0 && !nearbySlots.TrueForAll(slot => slot.IsOccupied()))
        {
            Debug.Log("有拼圖未放入，無法生成道具。");
            yield break;
        }

        // ✅ 儲存為道具
        if (target.rewardPrefab != null)
        {
            SavePrefabAsItem(target.rewardPrefab, hit.transform, target.itemName);
        }
        else
        {
            Texture2D overrideTex = Resources.Load<Texture2D>(target.overridePhotoResourceName);
            if (overrideTex != null)
            {
                SavePhotoAsItem(overrideTex, hit.transform, target.photoName);
                ShowPhoto(overrideTex); // 替換顯示圖為指定圖片
            }
            else
            {
                Debug.LogWarning("找不到指定的 override 素材圖！");
            }
        }
    }

    void SavePrefabAsItem(GameObject prefab, Transform targetObject, string itemName)
    {
        Item prefabItem = new Item();
        prefabItem.itemName = itemName;
        prefabItem.prefab = prefab;
        PhotoTarget photoTarget = targetObject.GetComponent<PhotoTarget>();
        if (photoTarget != null && photoTarget.IntroductionUI != null)
        {
            prefabItem.icon = photoTarget.IntroductionUI;
        }
        capturedPhotos.Add(prefabItem);
        inventoryManager.Add(prefabItem);

        photographedObjects.Add(targetObject);

        Debug.Log("Prefab saved as item: " + itemName);
    }

    void ShowPhoto(Texture2D photoTexture)
    {
        Sprite photoSprite = Sprite.Create(photoTexture, new Rect(0.0f, 0.0f, photoTexture.width, photoTexture.height), new Vector2(0.5f, 0.5f), photoHeight);
        photoFrame.SetActive(true);        
        photoDisplayArea.sprite = photoSprite;

        photoCanvasGroup.alpha = 0f;
        StartCoroutine(FadeInAndAutoHide());
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
    IEnumerator FadeInAndAutoHide()
    {
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            photoCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        photoCanvasGroup.alpha = 1f;
        yield return new WaitForSeconds(displayTime);

        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            photoCanvasGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        photoCanvasGroup.alpha = 0f;
        RemovePhoto(); // 關閉 UI、重設相機等
    }

}