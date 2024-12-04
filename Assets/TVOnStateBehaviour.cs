using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TVOnStateBehaviour : StateMachineBehaviour
{
    public Material onScreenMaterial; // 開啟後的螢幕材質

    // 在狀態進入時調用
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // 獲取 InteractableManager 並切換螢幕材質
        InteractableManager interactableManager = animator.GetComponent<InteractableManager>();

        if (interactableManager != null && interactableManager.tvScreenRenderer != null)
        {
            interactableManager.tvScreenRenderer.material = interactableManager.onScreenMaterial;
            Debug.Log("TV screen material changed to 'On' state.");
        }
    }
}
