using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Linq;
using System;

public enum SPActionType {Forward, Backward, Text, Link, Confirm, Close, Error}
public enum SPSelectableState {Default, Hovered, Clicked, Disabled, Selected, Inactive}
public enum SPDraggableState {Draggable, Static, Dragging}
public enum SPSelectableType {Default, Link, Address, Identity}
public class SPWindowSelectable : SPWindow, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler , IPointerUpHandler {


    public SPSelectableState State {get{return state;}}
    public SPDraggableState StateDraggable {get{return stateDraggable;}}
    public bool IsInteractable {get{return State != SPSelectableState.Disabled && State != SPSelectableState.Inactive;}}
    public bool IsDisabled {get{return State == SPSelectableState.Disabled;}}
    public bool IsDragging {get{return StateDraggable == SPDraggableState.Dragging;}}
    public bool IsReadOnly {get{return readOnly;}}


    public Action<SPWindowSelectable> OnWindowHover;
    public Action<SPWindowSelectable> OnWindowClick;
    public Action<bool> OnHoverUpdate;
    public Action<bool> OnClickUpdate;
    public Action<bool> OnInactiveUpdate;
    public Action<bool> OnDisabledUpdate;


    [Header("Selectable")]
    [SerializeField] protected SPSelectableState state;
    [SerializeField] protected SPDraggableState stateDraggable;
    [SerializeField] protected bool canDrag = false;
    [SerializeField] protected bool readOnly = true;
    [SerializeField] protected bool hideIfEmpty = false;
    [SerializeField] protected SPSelectableType selectableType;
    [SerializeField] protected SPActionType actionType;
    [SerializeField] protected SPWindowTheme.SPTheme selectableTheme;


    [Header("Reference")]
	[SerializeField] protected Selectable selectable;
    [SerializeReference] protected SPWindowSelectable [] _selectables;

    [SerializeReference] protected bool invert = false;
    protected List<SPWindowSelectable> children;

    [Header("State")]
    [SerializeField] protected SPDataType dataType;
    [SerializeReference] protected bool hovering = false; 
    [SerializeReference] protected bool clicking = false;

    //called in awake
    public override void Init() {
        
        if(hasInit)
            return;

        base.Init();

        //only gather children if we are in fact not, a child
        if(gameObject.GetComponentInParent<SPWindowSelectable>() == null) {

            children = GetComponentsInChildren<SPWindowSelectable>(true).ToList();
            
            //remove ourselves from the list
            children.RemoveAt(0);
            for(int i = 0; i< children.Count; i++) {children[i].Init();}
        }

        //set our type (default, link, or address - what kind of action or thing we except of this thing)
        ToggleType(selectableType);
        ToggleState(State);


    }

    public virtual void ToggleReadOnly(bool toggle) {
        readOnly = toggle;
    }

    public override void ToggleWindow(bool toggle) {
        base.ToggleWindow(toggle);
    }

    protected override void OnEnable() {
        base.OnEnable();

        if(selectable == null)
            selectable = GetComponent<Selectable>();

        ToggleState(State);
    }

    protected override void OnDisable() {
        base.OnDisable();
    }

    public override void SetTheme(SPWindowTheme newTheme) {
        base.SetTheme(newTheme);
        ToggleType(selectableType);
    }

    //is this selectable a text field, link, what kind of data? 
    public virtual void ToggleType(SPSelectableType newType) {

        selectableType = newType;

        if(newType == SPSelectableType.Default) {
            selectableTheme = Theme.defaultTheme;
        } else if(newType == SPSelectableType.Link) {
            selectableTheme = Theme.linkTheme;
        } else if(newType == SPSelectableType.Address) {
            selectableTheme = Theme.addressTheme;
        } else if(newType == SPSelectableType.Identity) {
            selectableTheme = Theme.identityTheme;
        } else {
            selectableTheme = Theme.defaultTheme;
        }

    }

    //is this interactable, disabled, inactive, etc.
    public virtual void ToggleState(SPSelectableState newState) {

        bool stateUpdate = newState != state;

        if(stateUpdate) {
            // Debug.Log("Updating to " + newState + " (from " + state + ")");
            StateToAction(newState)?.Invoke(false);
        }


        state = newState;
        selectable.interactable = IsInteractable;

        UpdateColor();
        ApplyGraphics();

        if(stateUpdate) {
            
            StateToAction(newState)?.Invoke(true);

            if(newState == SPSelectableState.Hovered) {
                OnWindowHover?.Invoke(this);
            } else if(newState == SPSelectableState.Clicked) {
                OnWindowClick?.Invoke(this);
            }
        }
    }

    Action<bool> StateToAction(SPSelectableState actionState) {
        if(state == SPSelectableState.Hovered) {return OnHoverUpdate;}
        else if(state == SPSelectableState.Clicked) {return OnClickUpdate;}
        else if(state == SPSelectableState.Disabled) {return OnDisabledUpdate;}
        else if(state == SPSelectableState.Inactive) {return OnInactiveUpdate;}
        else return null;
    }


    public virtual void ToggleCanDrag(bool toggle) {
        canDrag = toggle;
    }

    public virtual void ToggleDraggableState(SPDraggableState newState) {
        stateDraggable = newState;
    }


    public virtual void GiveDefaultCursor() {
        SPCursorTexture.UpdateCursor(SPCursorState.Default);
    }

    public virtual void GiveDisableCursor() {
        SPCursorTexture.UpdateCursor(State == SPSelectableState.Disabled ? SPCursorState.Disabled : SPCursorState.Default);
        //SPUI.UpdateCursor(SPCursorState.Disabled);
    }
    
    public virtual void GiveSpecialCursor() {
        SPCursorTexture.UpdateCursor(SPCursorState.Default);
    }

    public virtual void UpdateField(string newString) {
        UpdateField(newString, dataType);
    }

    public virtual void UpdateField(string newString, SPDataType newDataType) {
        //Debug.Log("Update field selectable: " + gameObject.name);
        if(hideIfEmpty) {
            ToggleWindow(!string.IsNullOrEmpty(newString));
        }
    }


    public override void UpdateColor() {

        if(State == SPSelectableState.Default) {
            
            //Debug.Log(selectableTheme.ToString());
            //Debug.Log(bg.ToString());

            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = Theme.defaultTheme.color; 
            borderColor = Theme.defaultTheme.color; 

        } else if(State == SPSelectableState.Hovered) {
            
            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = selectableTheme.color - Color.black * .2f;
            borderColor = selectableTheme.color - Color.black * .2f;

        } else if(State == SPSelectableState.Clicked) {
            
            //bgColor = Theme.defaultTheme.bgColor;
            //graphicsColor = selectableTheme.color - Color.black * .35f;
            //borderColor = selectableTheme.color - Color.black * .35f;
            
            bgColor = Theme.defaultTheme.color;
            graphicsColor = selectableTheme.bgColor;
            borderColor = selectableTheme.bgColor;


        } else if(State == SPSelectableState.Disabled) {
            
            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = Theme.defaultTheme.color - Color.black * .5f;
            borderColor = Theme.defaultTheme.color;

        } else if(State == SPSelectableState.Selected) {
            
            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = Theme.defaultTheme.color;
            borderColor = Theme.defaultTheme.color;

        } else if(State == SPSelectableState.Inactive) {
            
            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = Theme.defaultTheme.color;
            borderColor = Theme.defaultTheme.color;

        } else {
            
            bgColor = Theme.defaultTheme.bgColor;
            graphicsColor = Theme.defaultTheme.color; 
            borderColor = Theme.defaultTheme.color; 

        }
    
        ApplyGraphics();
    }

    public override void ApplyGraphics() {
        base.ApplyGraphics();

        /*
        if(children!=null) for(int i = 0; i < children.Count; i++) {
            children[i].ToggleState(state);
        }
        */

    }
    


    public virtual void OnPointerEnter(PointerEventData eventData) {

        hovering = true; 

        if(clicking)
            return;

        if(SPUIBase.IsDragging) {
            return;
        }

        if (selectable.IsInteractable()) {
            SPUIBase.PlayHover();
            ToggleState(SPSelectableState.Hovered);
            GiveSpecialCursor();
        } else if(State == SPSelectableState.Disabled) {
            ToggleState(SPSelectableState.Disabled);
            GiveDisableCursor();
        } else {

        }

    }

    
    public virtual void OnPointerExit(PointerEventData eventData) {

        hovering = false;

        /*
        if(clicking)
            return;
        */

        if(SPUIBase.IsDragging) {
            return;
        }
        
        if (selectable.IsInteractable()) {
            ToggleState(SPSelectableState.Default);
            
            //force selectables to deselect (I hate this behaviour this is why)
            //selectable.OnDeselect(null);
        }

        GiveDefaultCursor();

    }


    public virtual void OnPointerDown(PointerEventData eventData) {

        clicking = true;
        hovering = true; 

    }

    public virtual void OnPointerUp(PointerEventData eventData) {

        if (clicking && hovering && !eventData.dragging) {

            if(selectable.IsInteractable()) {
                ToggleState(SPSelectableState.Clicked);
                SPUIBase.PlayUISound(actionType);
            } else if(state == SPSelectableState.Disabled) {
                SPUIBase.PlayError();
            } else {
                SPUIBase.PlayUISound(actionType);
            }
          
        } else {

        }

        clicking = false;

        if (hovering && selectable.IsInteractable()) {
            ToggleState(SPSelectableState.Hovered);
            selectable.OnDeselect(null);
            EventSystem.current.SetSelectedGameObject(null);
        }


    }
   
}
