using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPActionWheelUI : MonoBehaviour
{
    [Header("Wheel")]
    public Image actionWheel;
    public SPWindowPosition actionPosition;


    public void Setup(SPActor newActor) {
        actionPosition.SetFollow(newActor.GetComponent<SPBase>().Root);
    }

}
