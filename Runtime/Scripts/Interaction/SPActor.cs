using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { None, Input, Ambient }
public class SPActor : MonoBehaviour, IActor {

    public ActionState ActionState { get { return internalState; } }
    public IAction ActionInterface {get{return action;}}
    public SPAction ActionScript { get { return actionScript; } }
    public IInteract Interact { get { return interact; } }
    public SPReciever Reciever { get { return reciever; } }
    public GameObject Target { get { return target; } }
    public List<GameObject> GameObjects {get{return gos;}}

    [Header("Action")]
    public SPBase sender;

    [Header("Debug")]
    [SerializeField] private SPState State;
    [SerializeField] protected IAction action;
    [SerializeField] protected SPReciever reciever;

    [SerializeField] protected SPAction actionScript;
    [SerializeField] protected IInteract interact;
    [SerializeField] protected IInteract activeInteract;
    [SerializeField] protected GameObject target;
    [SerializeField] protected List<GameObject> gos;

    [Header("Mutable Data")]
    [SerializeField] public ActionState internalState;
    [SerializeField] protected float castCount = 0f;
    [SerializeField] protected float actionCount = 0f;
    [SerializeField] protected float actionEndTime = 0f;
    [SerializeField] public float CastLerp = 0f;
    [SerializeField] public float ActionLerp = 0f;

    MonoBehaviour owner;

    public System.Action<bool, IInteract> OnTargetsUpdated;
    public System.Action<bool, IInteract> OnActionsUpdated;
    public System.Action<IAction> OnAction;
    public System.Action<ActionEndState> OnActionEnd;
    public System.Action OnActorUpdate;


    public void ToggleReciever(bool toggle, SPReciever r) {
        if (r == null) {
            return;
        }

        if (toggle) {
            reciever = r;
            reciever.OnInteractToggle += LoadActionFromReciever;
            reciever.OnTargetToggle += LoadTargetFromReciever;
        } else if(reciever != null) {
            reciever.OnInteractToggle -= LoadActionFromReciever;
            reciever.OnTargetToggle -= LoadTargetFromReciever;
            reciever = null;
        }
    }

    public void Init() {

        owner = sender != null ? sender : this;

        if (reciever == null) {
            ToggleReciever(true, gameObject.GetComponent<SPReciever>());
        }

        SetState(new SPState(PlayerState.Idle));

    }

    void OnDestroy() {
        ToggleReciever(false, reciever);
    }

    public void ToggleActor(bool toggle) {
        enabled = toggle;
    }

    public virtual MonoBehaviour Owner() {
        return owner;
    }


    bool hasUp = false;
    void Update() {

        UpdateActionsAvailable();
        UpdateInput();
        UpdateAction();
        UpdateState();

    }

    bool hasLift;

    void UpdateActionsAvailable() {
        for(int i = 0; i < reciever.Interactables.Count; i++) {
            reciever.Interactables[i].Action().TryAction(this, reciever.Interactables[i]);
        }
    }
    
    protected virtual void UpdateInput() {

    }

    public void InputClick(int mouseButton, IInteract i) {

        bool inputDown = SPUIBase.CanInput && Input.GetMouseButtonDown(mouseButton);
        bool input = SPUIBase.CanInput && Input.GetMouseButton(mouseButton);
        InputAction(inputDown,input,i);
    }

    public void InputKey(KeyCode keyCode, IInteract i) {

        bool inputDown = SPUIBase.CanInput && Input.GetKeyDown(keyCode);
        bool input = SPUIBase.CanInput && Input.GetKey(keyCode);
        InputAction(inputDown,input,i);
    }

    public void InputAction(bool inputDown, bool input, IInteract i) {

        bool canDoInput = (inputDown && ActionState == ActionState.Idle) || (input && (ActionState == ActionState.Casting || ActionState == ActionState.Acting));
        bool canDoAction = i.Action().TryAction(this, i);

        if (canDoInput && canDoAction) { Use(i.Action(), i); } 
        else if (Interact == i) { Stop(i.Action(), i, ActionEndState.Canceled);}
        
    }

    void UpdateAction() {
        OnActorUpdate?.Invoke();
    }

    void UpdateState() {

        if (activeInteract != null) {
            activeInteract.UpdateState();
        }

        State.UpdateState(this);
    }


    //get the most updated target from the interactreciever
    void LoadTargetFromReciever(bool toggle, IInteract newInteractable) {
        OnTargetsUpdated?.Invoke(toggle, newInteractable);
    }

    //get the list of potential actions from the reciever
    void LoadActionFromReciever(bool toggle, IInteract newInteract) {
        ToggleAction(toggle, newInteract);
    }

    public void ToggleAction(bool toggle, IInteract newInteract) {
        Debug.Assert(newInteract.Action() != null, name + " no action on " + newInteract.GameObject().name);
        IAction newAction = newInteract.Action();
        GameObject go = newInteract.GameObject();

        if (toggle) {

            gos.Add(go);
            //tell the interactable we are interactable
            newInteract.ToggleActor(true, this);

        } else {

            gos.Remove(go);

            //stop the action if it was active
            if (go == Target) {
                Stop(newAction, newInteract, ActionEndState.Failed);

                //tell the interactable we are not interactable
                newInteract.ToggleActor(false, this);

            }
        }

        OnActionsUpdated?.Invoke(toggle, newInteract);
    }

    public virtual void SetToInitialState() {

        internalState = ActionState.Idle;

        castCount = 0f;
        actionCount = 0f;

        CastLerp = 0f;
        ActionLerp = 0f;

    }

    public void Use() {

    }

    public void Use(IAction newAction, IInteract newInteractable) {

        //load new action if we haven't loaded it yet
        if (Target != newInteractable.GameObject() || actionScript != newInteractable.Action() as SPAction) {

            //stop current action
            if (action != null) {
                Stop(action, newInteractable, ActionEndState.Failed);
            }

            //setup new action
            // int index = reciever.GameObjects.IndexOf(newInteractable.GameObject());
            // interact = reciever.Interactables[index];
            // target = reciever.GameObjects[index];

            action = newInteractable.Action();
            actionScript = newInteractable.Action() as SPAction;
            interact = newInteractable;
            target = newInteractable.GameObject();

        }

        UpdateActionLogic();

        OnAction?.Invoke(action);

    }

    public virtual void SetState(IState newState) {

        if (State != null) {
            State.ExitState(this);
        }

        State = newState as SPState;

        if (State == null) {
            State = new SPState(PlayerState.Idle);
        }

        State.EnterState(this);
    }


    public void Stop(IAction a, IInteract i, ActionEndState reason) {

        bool shouldEnd = reason != ActionEndState.Canceled || (reason == ActionEndState.Canceled && (ActionScript.Type == ActionType.Hold || ActionScript.Type == ActionType.Looping));

        if (internalState == ActionState.Casting) {

            CastingEnd(reason);

        } else if (shouldEnd) {

            if (internalState == ActionState.Acting) {
                ActionEnd(reason);
                Interact.Interact(false, this);
            }

            if (reason == ActionEndState.Success) {
                actionEndTime = Time.time;
            }


            action = null;
            actionScript = null;
            interact = null;
            target = null;

            activeInteract = null;

        }

        OnActionEnd?.Invoke(reason);

        SetToInitialState();

    }


    protected virtual void UpdateActionLogic() {

        if (castCount < ActionScript.CastDuration) {

            if (castCount == 0f) {
                CastingStart();
            } else {
                CastingUpdate();
            }

        } else {

            if (ActionState == ActionState.Casting) {
                ActionStart();
            } else if (ActionState == ActionState.Acting) {
                ActionUpdate();
            }
        }
    }

    protected virtual void CastingStart() {

        // Debug.Log(ActionScript.name + " Casting Start", Interact.GameObject());
        internalState = ActionState.Casting;
        action.DoCast(true, this, Interact);
        ActionScript.OnActionStartCasting?.Invoke();

        CastingUpdate();
    }

    protected virtual void CastingUpdate() {

        castCount += Time.deltaTime;
        CastLerp = Mathf.Clamp01(castCount / ActionScript.CastDuration);
        ActionScript.OnActionUpdateCasting?.Invoke();
        if (CastLerp == 1f) {
            CastingEnd(ActionEndState.Success);
        }
    }

    protected virtual void CastingEnd(ActionEndState endState) {

        // Debug.Log(ActionScript.name + " Casting End", Interact.GameObject());

        if (endState == ActionEndState.Success) {

        } else {
            action.DoCast(false, this, Interact);
            internalState = ActionState.Idle;
        }


        ActionScript.OnActionEndCasting?.Invoke();
    }

    protected virtual void ActionStart() {

        // Debug.Log(ActionScript.name + " Action Start", Interact.GameObject());
        internalState = ActionState.Acting;

        action.DoAction(true, this, Interact);
        Interact.Interact(true, this);

        activeInteract = Interact;

        ActionScript.OnActionStart?.Invoke();

        if (ActionScript.Type == ActionType.OneShot || ActionScript.Type == ActionType.State) {
            Stop(ActionScript, Interact, ActionEndState.Success);
        } else {
            internalState = ActionState.Acting;
            ActionUpdate();
        }


    }

    protected virtual void ActionUpdate() {

        // Debug.Log(Interact.GameObject().name + " Action Update", Interact.GameObject());

        actionCount += Time.deltaTime;
        ActionLerp = Mathf.Clamp01(actionCount / ActionScript.ActionDuration);

        //update the interactable
        Interact.UpdateInteract();

        if (ActionLerp == 1f) {

            if (ActionScript.Type == ActionType.Hold) {
                ActionEnd(ActionEndState.Success);
            } else {

            }
        }

        ActionScript.OnActionUpdate?.Invoke();
    }


    protected virtual void ActionEnd(ActionEndState reason) {

        // Debug.Log(ActionScript.name + " Action End", Interact.GameObject());

        internalState = ActionState.Idle;

        action.DoAction(false, this, Interact);
        ActionScript.EndAction(this, Interact, reason);

    }


}


public interface IActor {

    MonoBehaviour Owner();
    void Use(IAction newAction, IInteract newInteractable);
    void Stop(IAction newAction, IInteract newInteract, ActionEndState reason);
    void SetState(IState newState);

}

