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
    public override void UpdateAnimation() {
        base.UpdateAnimation();

        if(animator.gameObject.activeSelf) {

            animator.SetBool("Weapon", false); //_currentUsable is de_gun);
            animator.SetFloat("Forward", m_ForwardAmount ,.075f,Time.deltaTime);
            animator.SetFloat("Strafe", m_StrafeAmount, .075f,Time.deltaTime);
            animator.SetBool("Crouch", crouching);

            //stops the standing animation from playing when the player is first starting to bhop
            animator.SetBool("OnGround", wasGrounded && grounded); //wasGrounded && grounded && (Mathf.Abs(animVelocity.z) < de_player_controller.walkSpeed * 1.5f)); //
            animator.SetFloat("Jump", m_JumpAmount);
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)

            float runCycle = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime + .35f, 1);//m_RunCycleLegOffset = .35f
            float jumpLeg = (runCycle < .5f ? 1 : -1) * m_ForwardAmount;

            if (animator.GetBool("OnGround")) {
                animator.SetFloat("JumpLeg", jumpLeg);
                // animator.speed = 1.35f;  //m_AnimSpeedMultiplier = 1.35f;
            }
            else {
                // animator.speed = 1f;
                animator.SetFloat("JumpLeg", 0f); 
            }
        }

        if(grounded) {
            
            //float footstepTime = Mathf.Max(Mathf.Abs(m_ForwardAmount), Mathf.Abs(m_StrafeAmount)) < .9f ? .6f : 1f;
            //footstepTime *= Time.deltaTime;
            float footstepTime = Mathf.Clamp(Mathf.Max(Mathf.Abs(m_ForwardAmount), Mathf.Abs(m_StrafeAmount)) * walkMultiplier, 0f, walkClamp) * Time.deltaTime;
            walkTime -= footstepTime;

            if(walkTime < 0f) {
                //PlaySound(player.s_walk);
                walkTime = walkCount;
            }
        }


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
