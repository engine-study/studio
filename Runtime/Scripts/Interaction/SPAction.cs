using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ActionState{Idle, Casting, Acting, Complete}
public enum ActionEndState{InProgress, Success, Canceled, Failed}
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
    public float Distance = 1.1f; 

    bool canPerform;
    public System.Action<ActionEndState> OnActionOver;
    public System.Action OnActionStart, OnActionUpdate, OnActionEnd;
    public System.Action OnActionStartCasting, OnActionUpdateCasting, OnActionEndCasting;
    public System.Action OnSweetSpotStart, OnSweetSpotEnd; 

    public SPConditionalScriptable [] conditions;
    public SPAction [] endAction;

    public virtual bool TryAction(IActor actor, IInteract interactable) {

        if(Vector3.Distance(actor.Owner().gameObject.transform.position, interactable.GameObject().transform.position) > Distance){
            canPerform = false;
            return canPerform;
        }

        for(int i = 0; i < conditions.Length; i++) {
            if(!conditions[i].IsAllowed()) {
                canPerform = false; 
                return canPerform;
            }
        }

        canPerform = true;
        return canPerform;
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
        OnActionOver?.Invoke(reason);
    }


}

public interface IAction {

    //this is the template of the action that we do not touch
    bool TryAction(IActor actor, IInteract interactable);
    void DoCast(bool toggle, IActor actor, IInteract interactable);
    void DoAction(bool toggle, IActor actor, IInteract interactable);
    void EndAction(IActor actor, IInteract interactable, ActionEndState reason);

}