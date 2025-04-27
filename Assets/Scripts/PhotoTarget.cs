using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoTarget : MonoBehaviour
{
    public string photoName; // 允许开发者在 Inspector 中设置物体名称
    public string photoId; // 允许开发者在 Inspector 中设置物体名称
    public string overridePhotoResourceName;
    public GameObject rewardPrefab;
    public string itemName;
    public Sprite IntroductionUI;
}
