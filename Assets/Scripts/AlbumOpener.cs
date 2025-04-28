using UnityEngine;

public class AlbumOpener : MonoBehaviour
{
    public GameObject SceneCamera;
    [SerializeField] private GameObject buttonToHide;
    public GameObject AlbumFrame;
    public void OpenAlbum()
    {
        if (AlbumFrame != null)
        {
            bool isActive = AlbumFrame.activeSelf;
            AlbumFrame.SetActive(!isActive);
            if (buttonToHide != null && !SceneCamera.activeInHierarchy)
            {
                buttonToHide.SetActive(isActive);
            }
        }
    }
}