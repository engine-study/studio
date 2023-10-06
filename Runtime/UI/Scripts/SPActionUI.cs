using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SPActionUI : SPWindowParent {
    public static SPActionUI Instance;
    public SPActor Actor { get { return actor; } }
    public List<SPActionPrompt> Actions { get { return activeActions; } }

    [Header("Interacting")]
    [SerializeField] SPActionWheelUI wheel;
    [SerializeField] SPActionPrompt actionPrefab;
    [SerializeField] List<SPActionPrompt> actions;
    [SerializeField] List<SPActionPrompt> actionSorted;
    [SerializeField] List<SPActionPrompt> activeActions;

    [Header("Debug")]
    [SerializeField] SPActor actor;
    [SerializeField] List<SPAction> actionStates;
    [SerializeField] List<GameObject> targets;
    [SerializeField] TextMeshProUGUI debugReadout;

    public override void Init() {

        if (hasInit) {
            return;
        }

        base.Init();

        Instance = this;

        for (int i = 0; i < actions.Count; i++) {
            actions[i].wheel = wheel;
            actions[i].ToggleWindowClose();
        }

        actionSorted = new List<SPActionPrompt>(actions);
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
        for(int i = 0; i < actionSorted.Count; i++) {
            if(actionSorted[i].gameObject.activeSelf ==false) {continue;}
            actionSorted[i].Input.SetKey(SPInput.GetAlphaKey(index));
            index++;
        }
    }

    public void SpawnAction(IInteract newInteract) {

        SPActionPrompt ap = Instantiate(actionPrefab, transform);
        ap.Init();
        ap.ToggleAction(true, actor, newInteract, true);

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

            if (actions.Count < 1) {
                Debug.LogError("Too many actions");
                return;
            }

            Debug.Log("Adding " + targetGO.name, this);

            SPActionPrompt newPrompt = actions[0];
            actions.RemoveAt(0);

            targets.Add(targetGO);
            activeActions.Add(newPrompt);
            actionStates.Add(ActionScript);

            newPrompt.ToggleAction(true, Actor, newInteract, silentAdd);
            newPrompt.ToggleActionTarget(true);
        } else {

            int index = targets.IndexOf(targetGO);

            if (index < 0) {
                Debug.LogError("Couldnt find action?");
                return;
            }

            SPActionPrompt newPrompt = activeActions[index];
            newPrompt.ToggleAction(false, Actor, newInteract, silentAdd);

            targets.Remove(targetGO);
            activeActions.Remove(newPrompt);
            actionStates.Remove(ActionScript);

            actions.Insert(0, newPrompt);

        }

        MapInputs();

    }



}
