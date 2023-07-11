using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPActionWheelUI : MonoBehaviour
{
    public enum WheelState{Empty, InProgress, Success, Failure}
    [Header("Wheel")]
    public Image actionWheel;
    public SPWindowPosition actionPosition;
    public WheelState state;


    public void Setup(SPActor newActor) {
        actionPosition.SetFollow(newActor.GetComponent<SPBase>().Root);
    }

    public void UpdateProgress(float lerp) {

    }

    public void UpdateState(WheelState newState) {
        
        if(newState != state) {

        }

        state = newState;

    }
}
