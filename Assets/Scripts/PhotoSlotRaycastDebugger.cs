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
            Debug.LogWarning("[RaycastDebugger] �۰ʴM�� GraphicRaycaster");
        }

        if (raycaster == null)
        {
            Debug.LogError("[RaycastDebugger] �䤣�� GraphicRaycaster");
            return;
        }

        if (targetCanvas == null)
        {
            targetCanvas = raycaster.GetComponent<Canvas>();
        }

        Debug.Log($"[RaycastDebugger] ���չ� camera ���� GraphicRaycast");

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        raycaster.Raycast(pointerData, results);

        Debug.Log($"[RaycastDebugger] Mouse Position: {Input.mousePosition}");
        Debug.Log($"[RaycastDebugger] �R���ƶq�G{results.Count}");

        if (results.Count == 0)
        {
            Debug.LogWarning("[RaycastDebugger] �S���R������ UI�A�}�l�ˬd��L�i���]...");

            if (targetCanvas != null)
            {
                Debug.Log($"[Canvas �ˬd] �W��: {targetCanvas.name}");
                Debug.Log($"[Canvas �ˬd] RenderMode: {targetCanvas.renderMode}");
                Debug.Log($"[Canvas �ˬd] Camera: {targetCanvas.worldCamera}");
                Debug.Log($"[Canvas �ˬd] SortingOrder: {targetCanvas.sortingOrder}");
            }
            else
            {
                Debug.LogWarning("[Canvas �ˬd] �䤣�� Canvas�I");
            }
        }

        foreach (var result in results)
        {
            GameObject hitObject = result.gameObject;
            Graphic graphic = hitObject.GetComponent<Graphic>();

            Debug.Log($"[�R��] {hitObject.name}, RaycastTarget={graphic?.raycastTarget}, Active={hitObject.activeInHierarchy}");
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
            Debug.Log("[RaycastDebugger] �w���s�ҥ� GraphicRaycaster");
        }
        else
        {
            Debug.LogError("[RaycastDebugger] �䤣�� GraphicRaycaster�A�L�k�״_");
        }
    }
}
