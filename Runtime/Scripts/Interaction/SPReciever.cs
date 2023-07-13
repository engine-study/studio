using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SPReciever : MonoBehaviour
{
    public IInteract TargetInteract {get{return targetInteract;}}
    public GameObject TargetGO {get{return targetGO;}}
    public SPBase TargetBase {get{return targetBase;}}
    public bool HasInteractable {get{return hasInteractable;}}
    
    public List<IInteract> Interactables {get{return interactables;}}
    public List<GameObject> GameObjects {get{return gameobjects;}}

    [Header("Interact")]
    [SerializeField] protected GameObject targetGO;
    [SerializeField] protected SPBase targetBase;
    [SerializeField] protected List<GameObject> gameobjects;
    [SerializeField] protected List<IInteract> interactables;

    protected IInteract targetInteract;
    bool hasInteractable = false;



    public System.Action<bool, IInteract> OnInteractToggle;
    public System.Action<bool, IInteract> OnTargetToggle;

    bool hasInit = false; 

    void Awake() {

        Init();

    }

    void Init() {
        
        if(hasInit) {
            return;
        }

        interactables = new List<IInteract>();
        gameobjects = new List<GameObject>();

        hasInit = true; 
    }

    
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

    
    protected void ToggleList(bool toggle, IInteract newInteract) {

        if(toggle) {
            interactables.Add(newInteract);
            gameobjects.Add(newInteract.GameObject());
        } else {
            int index = gameobjects.IndexOf(newInteract.GameObject());
            if(index == -1) {
                Debug.LogError("Cannot find object", this);
                return;
            }
            interactables.RemoveAt(index);
            gameobjects.RemoveAt(index);
        }

        OnInteractToggle?.Invoke(toggle,newInteract);

    }

    public void ToggleInteractableManual(bool toggle, IInteract newInteract) {
        
        if(!hasInit) {
            Init();
        }
        
        ToggleInteractable(toggle,newInteract);
    }

    protected void ToggleInteractable(bool toggle, IInteract newInteract) {

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

    protected void UpdateTarget() {
        
        // Debug.Log("Update Target");

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

        }
                
        //LOAD the new target
        IInteract newTarget = index != -1 ? interactables[index] : null;
        GameObject newTargetGO = index != -1 ? gameobjects[index] : null;

        //check if new gameobject
        //always use gameobjects for comparisons
        if(newTargetGO != targetGO) {

            if(targetGO != null) {
                OnTargetToggle?.Invoke(false, targetInteract);
            }

            //SET the new target
            targetInteract = newTarget;
            targetGO = newTargetGO;
            targetBase = targetGO?.GetComponent<SPBase>();
            hasInteractable = targetGO != null;

            //fire the event
            if(targetGO != null) {
                Debug.Log("New Target: " + newTargetGO.name, this);
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
