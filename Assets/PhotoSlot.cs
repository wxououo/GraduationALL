using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSlot : MonoBehaviour
{
    public int requiredPhotoId;
    private bool occupied = false;
    public GameObject PhotoToReveal;
    private string OccupiedKey => $"PhotoSlot_{requiredPhotoId}_Occupied";

    private void Start()
    {
        // 啟動時從 PlayerPrefs 載入佔用狀態
        occupied = PlayerPrefs.GetInt(OccupiedKey, 0) == 1;
        if (occupied && PhotoToReveal != null)
        {
            PhotoToReveal.SetActive(true);
        }
    }

    public bool IsOccupied()
    {
        return occupied;
    }

    public bool IsValidForPiece(PuzzlePiece piece)
    {
        return piece.itemData.id == requiredPhotoId; // 或其他判斷方式
    }

    public void MarkAsOccupied()
    {
        occupied = true;
        PlayerPrefs.SetInt(OccupiedKey, 1);
        PlayerPrefs.Save();
    }
    public bool IsWithinPlacementZone(Vector3 piecePosition)
    {
        Debug.Log($"槽位位置: {transform.position}, 拖曳位置: {piecePosition}, 距離: {Vector3.Distance(transform.position, piecePosition)}");
        float distance = Vector3.Distance(piecePosition, transform.position);
        return distance < 150000000f; // 你可以自訂距離範圍（例如 1.5f 為可放置距離）
    }
    public void RevealObject()
    {
        if (PhotoToReveal != null)
        {
            PhotoToReveal.SetActive(true);
        }
    }
    public void ResetSlot()
    {
        occupied = false;
        PlayerPrefs.DeleteKey(OccupiedKey);
        if (PhotoToReveal != null)
        {
            PhotoToReveal.SetActive(false);
        }
    }
}