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
        // �Ұʮɱq PlayerPrefs ���J���Ϊ��A
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
        return piece.itemData.id == requiredPhotoId; // �Ψ�L�P�_�覡
    }

    public void MarkAsOccupied()
    {
        occupied = true;
        PlayerPrefs.SetInt(OccupiedKey, 1);
        PlayerPrefs.Save();
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