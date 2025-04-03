using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour
{
    //public static PuzzleSlot Instance;
    public static PuzzleSlot Instance { get; private set; }

    public int slotID;  // Optional: 指定這個槽位的ID，用於檢查匹配
    public Material grayscaleMaterial;
    public Material colorMaterial;
    public Material previewMaterial; // 新增：悬浮高亮材质
    private MeshRenderer meshRenderer;
    private Material originalMaterial;
    private Image imageComponent;
    public bool isOccupied = false;

    public float placementThreshold = 2.0f;
    public float detectionRadius = 2.0f;  // 增加偵測範圍
    // private void Awake()
    // {
    //     if (Instance == null)
    //     {
    //         Instance = this;
    //         EnsureInitialized();
    //         // imageComponent = GetComponent<Image>();
    //         // if (imageComponent == null)
    //         // {
    //         //     Debug.LogError("No Image component found on " + gameObject.name);
    //         // }

    //     }
    //     // else{
    //     //     gameObject.SetActive(false);
    //     // }
    // }
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        // 确保有Collider
        BoxCollider boxCollider = GetComponent<BoxCollider>();
        if (boxCollider == null)
        {
            boxCollider = gameObject.AddComponent<BoxCollider>();
        }

        // 設定 Collider 屬性
        boxCollider.isTrigger = true;

        //originalMaterial = meshRenderer.material;
        UpdateAppearance();
        originalMaterial = GetComponent<Renderer>().material;
        previewMaterial = new Material(originalMaterial);
        previewMaterial.color = Color.yellow;
    }
    public void ShowInteractionPreview()
    {
        GetComponent<Renderer>().material = previewMaterial;
    }

    public void Interact(Item item)
    {
        // 在這裡實作與道具的互動邏輯
        Debug.Log($"與道具 {item.name} 互動");
        GetComponent<Renderer>().material = originalMaterial;
    }

    public void EnsureInitialized()
    {
        imageComponent = GetComponent<Image>();
        Collider collider = GetComponent<Collider>();
        if (imageComponent == null)
        {
            Debug.LogError("No Image component found on " + gameObject.name);
        }
        if (collider == null)
        {
            // Add a box collider if none exists
            BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
            // Make the collider slightly larger than the visual element
            boxCollider.size = new Vector3(
                transform.localScale.x * 1.2f,
                transform.localScale.y * 1.2f,
                0.1f
            );
            boxCollider.isTrigger = true;
        }
        UpdateAppearance();
    }
    void Start()
    {
        //imageComponent = GetComponent<Image>();
        //EnsureInitialized();
        UpdateAppearance();
    }

    // public void UpdateAppearance()
    // {
    //     if (imageComponent == null) return;

    //     // Check if slot is actually occupied
    //     isOccupied = IsOccupied();

    //     if (isOccupied)
    //     {
    //         SetToColor();
    //     }
    //     else
    //     {
    //         SetToGrayscale();
    //     }
    // }
    public void SetToColor()
    {
        if (imageComponent == null || colorMaterial == null) return;
        imageComponent.material = colorMaterial;
    }

    public void SetToGrayscale()
    {
        if (imageComponent == null || grayscaleMaterial == null) return;
        imageComponent.material = grayscaleMaterial;
    }
    public void ShowHighlight()
    {
        if (!isOccupied && previewMaterial != null && meshRenderer != null)
        {
            meshRenderer.material = previewMaterial;
            Debug.Log($"Showing highlight on slot {slotID}"); // 添加除錯訊息
        }
    }

    public void HideHighlight()
    {
        if (!isOccupied && meshRenderer != null)
        {
            meshRenderer.material = grayscaleMaterial;
            Debug.Log($"Hiding highlight on slot {slotID}"); // 添加除錯訊息
        }
    }
    public void SetToOccupied(bool status)
    {
        isOccupied = status;
        // if (status)
        // {
        //     SetToColor(); // Set the slot to the colored appearance
        // }
        // else
        // {
        //     SetToGrayscale(); // Set the slot to the grayscale appearance
        // }
        UpdateAppearance();
    }
    public bool IsOccupied()
    {
        return transform.childCount > 0 || isOccupied; // 檢查是否有拼圖碎片在槽位上
    }

    public bool IsWithinPlacementZone(Vector3 position)
    {
        float distance = Vector3.Distance(transform.position, position);
        return distance <= placementThreshold;
    }

    private void OnTriggerEnter(Collider other)
    {
        PuzzlePiece piece = other.GetComponent<PuzzlePiece>();
        if (piece != null)
        {
            // Highlight the slot or provide visual feedback
            SetToColor();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isOccupied)
        {
            SetToGrayscale();
        }
    }
    private void UpdateAppearance()
    {
        if (meshRenderer != null)
        {
            meshRenderer.material = isOccupied ? colorMaterial : grayscaleMaterial;
            Debug.Log($"Updated appearance for slot {slotID}. Occupied: {isOccupied}"); // 添加除錯訊息
        }
    }
}
