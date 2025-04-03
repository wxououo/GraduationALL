using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoPiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Item itemData;
    private Vector3 startPosition;
    private Transform originalParent;
    private Camera playerCamera;
    public GameObject targetObject;
    public bool isPlacedCorrectly = false;

    private void Start()
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        playerCamera = Camera.main;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = false;
        transform.SetParent(null);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            transform.position = hit.point;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = gameObject.AddComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = true;

        if (PhotoAlbumManager.Instance == null)
        {
            Debug.LogError("PhotoAlbumManager.Instance is null!");
            ReturnToStartPosition();
            return;
        }

        bool placedCorrectly = PhotoAlbumManager.Instance.PlacePhotoPiece(this);
        if (placedCorrectly)
        {
            HandleSuccessfulPlacement();
        }
        else
        {
            ReturnToStartPosition();
        }
    }

    private void HandleSuccessfulPlacement()
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.Remove(itemData);
            InventoryManager.Instance.ListItems();
        }
        if (targetObject != null)
        {
            targetObject.SetActive(true);
        }
    }

    private void ReturnToStartPosition()
    {
        transform.position = startPosition;
        transform.SetParent(originalParent);
    }
}
