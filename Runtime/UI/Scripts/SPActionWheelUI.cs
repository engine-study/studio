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

    [Header("Debug")]
    public ActionEndState state;

    Coroutine linger;

    public void Setup(SPActor newActor) {
        actionPosition.SetFollow(newActor.GetComponent<SPBase>().Root);
        UpdateProgress(0f);

    }

    public void UpdateProgress(float lerp) {
        actionWheel.fillAmount = lerp;
    }

    public void UpdateState(ActionEndState newState, bool force = false) {
        
        if(linger != null) {
            StopCoroutine(linger);
        }

        group.transform.localScale = Vector3.one;
        group.alpha = 1f;

        if(newState != state || force) {

            actionGlow.gameObject.SetActive(false);
            ToggleWindow(newState != ActionEndState.Canceled);

            if(newState == ActionEndState.InProgress) {
                actionWheel.color = inProgress;
                actionGlow.color = inProgress;
                if(actionWheel.fillAmount == 1f) {
                    linger = StartCoroutine(PendingCoroutine());
                }
            } else if(newState == ActionEndState.Success) {
                actionWheel.fillAmount = 1f;
                actionWheel.color = success;
                actionGlow.color = success;
                linger = StartCoroutine(LingerCoroutine());

            } else if(newState == ActionEndState.Canceled) {
                actionWheel.fillAmount = 1f;

            } else if(newState == ActionEndState.Failed) {
                actionWheel.fillAmount = 1f;
                actionWheel.color = failure;
                actionGlow.color = failure;
                linger = StartCoroutine(LingerCoroutine());
            }
        }

        state = newState;

    }

    public void ActionPending() {
        actionWheel.fillAmount = 1f;
        UpdateState(ActionEndState.InProgress);
    }


    IEnumerator PendingCoroutine() {
        // actionGlow.gameObject.SetActive(true);
        float lerp = 0f;

        while(true) {
            lerp += Time.deltaTime;
            float size = Mathf.Sin(lerp) * .5f + .5f;
            group.transform.localScale = Vector3.one * (size + 1f);
            group.alpha = 1f-size;
            yield return null;
        }

    }

    IEnumerator LingerCoroutine() {
        // actionGlow.gameObject.SetActive(true);
        float lerp = 0f;

        while(lerp < 1f) {
            lerp += Time.deltaTime * 5f;
            group.transform.localScale = Vector3.one * (1f + lerp * .5f);
            group.alpha = 1f-lerp;
            yield return null;
        }

        linger = null;
        ToggleWindowClose();
    }
}
