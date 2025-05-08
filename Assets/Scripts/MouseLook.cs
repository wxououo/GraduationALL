using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public class MouseLook : MonoBehaviour
{
    private float x;
    private float y;
    public float sensitivityX = -1f;
    public float sensitivityY = 0.5f;
    private Vector3 rotate;
    private bool isMousePressed = false;

    private bool isZooming = false;

    private bool hasAdjustedCamera = false;
    public bool HasAdjustedCamera
    {
        get { return hasAdjustedCamera; }
    }

    public Vector3 originalPosition;  // 鏡頭的初始位置
    private Quaternion originalRotation;  // 鏡頭的初始旋轉

    private Vector3 lastAdjustedPosition;  // 上一次調整前的位置
    private Quaternion lastAdjustedRotation;  // 上一次調整前的旋轉

    private Vector3 initialMousePosition;  // 滑鼠按下的初始位置
    private float clickThreshold = 5.0f;   // 滑鼠移動的距離閾值

    public Vector2 zoomYawLimit = new Vector2(-30f, 30f); // 左右 (Y 軸)
    public Vector2 zoomPitchLimit = new Vector2(-15f, 15f); // 上下 (X 軸)

    private Vector3 zoomBaseRotation; // Zoom 當下的基礎角度

    // 控制道具欄是否開啟的變數
    private bool isInventoryOpen = false;
    private GameObject mirrorObject;
    //public void SetInventoryState(bool state)
    //{
    //    isInventoryOpen = state;
    //}

    public bool isDraggingButton { get; private set; } = false;
    public void SetDraggingState(bool state)
    {
        isDraggingButton = state;
        //Debug.Log("Dragging State Changed: " + isDraggingButton + " at " + Time.time);
        System.Diagnostics.StackTrace stackTrace = new System.Diagnostics.StackTrace();
        Debug.Log("Call Stack:\n" + stackTrace.ToString());
        if (state) Debug.Log("目前拖曳物件：" + EventSystem.current.currentSelectedGameObject);
    }

    void Start()
    {
        Cursor.visible = true;
        originalPosition = Camera.main.transform.position;
        originalRotation = Camera.main.transform.rotation;
        mirrorObject = GameObject.Find("mirror (2)");
        if (!PlayerPrefs.HasKey("IntroShown"))
        {
            DialogueManager.Instance.ShowDialogue("……怎麼又是那個夢,\n那個男人到底是誰?");
            PlayerPrefs.SetInt("IntroShown", 1);
            PlayerPrefs.Save();
        }
    }

    void Update()
    {
        if (Camera.main == null)
        {
            Debug.LogError("Main Camera not found.");
            return;
        }
        if (isDraggingButton)
        {
            Debug.Log("Dragging detected, stopping camera movement.");
            return;
        }
        if (isDraggingButton && !Input.GetMouseButton(0))
        {
            Debug.LogWarning("Detected mouse released but dragging still true, auto-resetting.");
            SetDraggingState(false);
        }
        y = Input.GetAxis("Mouse X");
        x = Input.GetAxis("Mouse Y");
        rotate = new Vector3(x * sensitivityX, y * sensitivityY, 0);

        // 當按下滑鼠左鍵
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            initialMousePosition = Input.mousePosition; // 記錄滑鼠按下的初始位置
        }
        // 當釋放滑鼠左鍵
        else if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            isZooming = false;

            // 計算滑鼠移動距離
            float mouseDistance = Vector3.Distance(initialMousePosition, Input.mousePosition);
            if (mouseDistance <= clickThreshold) // 若滑鼠移動距離小於閾值，才執行點擊
            {
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    PointerEventData pointerData = new PointerEventData(EventSystem.current)
                    {
                        position = Input.mousePosition
                    };
                    List<RaycastResult> raycastResults = new List<RaycastResult>();
                    EventSystem.current.RaycastAll(pointerData, raycastResults);

                    foreach (RaycastResult result in raycastResults)
                    {
                        if (result.gameObject.CompareTag("IgnoreZoomClick"))
                        {
                            Debug.Log("點擊在特定 UI 上，取消鏡頭操作");
                            return;
                        }
                    }
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    Transform zoomTarget = hit.transform.Find("ZoomTarget");
                    if (zoomTarget != null && !hasAdjustedCamera)
                    {
                        // 在調整鏡頭前記錄當前位置和旋轉
                        lastAdjustedPosition = Camera.main.transform.position;
                        lastAdjustedRotation = Camera.main.transform.rotation;

                        Camera.main.transform.position = zoomTarget.position;
                        Camera.main.transform.rotation = zoomTarget.rotation;
                        hasAdjustedCamera = true;
                        //isZooming = true;
                        if (hit.transform.name == "dressing_table")
                        {
                            // 啟用 mirror(2) 的可見性
                            if (mirrorObject != null)
                            {
                                MirrorVisibility mirrorVisibility = mirrorObject.GetComponent<MirrorVisibility>();
                                if (mirrorVisibility != null)
                                {
                                    mirrorVisibility.SetVisibility(true);
                                }
                            }
                        }
                        zoomBaseRotation = transform.eulerAngles; // 記錄當下角度作為限制中心
                    }
                }
            }
        }

        // 當按下滑鼠右鍵返回到上一次調整前的位置
        if (Input.GetMouseButtonDown(1) && hasAdjustedCamera)
        {
            Camera.main.transform.position = lastAdjustedPosition;
            Camera.main.transform.rotation = lastAdjustedRotation;
            hasAdjustedCamera = false;
            if (mirrorObject != null)
            {
                MirrorVisibility mirrorVisibility = mirrorObject.GetComponent<MirrorVisibility>();
                if (mirrorVisibility != null)
                {
                    mirrorVisibility.SetVisibility(false);
                }
            }
        }

        // 拖動視角
        if (isMousePressed)
        {
            Vector3 rotationDelta = new Vector3(x * sensitivityX, y * sensitivityY, 0);

            if (hasAdjustedCamera)
            {
                // 計算限制後的新角度
                Vector3 newRotation = transform.eulerAngles - rotationDelta;

                float pitch = ClampAngle(newRotation.x, zoomBaseRotation.x + zoomPitchLimit.x, zoomBaseRotation.x + zoomPitchLimit.y);
                float yaw = ClampAngle(newRotation.y, zoomBaseRotation.y + zoomYawLimit.x, zoomBaseRotation.y + zoomYawLimit.y);

                transform.eulerAngles = new Vector3(pitch, yaw, 0);
            }
            else if (!hasAdjustedCamera)
            {
                transform.eulerAngles = transform.eulerAngles - rotationDelta;
            }
        }
    }
    private float ClampAngle(float angle, float min, float max)
{
    angle = angle % 360;
    if (angle < 0) angle += 360;

    min = min % 360;
    if (min < 0) min += 360;

    max = max % 360;
    if (max < 0) max += 360;

    if (min < max)
        return Mathf.Clamp(angle, min, max);
    else
        return (angle > min || angle < max) ? angle : (Mathf.Abs(angle - min) < Mathf.Abs(angle - max) ? min : max);
}

}
