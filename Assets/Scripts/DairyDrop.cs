using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class ClickToTriggerStory : MonoBehaviour
{


    [SerializeField] private GameObject objectToReveal;

    private bool triggered = false;

    private void OnMouseDown()
    {
        if (triggered) return;
        triggered = true;

        // 隱藏當前物件
        gameObject.SetActive(false);

        // 顯示劇情 UI
        if (objectToReveal != null)
            objectToReveal.SetActive(true);
    }

    //private IEnumerator ShowStoryThenReveal()
    //{


    //    yield return new WaitForSeconds(0.1f); // 等待劇情顯示時間

    //    // 隱藏劇情 UI


    //    // 顯示另一個物件

    //}
}
