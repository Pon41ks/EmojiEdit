using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TextInputHandler : MonoBehaviour
{
    [SerializeField] private RectTransform UIElement; // Единственный элемент UI
    [SerializeField] private RectTransform parentRect;

    private Vector2 originalPosition; // Оригинальная позиция элемента

    private InputAction primaryContactAction;
    private InputAction primaryPositionAction;

    private bool isTouchingUIElement;
    private Vector2 touchOffset;

    private float maxMovementDistance = 1000f; // Максимальное расстояние перемещения в пикселях (увеличено для теста)

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
        // Сохранение оригинальной позиции элемента
        originalPosition = UIElement.anchoredPosition;
        Debug.Log($"Элемент {UIElement.name} оригинальная позиция: {originalPosition}");
    }

    private void OnPrimaryTouchStarted()
    {
        Vector2 touchPosition = primaryPositionAction.ReadValue<Vector2>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, touchPosition, Camera.main, out localPoint);

        if (RectTransformUtility.RectangleContainsScreenPoint(UIElement, touchPosition, Camera.main))
        {
            isTouchingUIElement = true;
            // Преобразуем Vector3 anchoredPosition в Vector2 для выполнения операции вычитания
            Vector2 anchoredPosition2D = new Vector2(UIElement.anchoredPosition.x, UIElement.anchoredPosition.y);
            touchOffset = localPoint - anchoredPosition2D;
            Debug.Log($"Начато касание элемента {UIElement.name} в точке {localPoint}, смещение касания {touchOffset}");
        }
    }

    private void OnPrimaryTouchEnded()
    {
        isTouchingUIElement = false;
        Debug.Log("Касание завершено");
    }

    private void OnPrimaryTouchPosition(InputAction.CallbackContext context)
    {
        if (isTouchingUIElement)
        {
            Vector2 touchPosition = context.ReadValue<Vector2>();

            // Переводим экранные координаты в локальные координаты RectTransform родительского элемента
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRect, touchPosition, Camera.main, out Vector2 localPoint);

            Debug.Log($"Текущая локальная точка: {localPoint}, смещение касания: {touchOffset}");

            // Обновляем позицию элемента, вычитая смещение касания
            Vector2 newPosition = localPoint - touchOffset;

            Debug.Log($"Новое положение до ограничения: {newPosition}");

            // Ограничиваем новую позицию в пределах родительского прямоугольника
            newPosition.x = Mathf.Clamp(newPosition.x, parentRect.rect.min.x, parentRect.rect.max.x);
            newPosition.y = Mathf.Clamp(newPosition.y, parentRect.rect.min.y, parentRect.rect.max.y);

            Debug.Log($"Новое положение после ограничения: {newPosition}");

            // Обновляем позицию элемента
            UIElement.anchoredPosition = newPosition;

            // Ограничение на максимальное расстояние перемещения
            float distance = Vector2.Distance(UIElement.anchoredPosition, originalPosition);
            Debug.Log($"Расстояние перемещения: {distance}");

            if (distance > maxMovementDistance)
            {
                Vector2 direction = (UIElement.anchoredPosition - originalPosition).normalized;
                UIElement.anchoredPosition = originalPosition + direction * maxMovementDistance;
                Debug.Log($"Элемент перемещен на максимальное расстояние: {maxMovementDistance}");
            }
            else
            {
                Debug.Log($"Элемент перемещен в позицию: {UIElement.anchoredPosition}");
            }
        }
    }
}
