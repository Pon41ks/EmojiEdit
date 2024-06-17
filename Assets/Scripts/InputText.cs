using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InputText : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputText;
    [SerializeField] private TextMeshProUGUI uiText;

    private void Update()
    {
        uiText.text = inputText.text;
    }
}
