using UnityEngine;

public class AlbumOpener : MonoBehaviour
{
    public CanvasGroup albumCanvasGroup;
    public GameObject buttonToHide;
    private bool isOpen = false;
    void Start()
    {
        // �T�O albumCanvasGroup ������
        if (albumCanvasGroup != null)
        {
            // �N alpha �]�w�� 0�A�Ϩ䧹���z��
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
