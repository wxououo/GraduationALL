using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HintDistanceCheck : MonoBehaviour
{
    public float maxDistance = 3f; // 最大偵測距離
    public LayerMask hintLayer; // 只偵測提示物件
    //public GameObject hintUI; // 提示 UI 物件（UI Canvas 上的 Text）
    public TextMeshProUGUI hintText;  // 提示 UI 文字
    public Image backgroundImage;
    private Camera playerCamera;       // 玩家視角的相機
    private bool isUnlockHintShown = false;  // 紀錄是否顯示過破關提示

    [System.Serializable]
    public class TagHintPair
    {
        public string tag;
        [TextArea] public string hintMessage;
    }

    public List<TagHintPair> tagHintMapping = new List<TagHintPair>(); // 可在 Inspector 設定
    private Dictionary<string, string> hintDictionary = new Dictionary<string, string>();

    void Start()
    {
        playerCamera = Camera.main;   // 取得主攝影機
        hintText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);
        if (PlayerPrefs.GetInt("IsUnlocked", 0) == 1)
        {

            StartCoroutine(ShowUnlockHint("Something come out!", 1f));
            isUnlockHintShown = true;
        }
        // 將 tagHintMapping 轉換成 Dictionary，加快查找速度
        foreach (var pair in tagHintMapping)
        {
            hintDictionary[pair.tag] = pair.hintMessage;
        }
    }
    private void Update()
    {
        if (isUnlockHintShown) return;
        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        RaycastHit hit;
        float sphereRadius = 100f;
        Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.red);

        if (Physics.SphereCast(ray, sphereRadius, out hit, maxDistance, hintLayer))
        {
            if (hintDictionary.TryGetValue(hit.collider.tag, out string hint))
            {
                ShowHint(hint);
            }
            else
            {
                HideHint();
            }
        }
        else
        {
            HideHint();
        }
    }

    IEnumerator ShowUnlockHint(string message, float duration)
    {
        isUnlockHintShown = true;
        ShowHint(message);
        yield return new WaitForSeconds(duration);
        HideHint();
        isUnlockHintShown = false;  // 解除限制，恢復射線偵測
    }
    public void ShowHint(string message)
    {
        hintText.text = message;
        hintText.gameObject.SetActive(true);
        backgroundImage.gameObject.SetActive(true);

    }

    public void HideHint()
    {

        hintText.gameObject.SetActive(false);
        backgroundImage.gameObject.SetActive(false);

    }
}
