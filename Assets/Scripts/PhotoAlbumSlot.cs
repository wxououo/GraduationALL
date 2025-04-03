using UnityEngine;

public class PhotoAlbumSlot : MonoBehaviour
{
    public int slotID; // 指定這個插槽的唯一ID
    private bool AlbumisOccupied = false;

    public bool AlbumIsOccupied()
    {
        return AlbumisOccupied;
    }

    public void AlbumSetToOccupied(bool occupied)
    {
        AlbumisOccupied = occupied;
        PlayerPrefs.SetInt($"PhotoSlot_{slotID}_Occupied", occupied ? 1 : 0);
        PlayerPrefs.Save();
    }
}
