using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;

public class SPInputField : SPWindowSelectable {
    public string Text {get{return inputField.text;}}
    public TMP_InputField Input {get{return inputField;}}
    
    [Header("Input")]
    [SerializeField] protected bool useButton = false;
    [SerializeField] protected SPButton button;
    [SerializeField] protected TMP_InputField inputField;
    [SerializeField] protected string placeholderString = "";
    private string text;
    TMP_InputField.SubmitEvent EditEvent;
    [SerializeField] public UnityEvent OnEdit, OnSubmit;
    public Action OnSubmitted;

    protected override void Start()
    {
        base.Start();

        GetComponentInChildren<TMP_SelectionCaret>().raycastTarget = false;

    }

    public override void Init() {

        if(hasInit)
            return;

        //inputField.onEndEdit.AddListener(delegate {UpdateField(inputField.text); });

        inputField.pointSize = Theme.textSize;
        text = inputField.text;
        base.Init();
    }

    protected override void Destroy() {
        base.Destroy();
        //inputField.onEndEdit.RemoveListener(delegate {UpdateField(inputField.text); });
    }

    public void OnEndEdit() {

        if(inputField.text == text) 
            return;

        UpdateField(inputField.text, dataType);

        OnEdit.Invoke();

        if(UnityEngine.Input.GetButtonDown("Submit")) {
            Debug.Log("Textfield " + gameObject.name + " submitted");
            OnSubmit.Invoke();
            OnSubmitted?.Invoke();
        }
    }

    public override void GiveSpecialCursor() {
        if(inputField.readOnly) {
            SPCursorTexture.UpdateCursor(SPCursorState.TextSelect);    
        } else {
            SPCursorTexture.UpdateCursor(SPCursorState.Text);    
        }
    }

    public override void ToggleState(SPSelectableState newState) {
        base.ToggleState(newState);

        inputField.readOnly = readOnly;
        inputField.selectionColor = readOnly || State == SPSelectableState.Disabled ? Theme.readOnlyHighlight : Theme.highlightColor;
    }

    public override void UpdateField(string newString, SPDataType newDataType = SPDataType.String) {
        base.UpdateField(newString, newDataType);

        //Debug.Log("Update field inputfield " + gameObject.name);

        inputField.text = newString;
        text = newString;
        
        UpdateInputType(newDataType);

    }

    public void UpdateInputType(SPDataType newDataType) {

        dataType = newDataType;

        button.ToggleWindow(useButton || (dataType == SPDataType.PageURL || dataType == SPDataType.ImageURL || dataType == SPDataType.Address));
        inputField.lineType = dataType == SPDataType.MultiString ? TMP_InputField.LineType.MultiLineSubmit : TMP_InputField.LineType.SingleLine;
        //inputField.textComponent.overflowMode = dataType == SPDataType.MultiString ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;

        inputField.textComponent.overflowMode = dataType == SPDataType.MultiString || dataType == SPDataType.InputString ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
        inputField.textComponent.enableWordWrapping = dataType == SPDataType.MultiString ? true : false;


        if(dataType == SPDataType.Name) {

            inputField.contentType = TMP_InputField.ContentType.Standard;
            ToggleType(SPSelectableType.Default);

        } else if(dataType == SPDataType.String) {

            inputField.contentType = TMP_InputField.ContentType.Standard;

            ToggleType(SPSelectableType.Default);

        } else if(dataType == SPDataType.MultiString) {

            inputField.contentType = TMP_InputField.ContentType.Standard;

            ToggleType(SPSelectableType.Default);

        } else if(dataType == SPDataType.Float) {

            inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
            ToggleType(SPSelectableType.Default);

        } else if(dataType == SPDataType.Int) {

            inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
            ToggleType(SPSelectableType.Default);

        } else if(dataType == SPDataType.Address) {

            inputField.contentType = TMP_InputField.ContentType.Alphanumeric;

            button.ToggleWindow(SPHelper.IsValidAddress(Text));
            //linkButton.ToggleInteractable(SPHelper.IsValidAddress(Text));
            button.UpdateType(SPSelectableType.Address, "https://etherscan.io/address/" + Text);

            ToggleType(SPSelectableType.Address);

        } else if(dataType == SPDataType.PageURL) {

            inputField.contentType = TMP_InputField.ContentType.Custom;
            
            button.ToggleWindow(SPHelper.IsValidLink(Text));
            //linkButton.ToggleInteractable(SPHelper.IsValidLink(Text));
            button.UpdateType(SPSelectableType.Link, Text);

            ToggleType(SPSelectableType.Link);

        } else if(dataType == SPDataType.ImageURL) {

            inputField.contentType = TMP_InputField.ContentType.Custom;

            button.ToggleWindow(SPHelper.IsValidLink(Text));
            //linkButton.ToggleInteractable(SPHelper.IsValidLink(Text));
            button.UpdateType(SPSelectableType.Link, Text);

            ToggleType(SPSelectableType.Link);

        }

        //refresh the state of the selectable to reflect our new selectabletype if it changed
        ToggleState(State);

    }

    public void UpdateTyping() {
        SPUIBase.PlayTyping();
    }

}
