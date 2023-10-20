using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SPTriggerReciever : SPReciever
{


    float distanceCheck = .2f;
    void Update() {

        distanceCheck -= Time.deltaTime;
        if(distanceCheck < 0f) {
            distanceCheck += .2f;
            UpdateTarget();
        }

    }

    protected virtual void OnTriggerEnter(Collider other) {

        if(!enabled) {
            return;
        }

        IInteract i = other.GetComponentInParent<IInteract>();
        //Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        if(i != null && i.GameObject() != gameObject) {
            ToggleInteractable(true, i);
        }

    }

    protected virtual void OnTriggerExit(Collider other) {

        if(!enabled) {
            return;
        }
        
        IInteract i = other.GetComponentInParent<IInteract>();
        //Debug.Log("OnTriggerExit: " + other.gameObject.name);
        if(i != null && i.GameObject() != gameObject) {
            ToggleInteractable(false, i);
        }

    }
     
    
    


}
