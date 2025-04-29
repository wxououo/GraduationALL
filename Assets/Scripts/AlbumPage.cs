using UnityEngine;

public class AlbumPageController : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] albumPages;
    private int currentPage = 0;

    private void Start()
    {
        ShowPage(currentPage);
    }

    public void ShowNextPage()
    {
        currentPage++;
        if (currentPage >= albumPages.Length)
            currentPage = 0;
        ShowPage(currentPage);
    }

    public void ShowPreviousPage()
    {
        currentPage--;
        if (currentPage < 0)
            currentPage = albumPages.Length - 1;
        ShowPage(currentPage);
    }

    private void ShowPage(int index)
    {
        for (int i = 0; i < albumPages.Length; i++)
        {
            bool isActivePage = (i == index);
            albumPages[i].alpha = isActivePage ? 1f : 0f;
            albumPages[i].interactable = isActivePage;
            albumPages[i].blocksRaycasts = isActivePage;
        }
    }
}
