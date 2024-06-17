using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.UI;

public class Constructor : MonoBehaviour
{
    [SerializeField] SpriteRenderer imageComponent; // Ссылка на компонент Image объекта
    [SerializeField] Sprite newSprite; // Новый спрайт для установки на объект

    private void Start()
    {
        imageComponent.GetComponent<SpriteRenderer>();
    }

    public void ChangeSprite()
    {
        // Проверяем, что компонент Image существует
        if (imageComponent != null)
        {
            Debug.Log(imageComponent);
            // Устанавливаем новый спрайт
            imageComponent.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning("Image component is not assigned.");
        }
    }
}
