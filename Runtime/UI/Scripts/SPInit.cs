using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPInit : MonoBehaviour
{
    void Awake() {
        gameObject.BroadcastMessage("Init",SendMessageOptions.DontRequireReceiver);
    }
}
