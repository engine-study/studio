
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public enum SPButtonType {Text, Image}

public class SPButton : SPWindowSelectable {

    //public Button Button {get{return button;}}

    public string Text {get{return buttonText.text;}}
    public TextMeshProUGUI ButtonText {get{return buttonText;}}
    public Action OnClicked;

    [Header("Button")]
    [SerializeReference] protected Button button;
    [SerializeReference] protected TextMeshProUGUI buttonText;
    [SerializeReference] protected Image buttonImage;
    [SerializeField] protected SPOpenLink linkScript;
    [SerializeField] protected UnityEvent OnClick;
    
    public void SetupButton(string newText, bool useImage = false) {
        buttonText.text = newText;
        buttonImage.gameObject.SetActive(useImage);
    }

    public override void UpdateField(string newString, SPDataType newDataType = SPDataType.String) {
        base.UpdateField(newString, newDataType);

        SetupButton(newString, buttonImage.gameObject.activeSelf);
    }


    public void UpdateType(SPSelectableType newType, string url) {
        UpdateLink(url);
        ToggleType(newType);
    }

    public override void ToggleType(SPSelectableType newType) {
        base.ToggleType(newType);

    }

    //called from the unity event on the prefab
    public virtual void OnClickFunction() {
        

        if(selectableType == SPSelectableType.Default) {
            OnClick.Invoke();
        } else if(selectableType == SPSelectableType.Link) {
            linkScript.OpenLink();
        } else if(selectableType == SPSelectableType.Address) {
            linkScript.OpenLink();
        }

        OnClicked?.Invoke();
  
    }

    public override void GiveSpecialCursor() {

        if(selectableType == SPSelectableType.Default)
            SPCursorTexture.UpdateCursor(SPCursorState.Hand);    
        else 
            SPCursorTexture.UpdateCursor(SPCursorState.Hand);    

    }

    public void UpdateLink(string newURL) {
        linkScript.link = newURL;
    }    


}
