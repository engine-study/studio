using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SPActionPrompt : SPWindowParent
{

    public SPButton Button { get { return buttonText; } }
    public SPButton Input { get { return inputText; } }

    [Header("Action Prompt")]
    public bool worldSpace;
    [SerializeField] protected SPButton buttonText;
    [SerializeField] protected SPButton inputText;
    [SerializeField] protected GameObject miniPromptParent;
    [SerializeField] protected GameObject fullPromptParent;
    [SerializeField] protected GameObject promptParent;
    [SerializeField] protected SPWindowPosition windowPosition;

    [Header("Progress")]
    public SPActionWheelUI wheel;
    [SerializeField] protected Image sweetSpot;

    [Header("Debug")]
    [SerializeField] public SPActor actorComponent;
    [SerializeField] public SPAction action;
    [SerializeField] public IInteract interact;

    public override void Init()
    {

        if (hasInit)
        {
            return;
        }

        base.Init();

        TogglePrompt(false, "");

    }

    public void ToggleActionTarget(bool toggle)
    {
        miniPromptParent.SetActive(!toggle);
        fullPromptParent.SetActive(toggle);

    }
    public void ToggleAction(bool toggle, SPActor actor, IInteract interact)
    {

        action = interact.Action().ActionRef();
        actorComponent = actor;

        if (toggle)
        {

            action.OnActionStartCasting += StartCast;
            action.OnActionUpdateCasting += UpdateCast;
            action.OnActionEndCasting += EndCast;

            action.OnActionStart += StartAction;
            action.OnActionOver += EndAction;

            action.OnSweetSpotStart += StartSweetSpot;
            action.OnSweetSpotEnd += EndSweetSpot;

            ToggleActionTarget(false);
            ToggleCast(false);
            ToggleSweetSpot(false);

            Button.UpdateField(action.actionName);

            SPBase tryBase = interact.GameObject().GetComponent<SPBase>();

            if (worldSpace)
                windowPosition.SetFollow(tryBase ? tryBase.Root : interact.GameObject().transform);

            // buttonText.UpdateField(key.ToString());

        }
        else
        {

            action.OnActionStartCasting -= StartCast;
            action.OnActionUpdateCasting -= UpdateCast;
            action.OnActionEndCasting -= EndCast;

            action.OnActionStart -= StartAction;
            action.OnActionOver -= EndAction;

            action.OnSweetSpotStart -= StartSweetSpot;
            action.OnSweetSpotEnd -= EndSweetSpot;
        }

        ToggleWindow(toggle);

    }

    public void TogglePrompt(bool toggle, string message = "")
    {

        // Debug.Log("Prompt: " + toggle + " " + message + gameObject.name);
        ToggleWindow(toggle);

        if (toggle)
        {
            Button.UpdateField(string.IsNullOrEmpty(message) ? "Interact" : message);
        }
    }

    void ToggleCast(bool toggle)
    {

        promptParent.SetActive(!toggle);

        if (wheel)
        {
            wheel.ToggleWindow(toggle);
            wheel.UpdateState(toggle ? ActionEndState.InProgress : ActionEndState.Canceled);
        }

        UpdateCast();

    }

    void StartCast() { ToggleCast(true); }
    void EndCast() { ToggleCast(false); }
    void UpdateCast()
    {
        UpdateProgress(actorComponent.CastLerp);
    }

    void UpdateProgress(float lerp)
    {
        if (wheel)
            wheel.UpdateProgress(lerp);
    }

    void StartAction()
    {
        if (action.Type == ActionType.OneShot || action.Type == ActionType.Hold)
        {
            // ToggleWindowClose();
        }
        else if (action.Type == ActionType.Looping)
        {
            ToggleCast(true);
            UpdateCast();
        }
        else
        {
            ToggleCast(false);
        }

        UpdateProgress(1f);

        if (wheel)
        {
            wheel.UpdateState(ActionEndState.InProgress);
        }
    }

    void UpdateAction()
    {
        UpdateProgress(actorComponent.ActionLerp);
    }

    void EndAction(ActionEndState endState)
    {
        ToggleCast(false);

        if(wheel) {
            wheel.UpdateState(endState);
        }
        // ToggleWindowOpen();
    }

    void StartSweetSpot() { ToggleSweetSpot(true); }

    void EndSweetSpot() { ToggleSweetSpot(false); }

    void ToggleSweetSpot(bool toggle)
    {
        sweetSpot.gameObject.SetActive(toggle);
    }

    //function for button to call submit if the user clicks directly on the UI
    public void Interact()
    {
        SPUIBase.SendSubmit();
    }
}
