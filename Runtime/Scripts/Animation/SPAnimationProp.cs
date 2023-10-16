using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimationProp : MonoBehaviour
{
    public SPAnimator Animator {get{return animator;}}
    public string Name {get{return gameObject.name;}}
    
    [Header("Effects")]
    public ParticleSystem fx;
    public AudioClip [] sfx;

    [Header("Position")]
    public PlayerBody bodyParent;
    public Transform bodyProp;

    [Header("Debug")]
    [SerializeField] protected bool hasInit;
    [SerializeField] protected SPAnimator animator;
    
    void OnEnable() {
        if(!hasInit) { Init();}
    }

    void OnDisable() {
        // animator.ToggleAnimationEvent(false, this);
    }

    public virtual void SetAnimator(SPAnimator newAnimator) {
        animator = newAnimator;
    }
    
    public virtual void Init() {
        if(!fx) { fx = GetComponentInChildren<ParticleSystem>(true);}
        hasInit = true;
    }

    public virtual void Fire(string actionName) {
        if(fx) { fx.Play(true);}
        if(sfx.Length > 0) { SPAudioSource.Play(transform.position,sfx);}
    }

}
