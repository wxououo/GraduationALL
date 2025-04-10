using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSlot : MonoBehaviour
{
    public int requiredPhotoId;
    private bool occupied = false;
    public GameObject PhotoToReveal;

    public bool IsOccupied()
    {
        return occupied;
    }

    public bool IsValidForPiece(PuzzlePiece piece)
    {
        return piece.itemData.id == requiredPhotoId; // �Ψ�L�P�_�覡
    }

    public void MarkAsOccupied()
    {
        occupied = true;
    }
    public bool IsWithinPlacementZone(Vector3 piecePosition)
    {
        Debug.Log($"�Ѧ��m: {transform.position}, �즲��m: {piecePosition}, �Z��: {Vector3.Distance(transform.position, piecePosition)}");
        float distance = Vector3.Distance(piecePosition, transform.position);
        return distance < 150000000f; // �A�i�H�ۭq�Z���d��]�Ҧp 1.5f ���i��m�Z���^
    }
    public void RevealObject()
    {
        if (PhotoToReveal != null)
        {
            PhotoToReveal.SetActive(true);
        }
    }
}