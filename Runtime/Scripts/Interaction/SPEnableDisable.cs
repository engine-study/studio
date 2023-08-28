using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SPEnableDisable : MonoBehaviour
{

    //clones on disable
    [Header("EnableDisable")]
    public SPEffects onEffects;
    public SPEffects offEffects;

    bool hasStarted = false; 

    public virtual bool CanPlay(SPEffects effect) { return effect != null && hasStarted && effect.isEnabled && !SPGlobal.IsQuitting; }
    
    protected virtual void Awake() {}
    protected virtual void Start() { 
        hasStarted = true; 

        if(CanPlay(onEffects)) 
            Spawn(onEffects);
    }

    protected virtual void OnEnable() {
        if(CanPlay(onEffects)) 
            Spawn(onEffects);
    }

    protected virtual void OnDisable() {
        if(CanPlay(offEffects)) 
            Spawn(offEffects);
    }

    public void Spawn(SPEffects newEffect) {

        Debug.Log("Spawning", this);

        SPEffects clone = GameObject.Instantiate(newEffect, gameObject.transform.position, gameObject.transform.rotation, null).GetComponent<SPEffects>();
        clone.Play();

    }


}