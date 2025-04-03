using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoAlbumManager : MonoBehaviour
{
    public static PhotoAlbumManager Instance;
    public List<Transform> photoSlots;
    public float snapThreshold = 2f;

    private void Awake()
    {
        Instance = this;
    }

    public bool PlacePhotoPiece(PhotoPiece photoPiece)
    {
        if (photoPiece == null)
        {
            Debug.LogError("�ǤJ�� PhotoPiece �� null�I");
            return false;
        }

        Collider[] hitColliders = Physics.OverlapSphere(photoPiece.transform.position, 2.0f);
        foreach (Collider hitCollider in hitColliders)
        {
            PhotoSlot slot = hitCollider.GetComponent<PhotoSlot>();
            if (slot != null && !slot.AlbumIsOccupied() && slot.IsCorrectPhoto(photoPiece)) // �T�O��k�i��
            {
                photoPiece.transform.position = slot.transform.position;
                photoPiece.transform.SetParent(slot.transform);
                slot.SetOccupied(true);
                return true;
            }
        }
        return false;
    }
}
