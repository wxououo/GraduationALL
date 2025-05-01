using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlbumPageController : MonoBehaviour
{
    [System.Serializable]
    public class AlbumPage
    {
        public string name;
        public CanvasGroup pageGroup;
    }

    public List<AlbumPage> albumPages = new List<AlbumPage>();
    public Button nextButton;
    public Button prevButton;

    private int currentPageIndex = 0;

    void Start()
    {
        ShowPage(currentPageIndex);

        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);

        if (prevButton != null)
            prevButton.onClick.AddListener(PreviousPage);
    }

    void ShowPage(int index)
    {
        for (int i = 0; i < albumPages.Count; i++)
        {
            CanvasGroup group = albumPages[i].pageGroup;
            bool isActive = (i == index);
            group.alpha = isActive ? 1f : 0f;
            group.interactable = isActive;
            group.blocksRaycasts = isActive;
        }
    }

    void NextPage()
    {
        if (currentPageIndex < albumPages.Count - 1)
        {
            currentPageIndex++;
            ShowPage(currentPageIndex);
        }
    }

    void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowPage(currentPageIndex);
        }
    }

    public void OpenToPage(int index)
    {
        if (index >= 0 && index < albumPages.Count)
        {
            currentPageIndex = index;
            ShowPage(currentPageIndex);
        }
    }

    public int GetCurrentPageIndex()
    {
        return currentPageIndex;
    }
}
