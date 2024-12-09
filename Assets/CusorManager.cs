using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] private Texture2D ZoomCursor; // Zoom目標的鼠標樣式
    [SerializeField] private Texture2D PickCursor; // 可拾取目標的鼠標樣式
    private Vector2 cursorHotspot;
    private MouseLook cameraController; // 檢查鏡頭是否已調整的腳本

    void Start()
    {
        cursorHotspot = new Vector2(ZoomCursor.width / 2, ZoomCursor.height / 2);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto); // 預設為默認鼠標樣式
        cameraController = Camera.main.GetComponent<MouseLook>();
    }

    void Update()
    {
        // 使用射線檢測鼠標下的物件
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 1️⃣ 檢測是否為 ZoomTarget 物件
            ZoomTarget zoomTarget = hit.collider.GetComponent<ZoomTarget>();
            if (zoomTarget != null && !cameraController.HasAdjustedCamera)
            {
                    Cursor.SetCursor(ZoomCursor, cursorHotspot, CursorMode.Auto);
                    return; // 提前返回，防止後面的判斷干擾
            }

            // 2️⃣ 檢測是否為 PickTarget 物件
            PickTarget pickTarget = hit.collider.GetComponent<PickTarget>();
            if (pickTarget != null)
            {
                // 判斷 PickTarget 是否需要拉近
                if (pickTarget.requiresZoom)
                {
                    if (cameraController != null && cameraController.HasAdjustedCamera)
                    {
                        Cursor.SetCursor(PickCursor, cursorHotspot, CursorMode.Auto);
                    }
                    else
                    {
                        // 如果需要拉近但未拉近，恢復默認樣式
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    }
                }
                else
                {
                    // 這個 PickTarget 不需要拉近，直接改變鼠標樣式
                    Cursor.SetCursor(PickCursor, cursorHotspot, CursorMode.Auto);
                }
                return; // 提前返回，防止後面的判斷干擾
            }
        }

        // 3️⃣ 如果沒有點擊到任何物件，將鼠標設回默認樣式
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}


