using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SPInteractReciever : MonoBehaviour
{
    public IInteract TargetInteract {get{return targetInteract;}}
    public GameObject TargetGO {get{return targetGO;}}
    public SPBase TargetBase {get{return targetBase;}}
    public bool HasInteractable {get{return hasInteractable;}}
    
    public List<IInteract> Interactables {get{return interactables;}}
    public List<GameObject> GameObjects {get{return gameobjects;}}


    [Header("Interact")]
    protected IInteract targetInteract;
    protected GameObject targetGO;
    protected SPBase targetBase;
    bool hasInteractable = false;

    [SerializeField] protected List<IInteract> interactables;
    [SerializeField] protected List<GameObject> gameobjects;

    public Action<bool, IInteract> OnInteractToggle;
    public Action<bool, IInteract> OnTargetToggle;


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

    float distanceCheck = .1f;
    void Update() {

        distanceCheck -= Time.deltaTime;
        if(distanceCheck < 0f) {
            distanceCheck += .1f;
            UpdateTarget();
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
     
    
    void ToggleList(bool toggle, IInteract newInteract) {
        if(toggle) {
            interactables.Add(newInteract);
            gameobjects.Add(newInteract.GameObject());
        } else {
            int index = gameobjects.LastIndexOf(newInteract.GameObject());
            if(index == -1) {
                Debug.LogError("Cannot find object", this);
                return;
            }
            interactables.RemoveAt(index);
            gameobjects.RemoveAt(index);
        }

        OnInteractToggle?.Invoke(toggle,newInteract);

    }

    void ToggleInteractable(bool toggle, IInteract newInteract) {

        GameObject go = newInteract.GameObject();

        if(go == null) {
            Debug.LogError("NO gameobject", this);
        }

        //we shouldn't be toggling if we already have the object or have already disposed of it
        bool contains = gameobjects.Contains(go);
        if((toggle && contains) || (!toggle && !contains)) {
            return;
        }

        if(toggle) {
            // Debug.Log("Add Interactable: " + newInteract.GameObject().name);
            ToggleList(true, newInteract);

        } else {
            // Debug.Log("Remove Interactable: " + newInteract.GameObject().name);
            ToggleList(false, newInteract);

        }


        //creates action that lets the UI know that an interactble object has either left the player or entered the players range

        UpdateTarget();

        //Debug.Log("Toggle: " + newInteract);
    }

    void UpdateTarget() {
        
        float distance = 9999f;
        int index = -1;
        //iterate backwards in case we delete elements
        for(int i = gameobjects.Count - 1; i > -1; i--) {

            if(gameobjects[i] == null) {
                Debug.LogError("A gameobject became null", this);
                ToggleList(false, null);
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
        GameObject newTargetGO = newTarget != null ? newTarget.GameObject() : null; 

        //always use gameobjects for comparisons
        if(newTargetGO != targetGO) {

            if(targetInteract != null) {
                OnTargetToggle?.Invoke(false, targetInteract);
            }

            //SET the new target
            targetInteract = newTarget;
            targetGO = newTargetGO;
            targetBase = targetInteract?.GameObject()?.GetComponent<SPBase>();
            hasInteractable = targetInteract != null;

            //fire the event
            if(targetInteract != null) {
                OnTargetToggle?.Invoke(true, targetInteract);
            }
        }

    }

     void OnDrawGizmos() {
        for(int i = 0; i < GameObjects.Count; i++) {
            if(GameObjects[i] == TargetGO) {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(GameObjects[i].transform.position, transform.position);
            } else {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(GameObjects[i].transform.position, transform.position);
            }
        }
    }
    


}
