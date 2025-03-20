using UnityEngine;

public class TargetObject : MonoBehaviour
{
    // 控制目標物件的顯示與隱藏
    public void ShowTarget()
    {
        gameObject.SetActive(true);  // 顯示目標物件
    }

    public void HideTarget()
    {
        gameObject.SetActive(false);  // 隱藏目標物件
    }
}
