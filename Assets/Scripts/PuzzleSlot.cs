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
    private Image imageComponent;
    public bool isOccupied = false;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            EnsureInitialized();
            // imageComponent = GetComponent<Image>();
            // if (imageComponent == null)
            // {
            //     Debug.LogError("No Image component found on " + gameObject.name);
            // }

        }
       
    }
    public void EnsureInitialized()
    {
        if (imageComponent == null)
        {
            imageComponent = GetComponent<Image>();
            if (imageComponent == null)
            {
                Debug.LogError("No Image component found on " + gameObject.name);
            }
            else
            {
                SetToGrayscale();
            }
        }
    }
    void Start()
    {
        //imageComponent = GetComponent<Image>();
        //EnsureInitialized();
        if (imageComponent != null)
        {
            if (transform.childCount > 0)
            {
                SetToColor(); // Set the slot to the colored appearance if it's occupied
            }
            else
            {
                SetToGrayscale(); // Set the slot to the grayscale appearance if it's not occupied
            }
        }
    }

    public void SetToColor()
    {
        if (imageComponent == null)
        {
            Debug.LogError("Image component is null when trying to set to color");
            return;
        }
        if (colorMaterial != null)
        {
            imageComponent.material = colorMaterial;
        }
        else
        {
            Debug.LogError("Color material is not assigned on " + gameObject.name);
        }
    }

    public void SetToGrayscale()
    {
        if (imageComponent == null)
        {
            Debug.LogError("Image component is null when trying to set to grayscale");
            return;
        }
        if (grayscaleMaterial != null)
        {
            imageComponent.material = grayscaleMaterial;
        }
        else
        {
            Debug.LogError("Grayscale material is not assigned on " + gameObject.name);
        }
    }
    public void SetToOccupied(bool status)
    {
        isOccupied = status;
        if (status)
        {
            SetToColor(); // Set the slot to the colored appearance
        }
        else
        {
            SetToGrayscale(); // Set the slot to the grayscale appearance
        }
    }
    public bool IsOccupied()
    {
        return transform.childCount > 0; // 檢查是否有拼圖碎片在槽位上
    }
}
