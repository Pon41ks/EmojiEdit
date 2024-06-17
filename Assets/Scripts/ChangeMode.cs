using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeMode : MonoBehaviour
{
    [SerializeField] private GameObject[] modes;
    [SerializeField] private Image[] buttonsPicked;
    [SerializeField] private Image[] buttons;
    [SerializeField] private Sprite defaultSprite;
    [SerializeField] private Sprite pickedSprite;

    public void PickMode(int index)
    {
        foreach (var item in modes)
        {
            item.SetActive(false);
        }
        modes[index].SetActive(true);
        PickButton(index);
    }

    private void PickButton(int index)
    {
        foreach (var item in buttonsPicked)
        {
            Color allColor = item.color;
            allColor.a = 0;
            item.color = allColor;
        }

        Color color = buttonsPicked[index].color;
        color.a = 1;
        buttonsPicked[index].color = color;
        OffButtonImage(index);
    }

    private void OffButtonImage(int index)
    {
        foreach (var item in buttons)
        {
            Color color = item.color;
            color.a = 1;
            item.color = color;
        }

        Color sss = buttons[index].color;
        sss.a = 0;
        buttons[index].color = sss;
    }
}
