using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RevealMapping
{
    public int itemID;             // 對應的 item
    public GameObject targetObject; // 要顯示的物件
}
public class RevealZone : MonoBehaviour
{
    public List<RevealMapping> revealMappings;

    public bool TryReveal(int draggedItemID)
    {
        Debug.Log($"TryReveal called with item ID: {draggedItemID}");
        foreach (var mapping in revealMappings)
        {
            Debug.Log($"比對中：{draggedItemID} vs {mapping.itemID}");
            if (mapping.itemID == draggedItemID)
            {
                if (mapping.targetObject != null)
                {
                    Debug.Log("找到對應的目標物件，啟用中！");
                    mapping.targetObject.SetActive(true);
                    Debug.Log($"Revealed object: {mapping.targetObject.name} for item ID: {draggedItemID}");
                    return true;
                }
            }
        }
        Debug.Log("未找到對應 itemId");
        return false;
    }
}
