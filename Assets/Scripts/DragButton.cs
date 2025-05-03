using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;


public class DragButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject Button;
    public Canvas canvas;
    private RectTransform rectTransform;
    private Vector2 initialPosition;
    public string sceneName1;
    public RectTransform targetArea; // 目標區域（拖動到這裡時觸發切換）
    public string sceneName2;
    public RectTransform targetArea2;

    public float minY = -100f;  
    public float maxY = 100f;  


    private MouseLook mouseLook;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition; // 記錄按鈕初始位置
        mouseLook = FindObjectOfType<MouseLook>();
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mouseLook != null && Button.activeSelf)
        {
            mouseLook.SetDraggingState(true); // 鎖定畫面旋轉
        }
        FindObjectOfType<MouseLook>().SetDraggingState(true);
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 newPosition = rectTransform.anchoredPosition + eventData.delta / canvas.scaleFactor;

        newPosition.x = initialPosition.x;
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);

        rectTransform.anchoredPosition = newPosition; // 更新位置
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (RectTransformUtility.RectangleContainsScreenPoint(targetArea, eventData.position, Camera.main))
        {
            SwitchEra(sceneName1); // 觸發切換
            mouseLook.SetDraggingState(false);
            Debug.Log("change time" );
        }
        else if (RectTransformUtility.RectangleContainsScreenPoint(targetArea2, eventData.position, Camera.main))
        {
            SwitchEra(sceneName2); // 觸發切換
            mouseLook.SetDraggingState(false);
            Debug.Log("change time");
        }
        else
        {
                mouseLook.SetDraggingState(false); // 解除鎖定
                ReturnToStartPosition();
        }
        FindObjectOfType<MouseLook>().SetDraggingState(false);
    }
    private void ReturnToStartPosition()
    {
        rectTransform.anchoredPosition = initialPosition;
    }
    private void SwitchEra(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
