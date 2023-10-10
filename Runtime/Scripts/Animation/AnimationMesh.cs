using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationMesh : SPAnimation {

    public Animator Animator {get{return animator;}}
    public SPAnimator AnimatorScript {get{return animatorScript;}}

    [SerializeField] protected Animator animator;
    [SerializeField] protected SPAnimator animatorScript;
    [SerializeField] protected RuntimeAnimatorController originalController;

    [Header("FX")]
    [SerializeField] protected ParticleSystem jumpFX;
    [SerializeField] protected ParticleSystem energyFX, launchFX;
    [SerializeField] protected ParticleSystem bhopFX; 
    ParticleSystem.EmissionModule bhopEmission; 
    ParticleSystem.MainModule bhopMain; 
    bool outOfBoundsFlag = false, hitObject; 

    RaycastHit outOfBoundsTest; 
    float outOfBoundsTime = 5f, outOfBoundsCount, checkCount, checkTime = .1f;

    public override void Init()
    {
        base.Init();

        animatorScript = GetComponent<SPAnimator>();
        originalController = animator.runtimeAnimatorController;

    }


    public void OverrideController(AnimatorOverrideController overrideController) {
        animator.runtimeAnimatorController = overrideController == null ? originalController : overrideController;
    }

    public void ResetAnimation() {
    
        if(!animator.gameObject.activeInHierarchy)
            return;

        animator.SetFloat("Forward", 0f);
        animator.SetFloat("Turn", 0f);
        animator.SetBool("Crouch", false);
        animator.SetBool("OnGround", true);

    }
}
