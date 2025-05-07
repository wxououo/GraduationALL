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

    public VideoPlayer videoPlayer; // ��J Scene ���� VideoPlayer
    public GameObject VideoDisplay; // RawImage �������� (�Ҧp��Ӽv�����O)
    //hide
    public GameObject UI;
    public Button InventoryButton;


    public Dictionary<int, VideoClip> photoIdToVideoClip = new Dictionary<int, VideoClip>();

    private void Awake()
    {
        Instance = this;

        // ���J�v������
        photoIdToVideoClip[13] = Resources.Load<VideoClip>("Videos/family0422");
        photoIdToVideoClip[12] = Resources.Load<VideoClip>("Videos/hairclip0422");
        photoIdToVideoClip[15] = Resources.Load<VideoClip>("Videos/candybox0424");
        photoIdToVideoClip[11] = Resources.Load<VideoClip>("Videos/Maeey0422");

        // �T�O�@�}�l�v�������
        if (VideoDisplay != null)
            VideoDisplay.SetActive(false);

        // �[�J���񵲧��� callback
        if (videoPlayer != null)
            videoPlayer.loopPointReached += OnVideoFinished;
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
                StartCoroutine(PlayVideoWithDelay(clip, 1.5f)); // ���� 1.5 ����
                                                                //videoPlayer.Play();
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

    // ���񧹲������� VideoDisplay
    private void OnVideoFinished(VideoPlayer vp)
    {
        if (VideoDisplay != null)
        {
            VideoDisplay.SetActive(false);
            Debug.Log("�v�����񧹲��A�۰�����");
            UI.SetActive(true);
        }
    }

    private IEnumerator PlayVideoWithDelay(VideoClip clip, float delaySeconds)
    {
        Debug.Log($"���� {delaySeconds} ��Ἵ��v���G{clip.name}");

        yield return new WaitForSeconds(delaySeconds);
        if (VideoDisplay != null)
            VideoDisplay.SetActive(true);
        videoPlayer.clip = clip;
        VideoDisplay.gameObject.SetActive(true);
        UI.SetActive(false);
        if (InventoryButton != null && InventoryButton.interactable)
        {
            InventoryButton.onClick.Invoke();
            Debug.Log($"�{����Ĳ�o�F���s {InventoryButton.name} �� OnClick �ƥ�C");
        }
        videoPlayer.Play();

        Debug.Log("�}�l����v���I");
    }

}


