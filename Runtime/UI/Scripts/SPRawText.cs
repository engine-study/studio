using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SPRawText : SPWindowSelectable
{

    public string Text {get{return text.text;}}

    [Header("Text")]
    [SerializeField] protected TMP_InputField text;
    [SerializeField] protected int maxCharacters = 0;

    public override void Init() {
        if(hasInit) {
            return;
        }

        base.Init();

    }

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<TMP_SelectionCaret>().raycastTarget = false;

    }

    public override void UpdateField(string newText)
    {
        base.UpdateField(newText);

        /*
        if(maxCharacters > 0 && !string.IsNullOrEmpty(newText) && newText.Length > maxCharacters) {
            //Debug.Log("newText length: " + newText.Length);
            //Debug.Log("max characters: " + maxCharacters);
            //newText = newText.Substring(newText.Length-1,maxCharacters - 2);
            text.text = newText.Substring(newText.Length - maxCharacters);
        } else {
            text.text = newText;
        }
        */

        text.text = newText;
    }


    public override void ToggleState(SPSelectableState newState) {
        base.ToggleState(newState);

        text.readOnly = readOnly;
        text.selectionColor = readOnly || State == SPSelectableState.Disabled ? Theme.readOnlyHighlight : Theme.highlightColor;
    }

    public void AddText(string newText) {
        UpdateField(text.text + newText);
    }

    public override void GiveSpecialCursor() {
        SPCursorTexture.UpdateCursor(SPCursorState.TextSelect);
    }
}
