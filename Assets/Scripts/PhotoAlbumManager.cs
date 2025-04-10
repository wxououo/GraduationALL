using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PhotoAlbumManager : MonoBehaviour
{
    public static PhotoAlbumManager Instance;
    public List<Transform> photoSlots;
    public float snapThreshold = 2f;
    public VideoPlayer videoPlayer; // ��J Scene ���� VideoPlayer
    public Dictionary<int, VideoClip> photoIdToVideoClip = new Dictionary<int, VideoClip>();

    private void Start()
    {
        Debug.Log("���ռ���v��...");

        VideoClip testClip = Resources.Load<VideoClip>("Videos/marry");
        if (testClip == null)
        {
            Debug.LogError("�䤣�� marry.mp4�I");
        }
        else
        {
            Debug.Log("���\���J marry.mp4�A�ǳƼ���");
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
        Debug.Log($"���b��m�Ӥ��� slot�G{slot.name}");

        if (slot.IsOccupied())
        {
            Debug.Log("�o�� slot �w�g�Q����");
            return false;
        }

        piece.transform.position = slot.transform.position;
        slot.MarkAsOccupied();
        Debug.Log("���\��m�A�}�l RevealObject()");
        slot.RevealObject();

        int photoId = piece.itemData.id;

        if (photoIdToVideoClip.TryGetValue(photoId, out VideoClip clip))
        {
            if (videoPlayer != null && clip != null)
            {
                Debug.Log($"videoPlayer={videoPlayer}, clip={clip}");
                videoPlayer.clip = clip;
                videoPlayer.Play();
                Debug.Log($"����v���G{clip.name}");
            }
            else
            {
                Debug.LogWarning("VideoPlayer �μv�� Clip �� null");
            }
        }
        else
        {
            Debug.LogWarning($"�䤣����� photoId ���v���G{photoId}");
        }
        return true;
    }
    }
