using UnityEngine;

public class MirrorVisibility : MonoBehaviour
{
    private bool isVisible = false;
    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null)
        {
            meshRenderer.enabled = false;
        }
    }

    public void SetVisibility(bool visible)
    {
        isVisible = visible;
        if (meshRenderer != null)
        {
            meshRenderer.enabled = visible;
        }
    }
} 