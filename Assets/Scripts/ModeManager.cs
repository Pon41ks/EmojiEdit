using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModeManager : MonoBehaviour
{
    public enum Mode { Drawing, EditingText }
    public Mode currentMode = Mode.Drawing;

    private TouchInputHandler touchInputHandler;
    private LineDrawer drawingHandler;

    private void Awake()
    {
        touchInputHandler = FindObjectOfType<TouchInputHandler>();
        drawingHandler = FindObjectOfType<LineDrawer>();
    }

    private void Update()
    {
        // Проверяем текущий режим и включаем/выключаем соответствующие обработчики ввода
        if (currentMode == Mode.EditingText)
        {
            touchInputHandler.enabled = true;
            drawingHandler.drawingEnabled = false;
        }
        else if (currentMode == Mode.Drawing)
        {
            touchInputHandler.enabled = false;
            drawingHandler.enabled = true;
        }
    }

    // Метод для переключения режима
    public void SwitchMode()
    {
        if (currentMode == Mode.Drawing)
        {
            currentMode = Mode.EditingText;
        }
        else
        {
            currentMode = Mode.Drawing;
        }
        
    }
}
