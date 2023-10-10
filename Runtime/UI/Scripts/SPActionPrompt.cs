using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SPActionPrompt : SPWindow {

    public SPButton Button { get { return buttonText; } }
    public SPInputPrompt Input { get { return inputText; } }
    public IInteract Interactable { get { return interactable; } }

    [Header("Action Prompt")]
    [SerializeField] bool worldSpace;
    [SerializeField] SPButton buttonText;
    [SerializeField] SPInputPrompt inputText;
    [SerializeField] GameObject miniPromptParent;
    [SerializeField] GameObject fullPromptParent;
    [SerializeField] GameObject promptParent;
    [SerializeField] SPWindowPosition windowPosition;
    [SerializeField] Image canPerform;
    [Header("Progress")]
    [SerializeField] public SPActionWheelUI wheel;
    [SerializeField] Image sweetSpot;

    [Header("Debug")]
    [SerializeField] SPActor actorComponent;
    [SerializeField] SPAction actionScript;
    [SerializeField] IInteract interactable;
    [SerializeField] public int index;

    public override void Init() {

        if (hasInit) { return;}

        base.Init();

        windowPosition.enabled = worldSpace;
    }

    void Update() {

        if(!actionScript) { return;}

        UpdateActionInput();
        UpdateState();
        
    }

    void UpdateState() {

        bool allowed = actionScript.TryAction(actorComponent,interactable);
        inputText.ToggleWindow(allowed);
        canPerform.color = allowed ? Color.black : Color.black - Color.black * .5f;
    }

    void UpdateActionInput() {
        if(SPUIBase.CanInput && SPPlayer.CanInput) {actorComponent.InputKey(Input.Key, interactable);}
    }

    public void ToggleActionTarget(bool toggle) {
        miniPromptParent.SetActive(!toggle);
        fullPromptParent.SetActive(toggle);
    }
    
    public void ToggleAction(bool toggle, SPActor actor, IInteract interact, bool silentAdd = false) {

        actionScript = interact.Action() as SPAction;
        actorComponent = actor;
        interactable = interact;

        if (toggle) {

            gameObject.name = interact.GameObject().name;

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


            if (worldSpace) {
                // SPBase tryBase = interact.GameObject().GetComponent<SPBase>();
                // windowPosition.SetFollow(tryBase ? tryBase.Root : interact.GameObject().transform);
                windowPosition.SetFollow(interact.GameObject().transform, Vector3.down * (index+1f));
            }

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

        if(silentAdd) ToggleWindowClose();
        else ToggleWindow(toggle);

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
            wheel.UpdateProgress(0f);
            wheel.UpdateState(toggle ? ActionEndState.InProgress : ActionEndState.Canceled, true);
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
