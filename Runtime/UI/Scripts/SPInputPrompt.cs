using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPInputPrompt : SPWindow
{
    [SerializeField] TMPro.TextMeshProUGUI inputText;
    public void SetKey(KeyCode newKey) {
        SetKey(newKey.ToString());
    }
    public void SetKey(string input) {
        inputText.text = input;
    }


  
}
