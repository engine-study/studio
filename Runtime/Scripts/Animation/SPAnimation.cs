using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimation : MonoBehaviour
{
    

    protected Transform Root {get{return player.Root;}}
    protected SPPlayer player; 


    [Header("State")]
    public bool grounded;
    public bool crouching;
    public float speed; 
    public float walkCount = .25f, walkMultiplier = 2f, walkClamp = .1f; 
    public bool moving = false;
    public bool jumping = false;
    public bool hovering = false;
    float hoverCount = 0f, hoverTime = .5f;
    public bool dead = false;
    public bool wasGrounded = false; 

    public Vector3 velocity, newLocalPos, lastLocalPos, animVelocity, velocityNoY;

    public float rotateVel;
    public float m_ForwardAmount, m_StrafeAmount;
    public float m_JumpAmount;
    public float velocityMagnitude, velocityMagHorizontal; 
    public float walkTime = 0f, lastWalk, turnAnimateVel;




    [Header("Sound")]
    [SerializeField] protected float footstepDistance = .25f;
    private float footstepCount = 0f; 
    [SerializeField] protected AudioClip [] footsteps;

    public void SetSpeed(float newSpeed) {speed = newSpeed;}
    public void SetVelocity(Vector3 newVelocity) {velocity = newVelocity;}
    public void SetIsJumping(bool toggle) {jumping = toggle;}
    public void SetIsMoving(bool toggle) {moving = toggle;}
    public void SetIsDead(bool toggle) {dead = toggle;}
    public void SetIsHovering(bool hover) {hovering = hover;}


    protected bool hasInit = false;

    protected virtual void Awake() {
        Init();
    }

    public virtual void Init() {
        
        if(hasInit) {
            return;
        }

        player = GetComponentInParent<SPPlayer>();
        hasInit = true; 

    }

    public virtual void UpdateAnimation() {

        UpdateValues();
        UpdateVisuals();
        UpdateSound();

    }

    protected virtual void UpdateValues() {

        SetVelocity(player.Velocity);

        velocityNoY = velocity;
        velocityNoY.y = 0f;

        velocityMagnitude = velocity.magnitude;
        velocityMagHorizontal = velocityNoY.magnitude;

        moving = velocityNoY.magnitude > 0.1f;
        wasGrounded = grounded; 
        jumping = false && Mathf.Abs(velocity.y) > 0.2f;

        grounded = velocity.y > -.1f && velocity.y < .1f;

        if(velocity.y < -0.5f && velocity.y > -1.5f) {hoverCount += Time.deltaTime;}
        else {hoverCount = 0f;}

        hovering = hoverCount > hoverTime;

        //localalized velocity that drives anzimation values, i.e strafing, running/walking
        newLocalPos = Root.transform.InverseTransformDirection(Root.position);

        //get the local velocity but to remove velocity generated by rotation multiply by the world distnacee as well
        turnAnimateVel = Mathf.Clamp(Mathf.Lerp(turnAnimateVel,rotateVel/360f,.2f), -1f, 1f);

        animVelocity = Root.transform.InverseTransformDirection(velocity) ; //(lastLocalPos - newLocalPos);// * (Vector3.Distance(playerGlobal.position, lastWorldPos));
        lastLocalPos = newLocalPos;

        SetSpeed(animVelocity.z);

        //adjust to 0-1 lerp
        m_ForwardAmount = animVelocity.z; 
        //m_ForwardAmount = m_ForwardAmount > .85f ? 1f : m_ForwardAmount;
        m_StrafeAmount = animVelocity.x;   //Mathf.Atan2(transform.InverseTransformDirection(moveDirection).x, transform.InverseTransformDirection(moveDirection).z);
        m_StrafeAmount += turnAnimateVel * Mathf.Clamp01(m_ForwardAmount * m_ForwardAmount);
        m_JumpAmount = Mathf.MoveTowards(m_JumpAmount, Mathf.Abs(animVelocity.y), Time.deltaTime * 25f);
        //m_StrafeAmount = m_ForwardAmount == 1f ? 0f : m_StrafeAmount;

        //capsule.height = head.position.y - root.position.y + .25f;
        //capsule.center = Vector3.up * capsule.height * .5f;// Vector3.Lerp(collider.center, crouching ? Vector3.up * capsuleCenter * .6f : Vector3.up * capsuleCenter, 5f * Time.deltaTime);

        //Vector3 cartesian = SPHelper.IsometricToCartesian(MoveSpeed).normalized;        
        //SetDirection(SPHelper.VectorToDirection(cartesian));

        //SetDirection(SPHelper.VectorToDirection(transform.forward));

        /*
        if(hovering && !grounded && player.resources.fx_Jump) {
            if(!player.resources.fx_Jump.isPlaying) {player.resources.fx_Jump.Play();}
        } else {
            if(player.resources.fx_Jump.isPlaying) {player.resources.fx_Jump.Stop();}
        }
        */

    }

    protected virtual void UpdateVisuals() {

    }

    protected virtual void UpdateSound() {
        return;
        
        if(moving && grounded) {
            footstepCount += Time.deltaTime;
            if(footstepCount > footstepDistance) {
                footstepCount -= footstepDistance;
                player.Resources.sfx.PlaySound(footsteps, .1f);
            }
        } else {
            footstepCount = 0f; 
        }
    }
    
    public virtual void ToggleDeath(bool toggle) {
        SetIsDead(toggle);
    }
}
