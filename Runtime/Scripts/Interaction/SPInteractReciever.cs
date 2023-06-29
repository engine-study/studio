using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SPInteractReciever : MonoBehaviour
{
    public IInteract Target {get{return target;}}
    public SPBase TargetBase {get{return targetBase;}}
    public bool HasInteractable {get{return hasInteractable;}}

    [Header("Interact")]
    protected IInteract target;
    protected SPBase targetBase;
    bool hasInteractable = false;

    [SerializeField] protected List<IInteract> interactables;
    [SerializeField] protected List<GameObject> gameobjects;

    public Action OnInteractUpdate;
    public Action<bool, IInteract> OnInteractToggle;
    public Action<bool, IInteract> OnTargetToggle;
    public Action<IInteract> OnInteractAdded;
    public Action<IInteract> OnInteractRemoved;

    void Awake() {

        interactables = new List<IInteract>();
        gameobjects = new List<GameObject>();

    }

    /*

    public virtual void PlayerToggle(bool toggle, SPPlayer newPlayer) {

        bool contains = Players.Contains(newPlayer);
        if(toggle && !contains) {
            Players.Add(newPlayer);
        } else if(!toggle && contains) {
            Players.Remove(newPlayer);
        }

        OnPlayerToggle?.Invoke();

    }

    */



    void OnEnable() {

    }

    void OnDisable() {
        
        if(interactables == null) {
            return;
        }

        for(int i = interactables.Count-1; i > -1; i--) {
            ToggleInteractable(false, interactables[i]);
        }
    }
    


    protected virtual void OnTriggerEnter(Collider other) {

        IInteract i = other.GetComponentInParent<IInteract>();
        //Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        if(i != null && i.IsInteractable() && i.GameObject() != gameObject) {
            ToggleInteractable(true, i);
        }

    }

    protected virtual void OnTriggerExit(Collider other) {

        IInteract i = other.GetComponentInParent<IInteract>();
        //Debug.Log("OnTriggerExit: " + other.gameObject.name);
        if(i != null && i.GameObject() != gameObject) {
            ToggleInteractable(false, i);
        }

    }
     
    void ToggleList(bool toggle, IInteract newInteract, int index) {
        if(toggle) {
            interactables.Add(newInteract);
            gameobjects.Add(newInteract.GameObject());
        } else {
            interactables.RemoveAt(index);
            gameobjects.RemoveAt(index);
        }
    }

    void ToggleInteractable(bool toggle, IInteract newInteract) {

        //check if any null interactables have creapt into our list
        for(int i = gameobjects.Count - 1; i > -1; i--) {
            if(gameobjects[i] == null) {
                ToggleList(false, null, i);
                continue;
            }
        }

        //we shouldn't be toggling if we already have the object or have already disposed of it
        bool contains = interactables.Contains(newInteract);
        if((toggle && contains) || (!toggle && !contains)) {
            return;
        }

        if(toggle) {
            // Debug.Log("Add Interactable: " + newInteract.GameObject().name);
            ToggleList(true, newInteract, -1);
            OnInteractAdded?.Invoke(newInteract);

        } else {
            // Debug.Log("Remove Interactable: " + newInteract.GameObject().name);
            ToggleList(false, newInteract, gameobjects.IndexOf(newInteract.GameObject()));
            OnInteractRemoved?.Invoke(newInteract);

        }


        //creates action that lets the UI know that an interactble object has either left the player or entered the players range
        OnInteractToggle?.Invoke(toggle,newInteract);
        
        float distance = 9999f;
        int index = -1;
        for(int i = gameobjects.Count - 1; i > -1; i--) {

            if(gameobjects[i] == null) {
                ToggleList(false, null, i);
                Debug.LogError("This should never happen");
                continue;
            }

            float newDistance = Vector3.Distance(gameobjects[i].transform.position, transform.position);
            if(newDistance < distance) {
                distance = newDistance;
                index = i;
            }
            index = i;

        }
        
        //LOAD the new target
        IInteract newTarget = index != -1 ? interactables[index] : null;
        if(newTarget != target) {
            if(target != null) {
                OnTargetToggle?.Invoke(false, target);
            }
            target = newTarget;
            if(target != null) {
                OnTargetToggle?.Invoke(true, target);
            }
        }
      
    
        targetBase = target?.GameObject()?.GetComponent<SPBase>();
        hasInteractable = target != null;


        OnInteractUpdate?.Invoke();

        //Debug.Log("Toggle: " + newInteract);
    }

    


}
