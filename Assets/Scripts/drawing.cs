using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NativeGalleryNamespace;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor;

public class LineDrawer : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private GameObject linePrefab = null;
    [SerializeField] private Camera renderCamera;
    [SerializeField] private RectTransform drawArea;
    [SerializeField] private Slider sizeSlider;
    [SerializeField] private FlexibleColorPicker fcp;

    public bool drawingEnabled = true;

    private LineRenderer currentLine;
    private List<GameObject> images = new List<GameObject>();
    private List<GameObject> drawnObjects = new List<GameObject>();
    private List<GameObject> removedObjects = new List<GameObject>();
    private Vector2 lastPos;
    private InputAction drawAction;
    private bool isNewPoint = false;
    private int lineLayer;
    private string fileName = "Emoji";
    private bool isImageOnScene;

    private void Start()
    {
        if (sceneCamera == null)
        {
            Debug.LogError("Camera is not assigned!");
            return;
        }

        if (linePrefab == null)
        {
            Debug.LogError("Line prefab is not assigned!");
            return;
        }

        var inputActionMap = new InputActionMap("DrawMap");
        drawAction = inputActionMap.AddAction("Draw", binding: "<Pointer>/position");
        drawAction.performed += context => Draw(context.ReadValue<Vector2>());
        drawAction.Enable();
    }

    #region Draw

    private void CreateNewLine(Vector2 pos)
    {
        GameObject brush = Instantiate(linePrefab);
        brush.transform.SetParent(drawArea);
        currentLine = brush.GetComponent<LineRenderer>();
        Vector2 mousePos = sceneCamera.ScreenToWorldPoint(pos);
        float sliderValue = sizeSlider.value;
        currentLine.startWidth = sliderValue;
        currentLine.sortingOrder = lineLayer;
        currentLine.material.color = fcp.color;
        currentLine.SetPosition(0, mousePos);
        currentLine.SetPosition(1, mousePos);
        drawnObjects.Add(brush);
    }

    private void Draw(Vector2 pos)
    {
        if (DrawManager.isEmojiPanelOpened) return;


        if (IsInsideDrawArea(pos))
        {
            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                if (!isNewPoint)
                {
                    CreateNewLine(pos);
                    lineLayer++;
                }
            }

            if (Touchscreen.current.primaryTouch.press.isPressed)
            {
                Vector2 mousePos = sceneCamera.ScreenToWorldPoint(pos);
                if (mousePos != lastPos)
                {
                    lastPos = mousePos;
                    AddPoint(mousePos);
                }
            }
            else
            {
                currentLine = null;
            }
        }
    }

    private void AddPoint(Vector2 touchPosition)
    {
        isNewPoint = true;
        currentLine.positionCount++;
        int positionIndex = currentLine.positionCount - 1;
        currentLine.SetPosition(positionIndex, touchPosition);
    }

    private bool IsInsideDrawArea(Vector2 screenPosition)
    {
        Vector2 worldPos = sceneCamera.ScreenToWorldPoint(screenPosition);
        Vector2 localPos = drawArea.InverseTransformPoint(worldPos);
        return drawArea.rect.Contains(localPos);
    }

    public void RemoveLastLine()
    {
        if (drawnObjects.Count > 0)
        {
            GameObject removedObject = drawnObjects[drawnObjects.Count - 1];
            removedObject.SetActive(false);
            drawnObjects.RemoveAt(drawnObjects.Count - 1);
            removedObjects.Add(removedObject);
        }
    }

    public void ReturnRemoved()
    {
        if (removedObjects.Count > 0)
        {
            GameObject restoredObject = removedObjects[removedObjects.Count - 1];
            restoredObject.SetActive(true);
            removedObjects.RemoveAt(removedObjects.Count - 1);
            drawnObjects.Add(restoredObject);
        }
    }

    #endregion

    #region SaveAndLoad

    public void TakeScreenshot()
    {
        if (isImageOnScene)
        {
            StartCoroutine(SaveScreenshot());
        }
        else
        {
            SaveDrawnImageToGallery();
        }
    }

    private IEnumerator SaveScreenshot()
    {
        yield return new WaitForEndOfFrame();
        // Convert capture area's screen coordinates to texture coordinates
        Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(null, drawArea.position);
        Rect captureRect = new Rect(screenPos.x, screenPos.y, drawArea.rect.width, drawArea.rect.height);
        Vector2 texturePos = new Vector2(captureRect.x / Screen.width, captureRect.y / Screen.height);
        Vector2 textureSize = new Vector2(captureRect.width / Screen.width, captureRect.height / Screen.height);

        // Create a texture to hold the screenshot
        Texture2D screenshotTexture =
            new Texture2D((int) captureRect.width, (int) captureRect.height, TextureFormat.RGB24, false);

        // Capture the specified area of the screen
        sceneCamera.targetTexture = RenderTexture.GetTemporary(screenshotTexture.width, screenshotTexture.height, 16);
        sceneCamera.Render();
        RenderTexture.active = sceneCamera.targetTexture;
        screenshotTexture.ReadPixels(new Rect(0, 0, captureRect.width, captureRect.height), 0, 0);
        screenshotTexture.Apply();

        // Reset render targets
        sceneCamera.targetTexture = null;
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(sceneCamera.targetTexture);

        // Save or process the screenshot texture
        // For example, you can save it to a file
        /*
        byte[] bytes = screenshotTexture.EncodeToPNG();
        string fileName = "Screenshot.png";
        System.IO.File.WriteAllBytes(fileName, bytes);
*/
        // Destroy the temporary texture
        Destroy(screenshotTexture);

        NativeGallery.Permission permission = NativeGallery.SaveImageToGallery(screenshotTexture, "MyGallery",
            "Screenshot.png", (success, path) =>
            {
                if (success)
                {
                    Debug.Log("Screenshot saved to gallery");
                }
                else
                {
                    Debug.LogError("Failed to save screenshot to gallery");
                }

                // Освобождаем ресурсы текстуры
                Destroy(screenshotTexture);
            });
        Debug.Log("Screenshot saved as " + fileName);
    }


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
#if UNITY_EDITOR
        string desktopPath = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string filePath = Path.Combine(desktopPath, "screenshot.png");

        // Сохранение скриншота как файл изображения
        byte[] editorBytes = drawnTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, editorBytes);
        Debug.Log("Screenshot saved at: " + filePath);
#endif
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

    public void LoadImageFromGallery()
    {
        NativeGallery.Permission permission =
            NativeGallery.RequestPermission(NativeGallery.PermissionType.Read, NativeGallery.MediaType.Image);
        if (permission == NativeGallery.Permission.Granted)
        {
            NativeGallery.GetImageFromGallery((path) =>
            {
                if (path != null)
                {
                    Texture2D texture = NativeGallery.LoadImageAtPath(path);
                    if (texture != null)
                    {
                        GameObject imageObject = new GameObject("LoadedImage");
                        Image imageComponent = imageObject.AddComponent<Image>();
                        imageComponent.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height),
                            Vector2.one * 2);
                        imageObject.transform.SetParent(drawArea, false);
                        RectTransform rectTransform = imageObject.GetComponent<RectTransform>();
                        rectTransform.offsetMax = Vector3.zero;
                        rectTransform.offsetMin = Vector3.zero;
                        rectTransform.anchorMin = Vector2.zero;
                        rectTransform.anchorMax = Vector2.one;
                        rectTransform.pivot = Vector2.one * 0.5f; // Adjust pivot to center the image
                        drawnObjects.Add(imageObject);
                        isImageOnScene = true;
                    }
                    else
                    {
                        Debug.LogError("Failed to load texture from path: " + path);
                    }
                }
                else
                {
                    Debug.LogError("Failed to get image from gallery.");
                }
            }, "Select an image", "image/*");
        }
        else
        {
            Debug.LogError("Gallery access permission is not granted.");
        }
    }

    #endregion


    private void RemoveAllFromIndex(int index)
    {
        if (index >= 0 && index < removedObjects.Count)
        {
            int count = removedObjects.Count - index;
            removedObjects.RemoveRange(index, count);
            Debug.Log(removedObjects.Count);
        }
    }

    private void Update()
    {
        if (!Touchscreen.current.primaryTouch.press.isPressed)
        {
            isNewPoint = false;
        }

        if (removedObjects.Count >= 5)
        {
            //RemoveAllFromIndex(5);
        }
        else if (drawnObjects.Count >= 0 && removedObjects.Count > 3)
        {
            foreach (var item in removedObjects)
            {
                Destroy(item);
            }

            removedObjects.Clear();
            Debug.Log(removedObjects.Count);
        }


        if (images.Count == 0)
        {
            isImageOnScene = false;
        }
    }
}