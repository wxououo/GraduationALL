using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour
{
    public static PuzzleSlot Instance;
    public int slotID;  // Optional: 指定這個槽位的ID，用於檢查匹配
    public Material grayscaleMaterial;
    public Material colorMaterial;
    private Image imageComponent;
public bool isOccupied = false;
    void Start()
    {
        imageComponent = GetComponent<Image>();
        imageComponent.material = grayscaleMaterial;
    }

    public void SetToColor()
    {
        imageComponent.material = colorMaterial;
    }

    public void SetToGrayscale()
    {
        imageComponent.material = grayscaleMaterial;
    }
public void SetToOccupied(bool status)
    {
        isOccupied = status;
    }
    public bool IsOccupied()
    {
        return transform.childCount > 0; // 檢查是否有拼圖碎片在槽位上
    }
}
