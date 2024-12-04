using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ChangeToOriginalScene : MonoBehaviour
{
    public string originalSceneName; // 原始場景的名稱
    private bool shouldChangeScene = false; // 標誌變量，初始設為false

    void OnMouseDown()
    {
        if (shouldChangeScene)
        {
            SceneManager.LoadScene(originalSceneName);
        }
    }

    // 方法用於在特定情況下更改標誌變量
    public void SetShouldChangeScene(bool value)
    {
        shouldChangeScene = value;
    }
    public void ChangeSceneImmediately()
    {
        SceneManager.LoadScene(originalSceneName);
    }
}
