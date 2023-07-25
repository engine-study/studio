using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimationProp : MonoBehaviour
{
    [Header("Effects")]

    public ParticleSystem fx;
    public AudioClip [] sfx;

    [Header("Position")]
    public PlayerBody bodyParent;
    public Transform bodyProp;

    [Header("Debug")]
    public bool hasInit;

    void OnEnable() {
       
        if(!hasInit) {
            Init();
        }

    }


    void OnDisable() {

        // animator.ToggleAnimationEvent(false, this);

    }
    
    public virtual void Init() {

        if(!fx) {
            fx = GetComponentInChildren<ParticleSystem>(true);
        }


        hasInit = true;
    }

    public virtual void Fire() {

        if(fx) {
            fx.Play(true);
        }

        if(sfx.Length > 0) {
            SPAudioSource.Play(transform.position,sfx);
        }

    }

}
