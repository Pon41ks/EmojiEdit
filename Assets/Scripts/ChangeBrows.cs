using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using  UnityEngine.UI;

public class ChangeBrows : MonoBehaviour
{
    [SerializeField] private GameObject browsImage;
    [SerializeField] private Image[] pickedObj;
    [SerializeField] SpriteRenderer imageComponent; // Ссылка на компонент Image объекта
    [SerializeField] Sprite newSprite; // Новый спрайт для установки на объект

    private void Start()
    {
        imageComponent.GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite(int index)
    {
        ShowBackGroundPickedButton(index);
        browsImage.SetActive(true);
        // Проверяем, что компонент Image существует
        if (imageComponent != null)
        {
            // Устанавливаем новый спрайт
            imageComponent.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("Image component is not assigned.");
        }
    }

    

    private void ShowBackGroundPickedButton(int index)
    {
        foreach (var item in pickedObj)
        {
            Color allColor = item.color;
            allColor.a = 0;
            item.color = allColor;
        }

        Color color = pickedObj[index].color;
            color.a = 1;
            pickedObj[index].color = color; 
        
    }

    public void ClearElemnts()
    {
        browsImage.SetActive(false);
        foreach (var item in pickedObj)
        {
            Color allColor = item.color;
            allColor.a = 0;
            item.color = allColor;
        }
    }

    public void OffBrows()
    {
        browsImage.SetActive(false);
    }
}
