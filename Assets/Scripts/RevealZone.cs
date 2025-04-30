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
    private HashSet<int> revealedItemIDs = new HashSet<int>(); // 記錄已被 Reveal 的 item ID
    private const string RevealedKey = "RevealedItemIDs";      // 存在 PlayerPrefs 的 key

    private void Awake()
    {
        LoadRevealedItems();
    }


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
                    revealedItemIDs.Add(draggedItemID);
                    SaveRevealedItems(); // 每次 Reveal 成功就存一次
                    Debug.Log($"Revealed object: {mapping.targetObject.name} for item ID: {draggedItemID}");
                    return true;
                }
            }
        }
        Debug.Log("未找到對應 itemId");
        return false;
    }
    public void ResetRevealedObjects()
    {
        foreach (var mapping in revealMappings)
        {
            if (mapping.targetObject != null)
            {
                mapping.targetObject.SetActive(false);
            }
        }
        revealedItemIDs.Clear();
        PlayerPrefs.DeleteKey(RevealedKey);
    }

    private void SaveRevealedItems()
    {
        string saveString = string.Join(",", revealedItemIDs);
        PlayerPrefs.SetString(RevealedKey, saveString);
        PlayerPrefs.Save();
    }

    private void LoadRevealedItems()
    {
        if (PlayerPrefs.HasKey(RevealedKey))
        {
            string saveString = PlayerPrefs.GetString(RevealedKey);
            if (!string.IsNullOrEmpty(saveString))
            {
                string[] ids = saveString.Split(',');
                foreach (string idStr in ids)
                {
                    if (int.TryParse(idStr, out int id))
                    {
                        revealedItemIDs.Add(id);
                    }
                }
            }
        }

        // 根據已存的 revealedItemIDs 打開對應物件
        foreach (var mapping in revealMappings)
        {
            if (revealedItemIDs.Contains(mapping.itemID))
            {
                if (mapping.targetObject != null)
                {
                    mapping.targetObject.SetActive(true);
                }
            }
        }
    }
}