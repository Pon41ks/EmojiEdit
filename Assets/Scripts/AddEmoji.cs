using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddEmoji : MonoBehaviour
{
    [SerializeField] private SpriteRenderer addedEmoji;
    [SerializeField] private Sprite newSprite;
    [SerializeField] private GameObject emojiPanel;
    [SerializeField] private GameObject emoji;
    
    
    public static bool isEmojiPanelOpened { get; private set; }
 
    #region OpenEmoji

    public void OpenEmodjiPanel()
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
    
    public void ChangeEmoji()
    {
        if (!emoji.activeInHierarchy)
        {
            emoji.SetActive(true);
        }
        addedEmoji.sprite = newSprite;
    }

    #endregion
}
