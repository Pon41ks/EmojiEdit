using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TouchInputHandler : MonoBehaviour
{
    [SerializeField]private List<RectTransform> UIElements;
    [SerializeField] private List<Image> borders;
    [SerializeField] private RectTransform parentRect;
    
    private Vector2 originalPosition; // Оригинальная позиция элемента

    private InputAction primaryContactAction;
    private InputAction primaryPositionAction;

    private bool isTouchingUIElement;
    private Vector2 touchOffset;

    private float maxMovementDistance = 5f; // Максимальное расстояние перемещения в пикселях

    private RectTransform currentUIElement;
    private Image currentBorder;// Текущий перемещаемый объект

    private void Awake()
    {
        // Создаем InputActionMap
        var actionMap = new InputActionMap("Touch");

        // Определяем действия для первичного контакта
        primaryContactAction = actionMap.AddAction("PrimaryContact", binding: "<Touchscreen>/primaryTouch/press");
        primaryPositionAction = actionMap.AddAction("PrimaryPosition", binding: "<Touchscreen>/primaryTouch/position");

        // Включаем карту действий
        actionMap.Enable();

        // Привязываем обработчики событий
        primaryContactAction.started += ctx => OnPrimaryTouchStarted();
        primaryContactAction.canceled += ctx => OnPrimaryTouchEnded();
        primaryPositionAction.performed += ctx => OnPrimaryTouchPosition(ctx);
    }

    private void Start()
    {
        // Сохранение оригинального размера текста и позиции для всех объектов
        foreach (RectTransform element in UIElements)
        {
            originalPosition = element.anchoredPosition;
        }
    }

    private void OnPrimaryTouchStarted()
    {
        foreach (RectTransform element in UIElements)
        {
            Vector2 touchPosition = primaryPositionAction.ReadValue<Vector2>();
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(element.parent as RectTransform, touchPosition,
                Camera.main, out localPoint);

            if (RectTransformUtility.RectangleContainsScreenPoint(element, touchPosition, Camera.main))
            {
                isTouchingUIElement = true;
                SpriteRenderer elem = element.GetComponent<SpriteRenderer>();
                Color color = elem.color;
                color.a = 1;
                elem.color = color; 
                // Преобразуем Vector3 anchoredPosition в Vector2 для выполнения операции вычитания
                Vector2 anchoredPosition2D = new Vector2(element.anchoredPosition.x, element.anchoredPosition.y);
                touchOffset = localPoint - anchoredPosition2D;
                currentUIElement = element;
                break; // Выходим из цикла после нахождения первого подходящего элемента
            }
        }
    }


    private void OnPrimaryTouchEnded()
    {
        SpriteRenderer border = currentUIElement.GetComponent<SpriteRenderer>();
        Color color = border.color;
        color.a = 0;
        border.color = color;
        isTouchingUIElement = false;
        currentUIElement = null;
        
    }

    private void OnPrimaryTouchPosition(InputAction.CallbackContext context)
    {
        if (isTouchingUIElement && currentUIElement != null)
        {
            Vector2 touchPosition = context.ReadValue<Vector2>();

            // Переводим экранные координаты в локальные координаты RectTransform родительского элемента
            RectTransformUtility.ScreenPointToLocalPointInRectangle(currentUIElement.parent as RectTransform,
                touchPosition,
                Camera.main, out Vector2 localPoint);

            // Обновляем позицию элемента, вычитая смещение касания
            Vector2 newPosition = localPoint - touchOffset;

            // Ограничиваем новую позицию в пределах родительского прямоугольника
            newPosition.x = Mathf.Clamp(newPosition.x, parentRect.rect.min.x, parentRect.rect.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, parentRect.rect.min.y, parentRect.rect.max.y);

            // Обновляем позицию элемента
            currentUIElement.anchoredPosition = newPosition;

            // Ограничение на максимальное расстояние перемещения
            float distance = Vector2.Distance(currentUIElement.anchoredPosition, originalPosition);
            if (distance > maxMovementDistance)
            {
                Vector2 direction = (currentUIElement.anchoredPosition - originalPosition).normalized;
                currentUIElement.anchoredPosition = originalPosition + direction * maxMovementDistance;
            }
        }
    }
}
