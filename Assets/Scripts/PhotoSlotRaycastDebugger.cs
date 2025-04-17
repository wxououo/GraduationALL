using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class PhotoSlotRaycastDebugger : MonoBehaviour
{
    public GraphicRaycaster raycaster;
    public Canvas targetCanvas;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            RunRaycastDebug();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            FixGraphicRaycaster();
        }
    }

    void RunRaycastDebug()
    {
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
            Debug.LogWarning("[RaycastDebugger] 自動尋找 GraphicRaycaster");
        }

        if (raycaster == null)
        {
            Debug.LogError("[RaycastDebugger] 找不到 GraphicRaycaster");
            return;
        }

        if (targetCanvas == null)
        {
            targetCanvas = raycaster.GetComponent<Canvas>();
        }

        Debug.Log($"[RaycastDebugger] 嘗試對 camera 執行 GraphicRaycast");

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        Debug.Log($"[RaycastDebugger] Mouse Position: {Input.mousePosition}");
        Debug.Log($"[RaycastDebugger] 命中數量：{results.Count}");

        if (results.Count == 0)
        {
            Debug.LogWarning("[RaycastDebugger] 沒有命中任何 UI，開始檢查其他可能原因...");

            if (targetCanvas != null)
            {
                Debug.Log($"[Canvas 檢查] 名稱: {targetCanvas.name}");
                Debug.Log($"[Canvas 檢查] RenderMode: {targetCanvas.renderMode}");
                Debug.Log($"[Canvas 檢查] Camera: {targetCanvas.worldCamera}");
                Debug.Log($"[Canvas 檢查] SortingOrder: {targetCanvas.sortingOrder}");
            }
            else
            {
                Debug.LogWarning("[Canvas 檢查] 找不到 Canvas！");
            }
        }

        foreach (var result in results)
        {
            GameObject hitObject = result.gameObject;
            Graphic graphic = hitObject.GetComponent<Graphic>();

            Debug.Log($"[命中] {hitObject.name}, RaycastTarget={graphic?.raycastTarget}, Active={hitObject.activeInHierarchy}");
        }
    }

    void FixGraphicRaycaster()
    {
        if (raycaster == null)
        {
            raycaster = FindObjectOfType<GraphicRaycaster>();
        }

        if (raycaster != null)
        {
            raycaster.enabled = false;
            raycaster.enabled = true;
            Debug.Log("[RaycastDebugger] 已重新啟用 GraphicRaycaster");
        }
        else
        {
            Debug.LogError("[RaycastDebugger] 找不到 GraphicRaycaster，無法修復");
        }
    }
}
