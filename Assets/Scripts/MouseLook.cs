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

    public Vector3 originalPosition;  // ���Y����l��m
    private Quaternion originalRotation;  // ���Y����l����

    private Vector3 lastAdjustedPosition;  // �W�@���վ�e����m
    private Quaternion lastAdjustedRotation;  // �W�@���վ�e������

    private Vector3 initialMousePosition;  // �ƹ����U����l��m
    private float clickThreshold = 5.0f;   // �ƹ����ʪ��Z���H��

    public Vector2 zoomYawLimit = new Vector2(-30f, 30f); // ���k (Y �b)
    public Vector2 zoomPitchLimit = new Vector2(-15f, 15f); // �W�U (X �b)

    private Vector3 zoomBaseRotation; // Zoom ��U����¦����

    // ����D����O�_�}�Ҫ��ܼ�
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
        if (state) Debug.Log("�ثe�즲����G" + EventSystem.current.currentSelectedGameObject);
    }

    void Start()
    {
        Cursor.visible = true;
        originalPosition = Camera.main.transform.position;
        originalRotation = Camera.main.transform.rotation;
        mirrorObject = GameObject.Find("mirror (2)");
        if (!PlayerPrefs.HasKey("IntroShown"))
        {
            DialogueManager.Instance.ShowDialogue("�K�K���S�O���ӹ�,\n���Өk�H�쩳�O��?");
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

        // ����U�ƹ�����
        if (Input.GetMouseButtonDown(0))
        {
            isMousePressed = true;
            initialMousePosition = Input.mousePosition; // �O���ƹ����U����l��m
        }
        // ������ƹ�����
        else if (Input.GetMouseButtonUp(0))
        {
            isMousePressed = false;
            isZooming = false;

            // �p��ƹ����ʶZ��
            float mouseDistance = Vector3.Distance(initialMousePosition, Input.mousePosition);
            if (mouseDistance <= clickThreshold) // �Y�ƹ����ʶZ���p���H�ȡA�~�����I��
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
                            Debug.Log("�I���b�S�w UI �W�A�������Y�ާ@");
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
                        // �b�վ����Y�e�O����e��m�M����
                        lastAdjustedPosition = Camera.main.transform.position;
                        lastAdjustedRotation = Camera.main.transform.rotation;

                        Camera.main.transform.position = zoomTarget.position;
                        Camera.main.transform.rotation = zoomTarget.rotation;
                        hasAdjustedCamera = true;
                        //isZooming = true;
                        if (hit.transform.name == "dressing_table")
                        {
                            // �ҥ� mirror(2) ���i����
                            if (mirrorObject != null)
                            {
                                MirrorVisibility mirrorVisibility = mirrorObject.GetComponent<MirrorVisibility>();
                                if (mirrorVisibility != null)
                                {
                                    mirrorVisibility.SetVisibility(true);
                                }
                            }
                        }
                        zoomBaseRotation = transform.eulerAngles; // �O����U���ק@�������
                    }
                }
            }
        }

        // ����U�ƹ��k���^��W�@���վ�e����m
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

        // ��ʵ���
        if (isMousePressed)
        {
            Vector3 rotationDelta = new Vector3(x * sensitivityX, y * sensitivityY, 0);

            if (hasAdjustedCamera)
            {
                // �p�⭭��᪺�s����
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
