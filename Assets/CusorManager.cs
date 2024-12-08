using UnityEngine;

public class ZoomCursorHandler : MonoBehaviour
{
    public Texture2D zoomCursorTexture; 
    public Texture2D PickCursorTexture; 
    private Vector2 cursorHotspot; // 指標的焦點位置
    private Texture2D defaultCursorTexture; // 預設滑鼠指標
    private Vector2 defaultHotspot;
    public bool requiresZoom = false; // 是否需要鏡頭放大才能切換場景
    private MouseLook cameraController; // 用來檢查鏡頭是否已調整

    void Start()
    {
        // 設定指標的焦點為圖片中心
        cursorHotspot = new Vector2(zoomCursorTexture.width / 2, zoomCursorTexture.height / 2);

        // 保存系統的預設滑鼠指標
        defaultCursorTexture = null; // 系統默認指標
        defaultHotspot = Vector2.zero;

        cameraController = Camera.main.GetComponent<MouseLook>();
    }

    void Update()
    {
        // 建立 Ray 來檢測滑鼠指向的物件
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 如果擊中的物件有特定的 Tag（例如 "ZoomTarget"）
            if (hit.collider.CompareTag("ZoomTarget")&& !cameraController.HasAdjustedCamera)
            {
                Cursor.SetCursor(zoomCursorTexture, cursorHotspot, CursorMode.Auto); // 設置自訂滑鼠指標
                return;
            }
            if (hit.collider.CompareTag("Pickup") && cameraController.HasAdjustedCamera)
            {
                Cursor.SetCursor(PickCursorTexture, cursorHotspot, CursorMode.Auto); // 設置自訂滑鼠指標
                return;
            }
        }

        // 如果沒有擊中目標，恢復為預設指標
        Cursor.SetCursor(defaultCursorTexture, defaultHotspot, CursorMode.Auto);
    }
}
