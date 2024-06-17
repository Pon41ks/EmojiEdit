using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class Manager
{
    public static bool isSecondHand { get; private set; }


   
    public static void SendHandChanged()
    {
        if (isSecondHand)
        {
            isSecondHand = false;
            
        }
        else
        {
            isSecondHand = true;
        }
    }

}
