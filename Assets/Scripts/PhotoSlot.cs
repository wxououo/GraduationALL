using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSlot : MonoBehaviour
{
    public string requiredPhotoName; // �o�Ӵ��ѻݭn���ۤ��W��
    private bool occupied = false; // �o�Ӵ��ѬO�_�w�Q����

    public bool AlbumIsOccupied()
    {
        return occupied;
    }

    public void SetOccupied(bool state)
    {
        occupied = state;
    }

    // **�s�W����k�G�ˬd PhotoPiece �O�_���T**
    public bool IsCorrectPhoto(PhotoPiece photoPiece)
    {
        if (photoPiece == null || photoPiece.itemData == null)
        {
            return false;
        }

        return photoPiece.itemData.itemName == requiredPhotoName;
    }
}
