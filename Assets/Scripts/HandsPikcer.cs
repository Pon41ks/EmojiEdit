using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HandsPikcer : MonoBehaviour
{
   [SerializeField] private GameObject firstHand;
   [SerializeField] private GameObject secondHand;
   [SerializeField] private SpriteRenderer handsSpriteFirst;
   [SerializeField] private SpriteRenderer handsSpriteSecond;
   [SerializeField] private Image[] pickedObj;
   [SerializeField] private Sprite newSprite;

   private bool isFirstHand = true;
   
   

   private void Start()
   {
      handsSpriteFirst.GetComponent<SpriteRenderer>();
      handsSpriteSecond.GetComponent<SpriteRenderer>();
   }

   public void ChangeSprite(int index)
   {
      ShowBackGroundImage(index);
      if (!Manager.isSecondHand)
      {
       firstHand.SetActive(true);
       handsSpriteFirst.sprite = newSprite;
       isFirstHand = false;
       Manager.SendHandChanged();
      }
      else if (Manager.isSecondHand)
      {
         secondHand.SetActive(true);
         handsSpriteSecond.sprite = newSprite;
         isFirstHand = true;
         Manager.SendHandChanged();
      }
   }

   private void ShowBackGroundImage(int index)
   {
      if (!Manager.isSecondHand)
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
      else if (Manager.isSecondHand)
      {
         Color color = pickedObj[index].color;
         color.a = 1;
         pickedObj[index].color = color;
      }
   }

   public void OffBackGroundOfImage()
   {
      firstHand.SetActive(false);
      secondHand.SetActive(false);
   }

   public void ClearElemnts()
   {
      foreach (var item in pickedObj)
      {
         Color allColor = item.color;
         allColor.a = 0;
         item.color = allColor;
      }
      firstHand.SetActive(false);
      secondHand.SetActive(false);
   }
}
