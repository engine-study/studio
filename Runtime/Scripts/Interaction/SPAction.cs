using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActionState{Idle, Casting, Acting, Complete}
public enum ActionEndState{Success, Input, Canceled}
public enum ActionRestriction{None, Movement, Arms, Head}
public enum ActionType{OneShot, Looping, State, Hold}
public abstract class SPAction : ScriptableObject, IAction
{

    public ActionType Type {get{return actionType;}}

    [Header("Action Data")]
    public string actionName = "Interact";
    public ActionType actionType = ActionType.OneShot;
    public float CastDuration = .5f;
    public float ActionDuration = 0f; 

    public System.Action OnActionStart, OnActionUpdate, OnActionEnd;
    public System.Action OnActionStartCasting, OnActionUpdateCasting, OnActionEndCasting;
    public System.Action OnSweetSpotStart, OnSweetSpotEnd; 

    public SPConditionalScriptable [] conditions;
    public SPAction [] endAction;


    public virtual SPAction ActionRef() {
        return this;
    }
    public virtual bool TryAction(IActor actor, IInteract interactable) {

        for(int i = 0; i < conditions.Length; i++) {
            if(!conditions[i].IsAllowed()) {
                return false;
            }
        }

        return true;
    }

    public virtual void DoCast(bool toggle, IActor actor, IInteract interactable) {

    }

    public virtual void DoAction(bool toggle, IActor actor, IInteract interactable) {
   
        
    }

    public virtual void EndAction(IActor actor, IInteract interactable, ActionEndState reason) {
   
        // if(reason == ActionEndState.Success) {
        //     for(int i = 0; i < endAction.Length; i++) {
        //         endAction[i].ActionStart();
        //     }
        // }

        OnActionEnd?.Invoke();
    }


}

public interface IAction {
    SPAction ActionRef();
    bool TryAction(IActor actor, IInteract interactable);
    void DoCast(bool toggle, IActor actor, IInteract interactable);
    void DoAction(bool toggle, IActor actor, IInteract interactable);
    void EndAction(IActor actor, IInteract interactable, ActionEndState reason);

}