using UnityEngine;
using System.Collections;

public class SceneAnimationInitializer : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        StartCoroutine(PlayAnimationWithDelay());
    }

    IEnumerator PlayAnimationWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // ���ݳ������Jí�w
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("start");
            Debug.Log("Triggered animation after delay: start");
        }
        else
        {
            Debug.LogError("Animator component not found!");
        }
    }

}
