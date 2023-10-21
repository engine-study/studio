using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SPInteract : MonoBehaviour, IInteract {


    public SPAction ActionScript {get{return action;}}
    public IActor Actor {get{return actor;}}

    [Header("Interact")]
    [SerializeField] protected SPAction action;
    [SerializeField] protected SPState state;

    [Header("Debug")]
    [SerializeField] bool interacting = false;
    [SerializeField] GameObject targetGO;
    [SerializeField] SPActor actorScript;
    [SerializeField] IActor actor;

    [Header("Events")]
    public UnityEvent OnStartEvent;
    public UnityEvent OnEndEvent;

    public System.Action OnActor, OnInteract;
    public System.Action<bool, IActor> OnActorToggle, OnInteractToggle;

    protected virtual void Awake() {

        if (targetGO == null) {
            targetGO = gameObject;
        }
    }

    protected virtual void OnDestroy() {

    }

    public virtual bool IsInteractable() { return gameObject.activeInHierarchy; }

    public virtual void ToggleActor(bool toggle, IActor newActor) {

        if(toggle) {
            actor = newActor;
            actorScript = newActor as SPActor;
        } else {
            actor = null;
            actorScript = null;
        }

        OnActor?.Invoke();
        OnActorToggle?.Invoke(toggle, newActor);
    }

    public virtual void Interact(bool toggle, IActor newActor) {

        interacting = toggle;

        if (toggle) {
            
            //is this overkill?
            if(newActor != actor) {
                ToggleActor(false, actor);
                ToggleActor(true, newActor);
            }
            actor = newActor;
            newActor.SetState(state != null ? state : new SPState(PlayerState.Interact));

            OnStartEvent?.Invoke();

        } else {

            if (state != null) {
                newActor.SetState(null);
            }

            OnEndEvent?.Invoke();

        }


        OnInteract?.Invoke();
        OnInteractToggle?.Invoke(toggle, newActor);

    }


    public virtual void UpdateInteract() {

    }

    public virtual void UpdateState() {

    }

    public virtual GameObject GameObject() {
        return targetGO;
    }

    public virtual IAction Action() {
        return action;
    }

}


public interface IInteract {
    void ToggleActor(bool toggle, IActor newActor);
    void Interact(bool toggle, IActor newActor);
    void UpdateInteract();
    void UpdateState();
    bool IsInteractable() { return true; }
    GameObject GameObject();
    IAction Action();
}