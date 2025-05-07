using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.UI;

public class PhotoAlbumManager : MonoBehaviour
{
    public static PhotoAlbumManager Instance;
    public List<Transform> photoSlots;
    public float snapThreshold = 2f;

    public VideoPlayer videoPlayer; // 拖入 Scene 中的 VideoPlayer
    public GameObject VideoDisplay; // RawImage 的父物件 (例如整個影片面板)
    //hide
    public GameObject UI;
    public Button InventoryButton;


    public Dictionary<int, VideoClip> photoIdToVideoClip = new Dictionary<int, VideoClip>();

    private void Awake()
    {
        Instance = this;

        // 載入影片素材
        photoIdToVideoClip[13] = Resources.Load<VideoClip>("Videos/family0422");
        photoIdToVideoClip[12] = Resources.Load<VideoClip>("Videos/hairclip0422");
        photoIdToVideoClip[15] = Resources.Load<VideoClip>("Videos/candybox0424");
        photoIdToVideoClip[11] = Resources.Load<VideoClip>("Videos/Maeey0422");

        // 確保一開始影片不顯示
        if (VideoDisplay != null)
            VideoDisplay.SetActive(false);

        // 加入播放結束的 callback
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
    }

    public bool PlacePhotoPiece(PuzzlePiece piece, PhotoSlot slot)
    {
        Debug.Log($"正在放置照片到 slot：{slot.name}");

        if (slot.IsOccupied())
        {
            Debug.Log("這個 slot 已經被佔用");
            return false;
        }

        piece.transform.position = slot.transform.position;
        slot.MarkAsOccupied();
        Debug.Log("成功放置，開始 RevealObject()");
        slot.RevealObject();

        int photoId = piece.itemData.id;

        if (photoIdToVideoClip.TryGetValue(photoId, out VideoClip clip))
        {
            if (videoPlayer != null && clip != null)
            {
                Debug.Log($"videoPlayer={videoPlayer}, clip={clip}");

                videoPlayer.clip = clip;
                StartCoroutine(PlayVideoWithDelay(clip, 1.5f)); // 延遲 1.5 秒播放
                                                                //videoPlayer.Play();
                Debug.Log($"播放影片：{clip.name}");
            }
            else
            {
                Debug.LogWarning("VideoPlayer 或影片 Clip 為 null");
            }
        }
        else
        {
            Debug.LogWarning($"找不到對應 photoId 的影片：{photoId}");
        }

        return true;
    }

    // 播放完畢時隱藏 VideoDisplay
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (VideoDisplay != null)
        {
            VideoDisplay.SetActive(false);
            Debug.Log("影片播放完畢，自動隱藏");
            UI.SetActive(true);
        }
    }

    private IEnumerator PlayVideoWithDelay(VideoClip clip, float delaySeconds)
    {
        Debug.Log($"延遲 {delaySeconds} 秒後播放影片：{clip.name}");

        yield return new WaitForSeconds(delaySeconds);
        if (VideoDisplay != null)
            VideoDisplay.SetActive(true);
        videoPlayer.clip = clip;
        VideoDisplay.gameObject.SetActive(true);
        UI.SetActive(false);
        if (InventoryButton != null && InventoryButton.interactable)
        {
            InventoryButton.onClick.Invoke();
            Debug.Log($"程式化觸發了按鈕 {InventoryButton.name} 的 OnClick 事件。");
        }
        videoPlayer.Play();

        Debug.Log("開始播放影片！");
    }

}


