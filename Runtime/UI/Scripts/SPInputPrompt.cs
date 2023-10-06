using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPInputPrompt : SPWindow
{
    public KeyCode Key {get{return key;}}
    [Header("Input")]
    [SerializeField] TMPro.TextMeshProUGUI inputText;
    [Header("Debug")]
    [SerializeField] KeyCode key;

    public void SetKey(KeyCode newKey) {
        key = newKey;
        SetKey(SPInput.GetKeyCodeFormatted(newKey));
    }
    public void SetKey(string input) {
        inputText.text = input;
    }

    void Update() {
        transform.localScale = Input.GetKey(key) ? Vector3.one * .9f : Vector3.one;
    }

  
}
