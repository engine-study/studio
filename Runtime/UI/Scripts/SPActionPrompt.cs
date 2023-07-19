using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SPActionPrompt : SPWindowParent {

    public SPButton Button { get { return buttonText; } }
    public SPInputPrompt Input { get { return inputText; } }

    [Header("Action Prompt")]
    [SerializeField] private bool worldSpace;
    [SerializeField] private SPButton buttonText;
    [SerializeField] private SPInputPrompt inputText;
    [SerializeField] private GameObject miniPromptParent;
    [SerializeField] private GameObject fullPromptParent;
    [SerializeField] private GameObject promptParent;
    [SerializeField] private SPWindowPosition windowPosition;
    [SerializeField] private Image canPerform;
    [Header("Progress")]
    [SerializeField] public SPActionWheelUI wheel;
    [SerializeField] private Image sweetSpot;

    [Header("Debug")]
    [SerializeField] private SPActor actorComponent;
    [SerializeField] private SPAction actionScript;
    [SerializeField] private IInteract interactable;

    public override void Init() {

        if (hasInit) {
            return;
        }

        base.Init();

        TogglePrompt(false, "");

    }

    void Update() {
        if(!actionScript) {
            return;
        }

        bool allowed = actionScript.TryAction(actorComponent,interactable);
        inputText.ToggleWindow(allowed);
        canPerform.color = allowed ? Color.black : Color.black - Color.black * .5f;

        
    }

    public void ToggleActionTarget(bool toggle) {
        miniPromptParent.SetActive(!toggle);
        fullPromptParent.SetActive(toggle);

    }
    public void ToggleAction(bool toggle, SPActor actor, IInteract interact) {

        actionScript = interact.Action() as SPAction;
        actorComponent = actor;
        interactable = interact;

        if (toggle) {

            actionScript.OnActionStartCasting += StartCast;
            actionScript.OnActionUpdateCasting += UpdateCast;
            actionScript.OnActionEndCasting += EndCast;

            actionScript.OnActionStart += StartAction;
            actionScript.OnActionOver += EndAction;

            actionScript.OnSweetSpotStart += StartSweetSpot;
            actionScript.OnSweetSpotEnd += EndSweetSpot;

            ToggleActionTarget(false);
            ToggleCast(false);
            ToggleSweetSpot(false);

            Button.UpdateField(actionScript.actionName);

            SPBase tryBase = interact.GameObject().GetComponent<SPBase>();

            if (worldSpace)
                windowPosition.SetFollow(tryBase ? tryBase.Root : interact.GameObject().transform);

            // buttonText.UpdateField(key.ToString());

        } else {

            actionScript.OnActionStartCasting -= StartCast;
            actionScript.OnActionUpdateCasting -= UpdateCast;
            actionScript.OnActionEndCasting -= EndCast;

            actionScript.OnActionStart -= StartAction;
            actionScript.OnActionOver -= EndAction;

            actionScript.OnSweetSpotStart -= StartSweetSpot;
            actionScript.OnSweetSpotEnd -= EndSweetSpot;
        }

        ToggleWindow(toggle);

    }

    public void TogglePrompt(bool toggle, string message = "") {

        // Debug.Log("Prompt: " + toggle + " " + message + gameObject.name);
        ToggleWindow(toggle);

        if (toggle) {
            Button.UpdateField(string.IsNullOrEmpty(message) ? "Interact" : message);
        }
    }

    void ToggleCast(bool toggle) {

        // promptParent.SetActive(!toggle);

        if (wheel) {
            wheel.ToggleWindow(toggle);
            wheel.UpdateState(toggle ? ActionEndState.InProgress : ActionEndState.Canceled);
        }

        UpdateCast();

    }

    void StartCast() { ToggleCast(true); }
    void EndCast() { ToggleCast(false); }
    void UpdateCast() {
        UpdateProgress(actorComponent.CastLerp);
    }

    void UpdateProgress(float lerp) {
        if (wheel)
            wheel.UpdateProgress(lerp);
    }

    void StartAction() {
        if (actionScript.Type == ActionType.OneShot || actionScript.Type == ActionType.Hold) {
            // ToggleWindowClose();
        } else if (actionScript.Type == ActionType.Looping) {
            ToggleCast(true);
            UpdateCast();
        } else {
            ToggleCast(false);
        }

        UpdateProgress(1f);

        if (wheel) {
            wheel.UpdateState(ActionEndState.InProgress);
        }
    }

    void UpdateAction() {
        UpdateProgress(actorComponent.ActionLerp);
    }

    void EndAction(ActionEndState endState) {
        ToggleCast(false);

        if (wheel) {
            wheel.UpdateState(endState);
        }
        // ToggleWindowOpen();
    }

    void StartSweetSpot() { ToggleSweetSpot(true); }

    void EndSweetSpot() { ToggleSweetSpot(false); }

    void ToggleSweetSpot(bool toggle) {
        sweetSpot.gameObject.SetActive(toggle);
    }

    //function for button to call submit if the user clicks directly on the UI
    public void Interact() {
        SPUIBase.SendSubmit();
    }
}
