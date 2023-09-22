using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPActionWheelUI : SPWindow
{
    public enum WheelState{Empty, InProgress, Success, Failure}
    [Header("Wheel")]
    public CanvasGroup group;
    public Image actionWheel;
    public Image actionGlow;
    public SPWindowPosition actionPosition;
    public Color inProgress, success, failure;
    public bool Lock = false;

    [Header("Debug")]
    public ActionEndState state;

    Coroutine linger;

    public void Setup(SPActor newActor) {
        actionPosition.SetFollow(newActor.GetComponent<SPBase>().Root);
        UpdateProgress(0f);

    }

    public void UpdateProgress(float lerp) {
        if(Lock) {return;}
        actionWheel.fillAmount = lerp;
    }

    public void UpdateState(ActionEndState newState, bool force = false) {
        
        //same state
        if(Lock) {return;}
        if(newState == state && !force) {return;}

        if(linger != null) { StopCoroutine(linger);}

        group.transform.localScale = Vector3.one;
        group.alpha = 1f;

        actionGlow.gameObject.SetActive(false);
        ToggleWindow(newState != ActionEndState.Canceled);

        if(newState == ActionEndState.InProgress) {
            actionWheel.color = inProgress;
            actionGlow.color = inProgress - Color.black * .5f;
            if(actionWheel.fillAmount == 1f) {
                linger = StartCoroutine(PendingCoroutine());
            }
        } else if(newState == ActionEndState.Success) {
            actionWheel.fillAmount = 1f;
            actionWheel.color = success;
            actionGlow.color = success - Color.black * .5f;
            linger = StartCoroutine(LingerCoroutine());

        } else if(newState == ActionEndState.Canceled) {
            //do nothing, turn off
        } else if(newState == ActionEndState.Failed) {
            actionWheel.fillAmount = 1f;
            actionWheel.color = failure;
            actionGlow.color = failure - Color.black * .5f;
            linger = StartCoroutine(LingerCoroutine());
        }
        

        state = newState;

    }

    public void ActionPending() {
        Lock = false;
        UpdateProgress(1f);
        UpdateState(ActionEndState.InProgress, true);
        Lock = true;
    }

    public void ActionRelease(ActionEndState newState, bool force = false) {
        Lock = false;
        UpdateState(newState, force);
        
    }


    IEnumerator PendingCoroutine() {

        actionGlow.gameObject.SetActive(true);
        float lerp = 0f;
        Color startColor = actionGlow.color;

        while(true) {
            lerp += Time.deltaTime;
            float size = Mathf.Sin(lerp * 25f) * .5f + .5f;
            group.transform.localScale = Vector3.one * (size * .75f + .5f);
            actionGlow.color = startColor - Color.black * size;
            yield return null;
        }

    }

    IEnumerator LingerCoroutine() {
        actionGlow.gameObject.SetActive(true);
        float lerp = 0f;

        while(lerp < 1f) {
            lerp += Time.deltaTime * 5f;
            group.transform.localScale = Vector3.one * (1f + lerp);
            group.alpha = 1f-lerp;
            yield return null;
        }

        linger = null;
        actionWheel.fillAmount = 0f;
        ToggleWindowClose();
    }
}
