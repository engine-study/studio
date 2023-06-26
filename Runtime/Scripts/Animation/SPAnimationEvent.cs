using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimationEvent : MonoBehaviour
{

    [Header("Debug")]
    public bool hasInit;
    public SPAnimator animator;
    public ParticleSystem particles;

    void OnEnable() {
       
        if(!hasInit) {
            Init();
        }

        animator.ToggleAnimationEvent(true, this);
    }


    void OnDisable() {
        animator.ToggleAnimationEvent(false, this);

    }
    public virtual void Init() {

        if(animator == null) {
            animator = GetComponentInParent<SPAnimator>(true);
        } 

        if(!particles) {
            particles = GetComponentInChildren<ParticleSystem>(true);
        }


        hasInit = true;
    }

    public virtual void Fire() {

        if(particles) {
            particles.Play(true);
        }

    }

}
