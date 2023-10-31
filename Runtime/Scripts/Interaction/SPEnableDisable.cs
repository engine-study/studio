using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SPEnableDisable : MonoBehaviour
{

    public bool Active {get{return active;}}
    //clones on disable
    [Header("EnableDisable")]
    [SerializeField] bool active = true;
    public SPEffects onEffects;
    public SPEffects offEffects;

    protected bool hasStarted = false; 
    protected bool hasPlayed = false; 

    public virtual bool CanFire(bool enable, SPEffects effect) { return active && effect != null && hasStarted && effect.isEnabled && !SPGlobal.IsQuitting; }
    
    protected virtual void Awake() {}
    protected virtual void Start() { 
        hasStarted = true; 

        ToggleActive(active);

        if(CanFire(true, onEffects)) 
            Spawn(true);
    }

    public virtual void ToggleActive(bool toggle) {
        active = toggle;
    }

    protected virtual void OnEnable() {
        if(CanFire(true, onEffects))
            Spawn(true);

    }

    protected virtual void OnDisable() {
        if(CanFire(false, offEffects))
            Spawn(false);
    }

    public virtual void Spawn(bool toggle) {Spawn(toggle ? onEffects : offEffects);}
    public virtual void Spawn(SPEffects newEffect) {

        if (newEffect == null) { return; }

        // Debug.Log("Spawning", this);
        hasPlayed = true;

        SPEffects clone = GameObject.Instantiate(newEffect, gameObject.transform.position, gameObject.transform.rotation, null).GetComponent<SPEffects>();
        clone.gameObject.SetActive(true);
        clone.Play();

    }


}