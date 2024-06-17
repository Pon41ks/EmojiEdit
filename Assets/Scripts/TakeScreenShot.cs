using UnityEngine;
using System.IO;

public class TakeScreenShot : MonoBehaviour
{
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RectTransform drawArea;
    [SerializeField] private string fileName = "screenShot" ;

    
    private Texture2D GetDrawnTextureFromDrawArea()
    {
        int width = (int) drawArea.rect.width;
        int height = (int) drawArea.rect.height;

        Texture2D drawnTexture = new Texture2D(width, height, TextureFormat.RGBA32, false);

        RenderTexture currentRT = RenderTexture.active;

        RenderTexture renderTexture = new RenderTexture(width, height, 24);
        renderTexture.Create();

        RenderTexture.active = renderTexture;

        renderCamera.targetTexture = renderTexture;
        renderCamera.Render();

        drawnTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        drawnTexture.Apply();

        renderCamera.targetTexture = null;
        RenderTexture.active = currentRT;

        Destroy(renderTexture);

        return drawnTexture;
    }

    public void SaveDrawnImageToGallery()
    {
        Texture2D drawnTexture = GetDrawnTextureFromDrawArea();

        if (drawnTexture == null)
        {
            Debug.LogError("Не удалось получить текстуру с нарисованным изображением.");
            return;
        }

        byte[] bytes = drawnTexture.EncodeToPNG();

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(bytes, "My Gallery", fileName + ".png",
            (success, path) =>
            {
                if (success)
                {
                    Debug.Log("Изображение успешно сохранено в галерею по пути: " + path);
                }
                else
                {
                    Debug.LogError("Не удалось сохранить изображение в галерею.");
                }
            });

        if (permission == NativeGallery.Permission.Denied)
        {
            Debug.LogError("Не удалось получить разрешение на доступ к галерее.");
        }
    }
}
