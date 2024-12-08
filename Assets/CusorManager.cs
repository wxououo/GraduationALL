using UnityEngine;

public class ZoomCursorHandler : MonoBehaviour
{
    public Texture2D zoomCursorTexture; 
    public Texture2D PickCursorTexture; 
    private Vector2 cursorHotspot; // ���Ъ��J�I��m
    private Texture2D defaultCursorTexture; // �w�]�ƹ�����
    private Vector2 defaultHotspot;
    public bool requiresZoom = false; // �O�_�ݭn���Y��j�~���������
    private MouseLook cameraController; // �Ψ��ˬd���Y�O�_�w�վ�

    void Start()
    {
        // �]�w���Ъ��J�I���Ϥ�����
        cursorHotspot = new Vector2(zoomCursorTexture.width / 2, zoomCursorTexture.height / 2);

        // �O�s�t�Ϊ��w�]�ƹ�����
        defaultCursorTexture = null; // �t���q�{����
        defaultHotspot = Vector2.zero;

        cameraController = Camera.main.GetComponent<MouseLook>();
    }

    void Update()
    {
        // �إ� Ray ���˴��ƹ����V������
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // �p�G���������󦳯S�w�� Tag�]�Ҧp "ZoomTarget"�^
            if (hit.collider.CompareTag("ZoomTarget")&& !cameraController.HasAdjustedCamera)
            {
                Cursor.SetCursor(zoomCursorTexture, cursorHotspot, CursorMode.Auto); // �]�m�ۭq�ƹ�����
                return;
            }
            if (hit.collider.CompareTag("Pickup") && cameraController.HasAdjustedCamera)
            {
                Cursor.SetCursor(PickCursorTexture, cursorHotspot, CursorMode.Auto); // �]�m�ۭq�ƹ�����
                return;
            }
        }

        // �p�G�S�������ؼСA��_���w�]����
        Cursor.SetCursor(defaultCursorTexture, defaultHotspot, CursorMode.Auto);
    }
}
