using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PhotoAlbumManager : MonoBehaviour
{
    public static PhotoAlbumManager Instance;
    public List<Transform> photoSlots;
    public float snapThreshold = 2f;
    public VideoPlayer videoPlayer; // 拖入 Scene 中的 VideoPlayer
    public Dictionary<int, VideoClip> photoIdToVideoClip = new Dictionary<int, VideoClip>();

    private void Start()
    {
        Debug.Log("測試播放影片...");

        VideoClip testClip = Resources.Load<VideoClip>("Videos/marry");
        if (testClip == null)
        {
            Debug.LogError("找不到 marry.mp4！");
        }
        else
        {
            Debug.Log("成功載入 marry.mp4，準備播放");
            videoPlayer.clip = testClip;
            videoPlayer.Play();
        }
    }

    private void Awake()
    {
        Instance = this;
        photoIdToVideoClip[0] = Resources.Load<VideoClip>("Videos/marry");
        photoIdToVideoClip[1] = Resources.Load<VideoClip>("Videos/clipB");
        photoIdToVideoClip[2] = Resources.Load<VideoClip>("Videos/clipA");
        photoIdToVideoClip[3] = Resources.Load<VideoClip>("Videos/clipB");
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
                videoPlayer.Play();
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
    }
