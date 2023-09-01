using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SPEnableDisable : MonoBehaviour
{

    //clones on disable
    [Header("EnableDisable")]
    public bool active = true;
    public SPEffects onEffects;
    public SPEffects offEffects;

    protected bool hasStarted = false; 
    protected bool hasPlayed = false; 

    public virtual bool CanPlay(bool enable, SPEffects effect) { return active && effect != null && hasStarted && effect.isEnabled && !SPGlobal.IsQuitting; }
    
    protected virtual void Awake() {}
    protected virtual void Start() { 
        hasStarted = true; 

        if(CanPlay(true, onEffects)) 
            PlayEnabled();
    }

    protected virtual void OnEnable() {
        if(CanPlay(true, onEffects))
            PlayEnabled();

    }

    protected virtual void OnDisable() {
        if(CanPlay(false, offEffects))
            PlayDisabled();
    }

    public void PlayEnabled() {
        Spawn(onEffects);
    }

    public void PlayDisabled() {
        Spawn(offEffects);
    }

    public virtual void Spawn(SPEffects newEffect) {

        if (newEffect == null) { return; }

        // Debug.Log("Spawning", this);
        hasPlayed = true;

        SPEffects clone = GameObject.Instantiate(newEffect, gameObject.transform.position, gameObject.transform.rotation, null).GetComponent<SPEffects>();
        clone.Play();

    }


}