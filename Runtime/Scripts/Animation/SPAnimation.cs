using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAnimation : MonoBehaviour
{
    

    public SPVelocity Velocity {get{return velocityScript;}}
    protected Transform Root {get{return transform.parent;}}

    [Header("Animation")]
    [SerializeField] protected SPVelocity velocityScript;

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

        hasInit = true; 

    }

    public void LateUpdate() {

        UpdateVisuals();

    }
    
    protected virtual void UpdateVisuals() {

    }

    
    public virtual void ToggleDeath(bool toggle) {
        SetIsDead(toggle);
    }
}
