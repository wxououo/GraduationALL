using UnityEngine;

public class TargetObject : MonoBehaviour
{
    // ����ؼЪ�����ܻP����
    public void ShowTarget()
    {
        gameObject.SetActive(true);  // ��ܥؼЪ���
    }

    public void HideTarget()
    {
        gameObject.SetActive(false);  // ���åؼЪ���
    }
}
