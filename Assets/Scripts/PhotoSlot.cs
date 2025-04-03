using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSlot : MonoBehaviour
{
    public string requiredPhotoName; // 這個插槽需要的相片名稱
    private bool occupied = false; // 這個插槽是否已被佔據

    public bool AlbumIsOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool state)
    {
        occupied = state;
    }

    // **新增的方法：檢查 PhotoPiece 是否正確**
    public bool IsCorrectPhoto(PhotoPiece photoPiece)
    {
        if (photoPiece == null || photoPiece.itemData == null)
        {
            return false;
        }

        return photoPiece.itemData.itemName == requiredPhotoName;
    }
}
