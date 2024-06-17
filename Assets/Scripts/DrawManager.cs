using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DrawManager : MonoBehaviour
{
    [Header("Emoji")]
    [SerializeField] private Animator animator;
    [SerializeField] private Image[] buttons;
    [SerializeField] private Image[] buttonsPicked;
    [SerializeField] private GameObject emojiPanel;
    [Header("Text")]
    [SerializeField] private GameObject textObj;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private TMP_InputField inputText;

    private bool isSettingsOpened = false;
    private bool isTextOpened = false;
    private bool isChanged;
    public static bool isEmojiPanelOpened { get; private set; }

    private void Update()
    {
        text.text = inputText.text;
    }
    
    #region Settings

    private void OpenEmodjiPanel()
    {
        
        if (!emojiPanel.activeInHierarchy)
        {
            emojiPanel.SetActive(true);
            isEmojiPanelOpened = true;
        }
        else
        {
            emojiPanel.SetActive(false);
            isEmojiPanelOpened = false;
        }
    }
    
    public void PlayAnimation(int indexOfbutton)
    {
        if (indexOfbutton == 0)
        {
            if (!isSettingsOpened && !isTextOpened)
            {
                Debug.Log("w");
                OpenEmodjiPanel();
            }
            else if (isSettingsOpened)
            {
                animator.SetTrigger("Close");
                isSettingsOpened = false;
                OpenEmodjiPanel();
            }
            else if (isTextOpened)
            {
                animator.SetTrigger("CloseText");
                isTextOpened = false;
                OpenEmodjiPanel();
            }
        }
        
        if (indexOfbutton == 1)
        {
            
            if (!isSettingsOpened && !isEmojiPanelOpened)
            {
                Debug.Log("s");
                ShowTextSettings(indexOfbutton);
            }
            else if (isSettingsOpened)
            {
                animator.SetTrigger("Close");
                isSettingsOpened = false;
                ShowTextSettings(indexOfbutton);
            }
            else if (isEmojiPanelOpened)
            {
                OpenEmodjiPanel();
                ShowTextSettings(indexOfbutton);
            }
            
        }

        if (indexOfbutton == 3)
        {
            if (!isTextOpened && !isEmojiPanelOpened)
            {
                Debug.Log("q");
                ShowBrushSettings(indexOfbutton);
            }
            else if (isTextOpened)
            {
                animator.SetTrigger("CloseText");
                isTextOpened = false;
                ShowBrushSettings(indexOfbutton);
            }
            else if (isEmojiPanelOpened)
            {
                OpenEmodjiPanel();
                ShowBrushSettings(indexOfbutton);
            }
        }
    }

    public void ShowBrushSettings(int index)
    {
        if (!isSettingsOpened)
        {
            PickButton(index);
            animator.SetTrigger("Open");
            isSettingsOpened = true;
        }
        else
        {
            animator.SetTrigger("Close");
            isSettingsOpened = false;
            OffImage(index);
        }
    }

    private void ShowTextSettings(int index)
    {
        if (!isTextOpened)
        {
            PickButton(index);
            animator.SetTrigger("OpenText");
            isTextOpened = true;
        }
        else
        {
            animator.SetTrigger("CloseText");
            isTextOpened = false;
            OffImage(index);
        }
    }

    private void PickButton(int index)
    {
        foreach (var item in buttonsPicked)
        {
            Color allColor = item.color;
            allColor.a = 0;
            item.color = allColor;
        }

        foreach (var item in buttons)
        {
            Color transparancy = item.color;
            transparancy.a = 1;
            item.color = transparancy;
        }

        Color sss = buttons[index].color;
        sss.a = 0;
        buttons[index].color = sss;

        Color color = buttonsPicked[index].color;
        color.a = 1;
        buttonsPicked[index].color = color;
        isChanged = true;
    }

    private void OffImage(int index)
    {
        Color buttonColor = buttons[index].color;
        buttonColor.a = 1;
        buttons[index].color = buttonColor;


        Color pickColor = buttonsPicked[index].color;
        pickColor.a = 0;
        buttonsPicked[index].color = pickColor;
        isChanged = false;
    }
    
    #endregion

 
}