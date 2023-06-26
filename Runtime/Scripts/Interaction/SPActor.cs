using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType {None, Input, Ambient}
public class SPActor : MonoBehaviour, IActor
{

    public ActionState ActionState {get{return internalState;}}

    public IAction Action;
    public SPAction ActionRef {get{return actionRef;}}
    public IInteract Interact {get{return interact;}}
    public GameObject Target {get{return target;}}
    private static KeyCode [] KEYS = new KeyCode[1] {KeyCode.E};

    [Header("Action")]
    public SPBase sender;
    public SPInteractReciever reciever;

    [Header("Debug")]
    public SPState State;
    [SerializeField] protected List<IInteract> interactables; 
    [SerializeField] protected List<GameObject> gameObjects; 

    [SerializeField] protected SPAction actionRef;
    [SerializeField] protected IInteract interact; 
    [SerializeField] protected IInteract activeInteract; 
    [SerializeField] protected GameObject target; 


    [Header("Mutable Data")]
    [SerializeField] public ActionState internalState;
    [SerializeField] protected float castCount = 0f; 
    [SerializeField] protected float actionCount = 0f; 
    [SerializeField] protected float actionEndTime = 0f; 
    [SerializeField] public float CastLerp = 0f; 
    [SerializeField] public float ActionLerp = 0f; 



    public System.Action<bool, IInteract> OnTarget;
    public System.Action<bool, IInteract> OnActionToggle;
    public System.Action<IAction> OnAction;
    public System.Action OnActorUpdate;
    


    void Start() {

        interactables = new List<IInteract>();
        gameObjects = new List<GameObject>();

        if(reciever == null) {
            reciever = gameObject.GetComponent<SPInteractReciever>();
        }

        reciever.OnTargetToggle += ToggleTarget;
        reciever.OnInteractToggle += ToggleAction;

        SetState(new SPState(PlayerState.Idle));

    }

    void OnDestroy() {
        reciever.OnTargetToggle -= ToggleTarget;
        reciever.OnInteractToggle -= ToggleAction;
    }

    public virtual MonoBehaviour Player() {
        return sender; 
    }


    bool hasUp = false;
    void Update() {

        UpdateInput();
        UpdateAction();
        UpdateState();
        
    }

    bool hasLift;
    void UpdateInput() {

        for(int i = 0; i < KEYS.Length; i++) {

            if(i > interactables.Count - 1) {break;}

            bool input = SPUIBase.CanInput && (Input.GetKeyDown(KEYS[i]) || (Input.GetKey(KEYS[i])) && ActionRef == interactables[i].Action().ActionRef());

            if(input || ActionState == ActionState.Acting) {
                Use(interactables[i].Action(), interactables[i]);
                break;
            } else if(ActionRef) {
                Stop(interactables[i].Action(), interactables[i], ActionEndState.Input);   
            } 
        }

    }

    void UpdateAction() {
        OnActorUpdate?.Invoke();
    }

    void UpdateState() {

        if(activeInteract != null) {
            activeInteract.UpdateState();
        }

        State.UpdateState(this);
    }
    
    
    //get the most updated target from the interactreciever
    void ToggleTarget(bool toggle, IInteract newInteractable) {
        int index = gameObjects.IndexOf(newInteractable.GameObject());
        if(index < 0) { return;}
        OnTarget?.Invoke(toggle, interactables[index]);
    }

    //get the list of potential actions from the reciever
    void ToggleAction(bool toggle, IInteract newInteractable) {

        IAction newAction = null;

        Debug.Assert(newInteractable.Action() != null, name + " no action on " + newInteractable.GameObject().name);

        if(toggle) {

            newAction = newInteractable.Action();

            interactables.Add(newInteractable);
            gameObjects.Add(newInteractable.GameObject());

            //tell the interactable we are interactable
            newInteractable.ToggleActor(true, this);
            OnActionToggle?.Invoke(true, newInteractable);


        } else {

            int indexOfAction = gameObjects.IndexOf(newInteractable.GameObject());
            
            if(indexOfAction < 0) {
                Debug.LogError("No action found");
            } else {
                
                newAction = interactables[indexOfAction].Action();

                //stop the action if it was active
                if(newInteractable == Interact) {
                    Stop(newAction, newInteractable, ActionEndState.Canceled);
                }

                interactables.Remove(newInteractable);
                gameObjects.Remove(newInteractable.GameObject());

                //tell the interactable we are not interactable
                newInteractable.ToggleActor(false, this);
                OnActionToggle?.Invoke(false, newInteractable);

            }
        }
    }

    public virtual void SetToInitialState() {
        
        castCount = 0f;
        actionCount = 0f; 

        CastLerp = 0f;
        ActionLerp = 0f;

    }


    public void Use(IAction newAction, IInteract newInteractable) {
        
        //load new action if we haven't loaded it yet
        if(target != newInteractable.GameObject() || actionRef != newInteractable.Action().ActionRef()) {

            //stop current action
            if(Action != null) {
                Stop(Action, newInteractable, ActionEndState.Canceled);
            }

            //setup new action
            int index = gameObjects.IndexOf(newInteractable.GameObject());

            Action = newInteractable.Action();
            actionRef = newInteractable.Action().ActionRef();
            interact = interactables[index];
            target = gameObjects[index];

        }

        UpdateActionLogic();

        OnAction?.Invoke(Action);  

    }

    public virtual void SetState(IState newState) {

        if(State != null) {
            State.ExitState(this);
        }

        State = newState as SPState;

        if(State == null) {
            State = new SPState(PlayerState.Idle);
        } 

        State.EnterState(this);
    }


    public void Stop(IAction newAction, IInteract newInteract, ActionEndState reason) {

        bool shouldEnd = reason != ActionEndState.Input || (reason == ActionEndState.Input && (ActionRef.Type == ActionType.Hold || ActionRef.Type == ActionType.Looping));

        if(internalState == ActionState.Casting) {

            CastingEnd(reason);

        } else if(shouldEnd) {

            if(internalState == ActionState.Acting) {
                ActionEnd(reason);
                Interact.Interact(false, this);
            }

            if(reason == ActionEndState.Success) {
                actionEndTime = Time.time;
            }
            

            Action = null; 
            actionRef = null;
            interact = null;
            target = null;

            activeInteract = null;

        }

        SetToInitialState();

    }


    protected virtual void UpdateActionLogic() {

        if(castCount < ActionRef.CastDuration) {

            if(castCount == 0f) {
                CastingStart();
            } else {
                CastingUpdate();
            }

        } else {

            if(ActionState == ActionState.Casting) {
                ActionStart();
            } else if(ActionState == ActionState.Acting) {
                ActionUpdate();
            } 
        }
    }

    protected virtual void CastingStart() {

        Debug.Log(ActionRef.name + " Casting Start",Interact.GameObject());
        internalState = ActionState.Casting;
        Action.DoCast(true, this, Interact);   
        ActionRef.OnActionStartCasting?.Invoke();
        
        CastingUpdate();
    }

    protected virtual void CastingUpdate() {

        castCount += Time.deltaTime;
        CastLerp = Mathf.Clamp01(castCount / ActionRef.CastDuration);
        ActionRef.OnActionUpdateCasting?.Invoke();
        if(CastLerp == 1f) {
            CastingEnd(ActionEndState.Success);
        }
    }

    protected virtual void CastingEnd(ActionEndState endState) {

        Debug.Log(ActionRef.name + " Casting End",Interact.GameObject());

        if(endState == ActionEndState.Success) {

        } else {
            Action.DoCast(false, this, Interact);   
            internalState = ActionState.Idle;
        }


        ActionRef.OnActionEndCasting?.Invoke();
    }

    protected virtual void ActionStart() {

        Debug.Log(ActionRef.name + " Action Start",Interact.GameObject());
        internalState = ActionState.Acting;

        Action.DoAction(true, this, Interact);   
        Interact.Interact(true, this);

        activeInteract = Interact;

        ActionRef.OnActionStart?.Invoke();

        if(ActionRef.Type == ActionType.OneShot || ActionRef.Type == ActionType.State) {
            SetToInitialState();
        } else if(ActionRef.ActionDuration > 0f || ActionRef.Type == ActionType.Hold) {

        } else {
            internalState = ActionState.Acting;
        }

        ActionUpdate();

    }

    protected virtual void ActionUpdate() {

        Debug.Log(Interact.GameObject().name + " Action Update",Interact.GameObject());

        actionCount += Time.deltaTime;
        ActionLerp = Mathf.Clamp01(actionCount/ActionRef.ActionDuration);

        //update the interactable
        Interact.UpdateInteract();

        if(ActionLerp == 1f) {

            if(ActionRef.Type == ActionType.Hold) {
                ActionEnd(ActionEndState.Success);
            } else {   

            }
        }

        ActionRef.OnActionUpdate?.Invoke();   
    }


    protected virtual void ActionEnd(ActionEndState reason) {

        Debug.Log(ActionRef.name  + " Action End",Interact.GameObject());

        internalState = ActionState.Idle;
        
        Action.DoAction(false, this, Interact);   
        ActionRef.EndAction(this,Interact,reason);

    }




    void OnDrawGizmosSelected() {
        for(int i = 0; i < gameObjects.Count; i++) {
            if(Target != null && gameObjects[i] == Target) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(gameObjects[i].transform.position, transform.position);
            } else {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(gameObjects[i].transform.position, transform.position);
            }
        }
    }
}


public interface IActor {

    MonoBehaviour Player();
    void Use(IAction newAction, IInteract newInteractable);
    void Stop(IAction newAction, IInteract newInteract, ActionEndState reason);
    void SetState(IState newState);

}

