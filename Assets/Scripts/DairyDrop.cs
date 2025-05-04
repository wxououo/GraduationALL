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

        // ���÷�e����
        gameObject.SetActive(false);

        // ��ܼ@�� UI
        if (objectToReveal != null)
            objectToReveal.SetActive(true);
    }

    //private IEnumerator ShowStoryThenReveal()
    //{


    //    yield return new WaitForSeconds(0.1f); // ���ݼ@����ܮɶ�

    //    // ���ü@�� UI


    //    // ��ܥt�@�Ӫ���

    //}
}
