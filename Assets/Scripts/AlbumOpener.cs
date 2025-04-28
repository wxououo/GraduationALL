using UnityEngine;

public class AlbumOpener : MonoBehaviour
{
    public CanvasGroup albumCanvasGroup;
    public GameObject buttonToHide;
    private bool isOpen = false;
    void Start()
    {
        // 確保 albumCanvasGroup 不為空
        if (albumCanvasGroup != null)
        {
            // 將 alpha 設定為 0，使其完全透明
            albumCanvasGroup.alpha = 0f;
        }
    }

    public void OpenAlbum()
    {
        if (albumCanvasGroup != null)
        {
            isOpen = !isOpen;
            albumCanvasGroup.alpha = isOpen ? 0f : 1f;
            albumCanvasGroup.interactable = !isOpen;
            albumCanvasGroup.blocksRaycasts = !isOpen;

            if (buttonToHide != null)
            {
                buttonToHide.SetActive(isOpen);
            }
        }
    }
}
