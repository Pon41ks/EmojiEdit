using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class SaveSystem : MonoBehaviour
{
    [SerializeField] private GameObject newPrefab;
    [SerializeField] private RectTransform mainCanvas;

    private string localPath;
    
    

    public void CreatePrefab(int index)
    {
        
        
            if (!Directory.Exists("Assets/Prefabs"))
            {
                AssetDatabase.CreateFolder("Assets", "Prefabs");
            }

            string number = index.ToString();
            localPath = "Assets/Prefabs/" +  number + ".Prefab";

            localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);
            bool prefabSuccess;
            gameObject.transform.SetParent(mainCanvas);
            PrefabUtility.SaveAsPrefabAssetAndConnect(gameObject, localPath, InteractionMode.UserAction,
                out prefabSuccess);
            if (prefabSuccess)
            {
                Debug.Log("Prefab was saved successfully");
            }
            else
            {
                Debug.Log("Prefab failed to save");
            }
        
        
    }

    public void OpenPrefab(int prefabNum)
    {
        LoadPrefab(prefabNum);
        
    }

    private GameObject LoadPrefab(int prefabNum)
    {
        string path = "Assets/Prefabs/" + prefabNum + ".Prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath(path, typeof(GameObject)) as GameObject;
        if (prefab != null)
        {
            GameObject emodji = Instantiate(prefab);
            emodji.transform.SetParent(mainCanvas);
            RectTransform rectTransform = emodji.GetComponent<RectTransform>();
            rectTransform.offsetMax = Vector3.zero;
            rectTransform.offsetMin = Vector3.zero;
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition3D = -Vector3.one;
            rectTransform.pivot = Vector2.one * 0.5f;
            emodji.transform.localScale = new Vector3(1, 1 ,1 );
            return emodji;
        }
        else
        {
            Debug.LogError("Failed to load prefab at path: " + path);
            return null;
        }
    }
}