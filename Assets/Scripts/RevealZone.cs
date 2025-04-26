using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RevealMapping
{
    public int itemID;             // ������ item
    public GameObject targetObject; // �n��ܪ�����
}
public class RevealZone : MonoBehaviour
{
    public List<RevealMapping> revealMappings;

    public bool TryReveal(int draggedItemID)
    {
        Debug.Log($"TryReveal called with item ID: {draggedItemID}");
        foreach (var mapping in revealMappings)
        {
            Debug.Log($"��襤�G{draggedItemID} vs {mapping.itemID}");
            if (mapping.itemID == draggedItemID)
            {
                if (mapping.targetObject != null)
                {
                    Debug.Log("���������ؼЪ���A�ҥΤ��I");
                    mapping.targetObject.SetActive(true);
                    Debug.Log($"Revealed object: {mapping.targetObject.name} for item ID: {draggedItemID}");
                    return true;
                }
            }
        }
        Debug.Log("�������� itemId");
        return false;
    }
}
