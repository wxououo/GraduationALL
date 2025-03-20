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

    public bool PlacePhotoPiece(PuzzlePiece photoPiece)
    {
        if (photoPiece == null || photoPiece.itemData == null)
        {
            Debug.LogError("Invalid PhotoPiece!");
            return false;
        }

        foreach (Transform slot in photoSlots)
        {
            float distance = Vector3.Distance(photoPiece.transform.position, slot.position);

            if (distance < snapThreshold)
            {
                photoPiece.transform.position = slot.position;
                photoPiece.transform.SetParent(slot);
                Debug.Log($"Photo placed in album: {photoPiece.name}");
                return true;
            }
        }

        return false;
    }
}
