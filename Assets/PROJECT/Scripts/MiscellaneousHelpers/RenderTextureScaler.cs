using UnityEngine;

[ExecuteAlways]
public class RenderTextureScaler : MonoBehaviour
{
    [SerializeField] RenderTexture renderTexture;
    [SerializeField] float scale = 0.5f;
    [SerializeField] Vector2 referenceResolution = new Vector2(1920, 1080);

    private void Awake()
    {
        UpdateAspectRatio();
    }

    void Update()
    {
        UpdateAspectRatio();
    }

    private void UpdateAspectRatio()
    {
        if (renderTexture != null && Screen.width > 0 && Screen.height > 0)
        {
            float screenAspectRatio = (float)Screen.width / Screen.height;

            float width = referenceResolution.x * scale;
            float height = referenceResolution.y * scale;

            if (screenAspectRatio > (width / height))
            {
                width = height * screenAspectRatio;
            }
            else
            {
                height = width / screenAspectRatio;
            }

            renderTexture.Release();
            renderTexture.width = (int)width;
            renderTexture.height = (int)height;
            renderTexture.Create();

            Camera.main.ResetAspect();
        }
    }
}