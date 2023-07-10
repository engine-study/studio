using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPInteract : MonoBehaviour, IInteract
{


    [Header("Interact")]
    public SPAction action;
    public SPState state;

    [Header("Debug")]
    public bool interacting = false; 
    public SPPlayer actorPlayer;
    public IActor actor;

    [HideInInspector] public GameObject go;

    public System.Action OnActor, OnInteract;
    public System.Action<bool, IActor> OnActorToggle, OnInteractToggle;

    protected virtual void Awake() {
        if(go == null) {
            go = gameObject;
        }
    }

    protected virtual void OnDestroy() {

    }

    public virtual bool IsInteractable() {return true;}

    public virtual void ToggleActor(bool toggle, IActor newActor) {

        OnActor?.Invoke();
        OnActorToggle?.Invoke(toggle, newActor);
    }

    public virtual void Interact(bool toggle, IActor newActor) {

        interacting = toggle; 
        
        if(toggle) {
            actor = newActor;
            actorPlayer = newActor.Player() as SPPlayer;

            newActor.SetState(state != null ? state : new SPState(PlayerState.Interact));

        } else {
            actorPlayer = null;

            if(state != null) {
                newActor.SetState(null);
            }
        }

        OnInteract?.Invoke();
        OnInteractToggle?.Invoke(toggle,newActor);

    }


    public virtual void UpdateInteract() {

    }

    public virtual void UpdateState() {

    }

    public virtual GameObject GameObject() {
        return go;
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
    bool IsInteractable() {return true;}
    GameObject GameObject();
    IAction Action();
}