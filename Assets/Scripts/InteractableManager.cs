using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance;
    public VideoPlayer videoPlayer;             // 綁定你的 VideoPlayer 組件
    public Renderer tvScreenRenderer;           // 電視螢幕的 Renderer，用於控制材質
    public Material offScreenMaterial;          // 關閉時的螢幕材質
    public Material onScreenMaterial;           // 打開時的螢幕材質

    private bool isVideoPlaying = false;        // 記錄影片是否正在播放
    public float snapThreshold = 2f;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 初始化時讓電視處於關閉狀態
        if (tvScreenRenderer != null && offScreenMaterial != null)
        {
            tvScreenRenderer.material = offScreenMaterial;
        }
    }

    public void PlayVideo(GameObject interactableObject)
    {
       
        if (isVideoPlaying) return;

        if (videoPlayer != null && tvScreenRenderer != null)
        {
            Debug.Log("播放影片函式被呼叫");
            // 切換螢幕材質
            Material newOnScreenMaterial = new Material(onScreenMaterial);
            tvScreenRenderer.material = newOnScreenMaterial;

            videoPlayer.targetMaterialRenderer = tvScreenRenderer;
            videoPlayer.targetMaterialProperty = "_MainTex";
            // 播放影片
            videoPlayer.Play();
            isVideoPlaying = true;

            // 儲存播放紀錄
            PlayerPrefs.SetInt("IsTVPlayed", 1);
            PlayerPrefs.Save();
        }
    }

    public void HandleInteraction(Item item, GameObject interactableObject)
    {
        InteractableManager tv = interactableObject.GetComponent<InteractableManager>();
        if (tv != null)
        {
            float distance = Vector3.Distance(interactableObject.transform.position, this.transform.position);
            if (distance <= snapThreshold)
            {
                tv.PlayVideo(interactableObject); // 播放影片
            }
        }
    }
}
