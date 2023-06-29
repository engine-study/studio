using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class SPActionUI : SPWindowParent
{
    public SPActor Actor {get{return actor;}}

    [Header("Interacting")]
    public List<SPActionPrompt> actions;
    public List<SPActionPrompt> actionSorted;
    public List<SPActionPrompt> activeActions;

    [Header("Debug")]
    public SPActor actor;
    public List<SPAction> actionStates;
    public List<GameObject> targets;
    public TextMeshProUGUI debugReadout;

    public override void Init()
    {

        if (hasInit)
        {
            return;
        }

        base.Init();

        actionSorted = new List<SPActionPrompt>(actions);
        debugReadout.gameObject.SetActive(SPGlobal.IsDebug && Application.isEditor);

        ToggleWindowOpen();

    }

    public void Setup(SPActor newActor)
    {

        if (actor != null)
        {
            actor.OnActionToggle -= LoadAction;
            actor.OnTarget -= ToggleTarget;
        }

        actor = newActor;
        actor.OnActionToggle += LoadAction;
        actor.OnTarget += ToggleTarget;
    }

    protected override void Destroy()
    {
        base.Destroy();

        if (actor != null)
        {
            actor.OnActionToggle -= LoadAction;
            actor.OnTarget -= ToggleTarget;
        }

    }
#if UNITY_EDITOR
    void Update() {

        if(!actor || !Actor.ActionRef) {
            debugReadout.text = "";
            return;
        }


        if(Actor.ActionRef != null) {
            debugReadout.text = "Action: " + Actor?.ActionRef.name + "\n";
            debugReadout.text += "State: " + Actor.ActionState.ToString() + "\n";
            debugReadout.text += "Type: " + Actor.ActionRef.Type.ToString() + "\n";
            debugReadout.text += "Cast: " + Actor.CastLerp.ToString("F1") + "\n";
            debugReadout.text += "Action: " + Actor.ActionLerp.ToString("F1") + "\n";
            
        }
        
    }
#endif

    void ToggleTarget(bool toggle, IInteract newInteract)
    {
        int index = targets.IndexOf(newInteract.GameObject());

        if(index == -1) {
            Debug.LogError(newInteract.GameObject() + " not found", this);
            return;
        }

        SPActionPrompt prompt = activeActions[index];
        prompt.ToggleActionTarget(toggle);
    }

    void LoadAction(bool toggle, IInteract newInteract)
    {

        SPAction actionRef = newInteract.Action().ActionRef();

        if (toggle)
        {

            if (actions.Count < 1)
            {
                Debug.LogError("Too many actions");
                return;
            }

            GameObject targetGO = newInteract.GameObject();

            if(targetGO == null) {
                Debug.LogError("No gameobject on interactable", this);
                return;
            }

            Debug.Log("Adding " + targetGO.name, this);

            SPActionPrompt newPrompt = actions[0];
            actions.RemoveAt(0);

            targets.Add(targetGO);
            activeActions.Add(newPrompt);
            actionStates.Add(actionRef);

            newPrompt.ToggleAction(true, Actor, newInteract);
        }
        else
        {

            int index = targets.IndexOf(newInteract.GameObject());

            if (index < 0)
            {
                Debug.LogError("Couldnt find action?");
                return;
            }

            SPActionPrompt newPrompt = activeActions[index];

            targets.Remove(newInteract.GameObject());
            activeActions.Remove(newPrompt);
            actionStates.Remove(actionRef);

            actions.Insert(0, newPrompt);

            newPrompt.ToggleAction(false, Actor, newInteract);
        }

    }



}
