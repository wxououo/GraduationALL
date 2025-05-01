using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumController : MonoBehaviour
{
    public GameObject albumFrame;              // Album UI �~�خe��
    public List<CanvasGroup> albumPages;       // �C�@���� CanvasGroup
    public GameObject buttonToHide;            // ��� Album �ɭn���ê����s�]�i�� null�^

    private int currentPage = 0;
    private bool albumIsOpen = false;

    void Start()
    {
        ShowPage(0);               // �w�]��ܲĤ@��
        if (albumFrame != null)
            albumFrame.SetActive(false);   // ��ï�w�]����
    }

    public void ToggleAlbum()
    {
        albumIsOpen = !albumIsOpen;
        albumFrame.SetActive(albumIsOpen);

        if (buttonToHide != null)
            buttonToHide.SetActive(!albumIsOpen);
    }

    public void NextPage()
    {
        if (!albumIsOpen) return;
        int nextPage = currentPage + 1;
        if (nextPage < albumPages.Count)
            ShowPage(nextPage);
    }

    public void PrevPage()
    {
        if (!albumIsOpen) return;
        int prevPage = currentPage - 1;
        if (prevPage >= 0)
            ShowPage(prevPage);
    }

    private void ShowPage(int pageIndex)
    {
        for (int i = 0; i < albumPages.Count; i++)
        {
            bool isActive = (i == pageIndex);
            CanvasGroup cg = albumPages[i];
            albumPages[i].alpha = isActive ? 1f : 0f;
            albumPages[i].interactable = isActive;
            albumPages[i].blocksRaycasts = isActive;
        }
        currentPage = pageIndex;
    }
}
