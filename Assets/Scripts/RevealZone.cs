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
    private HashSet<int> revealedItemIDs = new HashSet<int>(); // �O���w�Q Reveal �� item ID
    private const string RevealedKey = "RevealedItemIDs";      // �s�b PlayerPrefs �� key

    private void Awake()
    {
        LoadRevealedItems();
    }


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
                    revealedItemIDs.Add(draggedItemID);
                    SaveRevealedItems(); // �C�� Reveal ���\�N�s�@��
                    Debug.Log($"Revealed object: {mapping.targetObject.name} for item ID: {draggedItemID}");
                    return true;
                }
            }
        }
        Debug.Log("�������� itemId");
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

        // �ھڤw�s�� revealedItemIDs ���}��������
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