using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using System.Linq;

public class SPActionUI : SPWindowParent {
    public static SPActionUI Instance;
    public SPActor Actor { get { return actor; } }
    public List<SPActionPrompt> Actions { get { return actions; } }

    [Header("Interacting")]
    [SerializeField] SPActionWheelUI wheel;
    [SerializeField] SPActionPrompt actionPrefab;
    [SerializeField] List<SPActionPrompt> actions;

    [Header("Debug")]
    [SerializeField] SPActor actor;
    [SerializeField] List<GameObject> targets;
    [SerializeField] TextMeshProUGUI debugReadout;

    public override void Init() {

        if (hasInit) {
            return;
        }

        base.Init();

        Instance = this;

        for (int i = 0; i < actions.Count; i++) { actions[i].ToggleWindowClose(); targets.Add(null);}

        debugReadout.gameObject.SetActive(debugReadout.gameObject.activeSelf && SPGlobal.IsDebug && Application.isEditor);

        wheel.ToggleWindowClose();
        ToggleWindowOpen();

    }

    public void Setup(SPActor newActor) {

        if (actor != null) {
            actor.OnActionsUpdated -= LoadFromReciever;
            actor.OnTargetsUpdated -= ToggleTarget;
        }

        actor = newActor;
        actor.OnActionsUpdated += LoadFromReciever;
        actor.OnTargetsUpdated += ToggleTarget;

        wheel.Setup(newActor);
    }

    protected override void Destroy() {
        base.Destroy();

        if (actor != null) {
            actor.OnActionsUpdated -= LoadFromReciever;
            actor.OnTargetsUpdated -= ToggleTarget;
        }

    }
    void Update() {


        #if UNITY_EDITOR
        if(!actor || !Actor.ActionScript) {
            debugReadout.text = "";
            return;
        }

        if(Actor.ActionScript != null) {
            debugReadout.text = "Action: " + Actor?.ActionScript.name + "\n";
            debugReadout.text += "State: " + Actor.ActionState.ToString() + "\n";
            debugReadout.text += "Type: " + Actor.ActionScript.Type.ToString() + "\n";
            debugReadout.text += "Cast: " + Actor.CastLerp.ToString("F1") + "\n";
            debugReadout.text += "Action: " + Actor.ActionLerp.ToString("F1") + "\n";
            
        }
        #endif

    }

    void ToggleTarget(bool toggle, IInteract newInteract) {
        if (!hasInit) {
            Init();
        }
        // actions[0].ToggleAction(toggle, actor, newInteract);
        // actions[0].ToggleActionTarget(toggle);
    }

    //     void ToggleTarget(bool toggle, IInteract newInteract)
    // {
    //     int index = targets.IndexOf(newInteract.GameObject());

    //     if(index == -1) {
    //         Debug.LogError(newInteract.GameObject() + " not found", this);
    //         return;
    //     }

    //     SPActionPrompt prompt = activeActions[index];
    //     prompt.ToggleActionTarget(toggle);
    // }


    
    void LoadFromReciever(bool toggle, IInteract newInteract) {
        ToggleAction(toggle, newInteract);
    }

    public void MapInputs() {
        int index = 0;
        for(int i = 0; i < actions.Count; i++) {
            if(actions[i].gameObject.activeSelf ==false) {continue;}
            actions[i].Input.SetKey(SPInput.GetAlphaKey(index));
            index++;
        }
    }

    public void SpawnAction(IInteract newInteract) {

        SPActionPrompt ap = Instantiate(actionPrefab, transform);
        ap.Init();
        ap.wheel = wheel;
        ap.ToggleAction(true, actor, newInteract, true);

    }

    public int GetPrompt() {
        for(int i = 0; i < actions.Count; i++) { if(actions[i].gameObject.activeSelf == false) {return i;}}
        return -1;
    }

    public void ToggleAction(bool toggle, IInteract newInteract, bool silentAdd = false) {

        if (!hasInit) {Init();}

        GameObject targetGO = newInteract.GameObject();
        SPAction ActionScript = newInteract.Action() as SPAction;

        if (targetGO == null) {
            Debug.LogError("No gameobject on interactable", this);
            return;
        }

        if (toggle) {

            int newIndex = GetPrompt();
            if (newIndex == -1) { Debug.LogError("Too many actions"); return; }


            SPActionPrompt action = actions[newIndex];
            // Debug.Log("Adding " + targetGO.name, this);

            action.wheel = wheel;

            targets[newIndex] =  targetGO;

            action.ToggleAction(true, Actor, newInteract, silentAdd);
            action.ToggleActionTarget(true);

        } else {

            int index = targets.IndexOf(targetGO);
            if (index < 0) { Debug.LogError("Couldnt find action?");return;}

            SPActionPrompt action = actions[index];

            action.ToggleAction(false, Actor, newInteract, silentAdd);
            targets[index] = null;

        }

        MapInputs();

    }



}
