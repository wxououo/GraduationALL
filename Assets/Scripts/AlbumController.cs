using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumController : MonoBehaviour
{
    public GameObject albumFrame;              // Album UI 外框容器
    public List<CanvasGroup> albumPages;       // 每一頁的 CanvasGroup
    public GameObject buttonToHide;            // 顯示 Album 時要隱藏的按鈕（可為 null）

    private int currentPage = 0;
    private bool albumIsOpen = false;

    void Start()
    {
        ShowPage(0);               // 預設顯示第一頁
        if (albumFrame != null)
            albumFrame.SetActive(false);   // 相簿預設關閉
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
