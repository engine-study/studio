using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPController : MonoBehaviour
{

    public float Speed {get{return speed;}}
    public bool Active {get{return hasInit && enabled;}}

    [Header("Fields")]
    [SerializeField] protected SPPlayer player;
    [SerializeField] protected SPLogic Logic {get{return player.Logic;}}
    [SerializeField] protected Collider mainCollider;
    [SerializeField] protected CharacterController controller;
    [SerializeField] protected SPAnimator animator;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected SPVelocity velocity;
    [SerializeField] protected Rigidbody[] rigidbodies;

    [Header("Controller")]
    [SerializeField] protected bool grounded = false;
    [SerializeField] protected bool bumped = false;
    [SerializeField] protected bool crouching = false, bhopping = false, noclip = false, jumping = false;
    [SerializeField] protected bool canBhop = false; 
    [SerializeField] protected float airAccel = 1f, minAirAccel = 1f, maxAirAccel = 6f, bhopBoost = .25f;


    [Header("Debug")]
    public Vector3 moveDirection = Vector3.zero;
    public Vector3 lookAt = Vector3.zero;
    public Vector3 moveDirectionNoY, forceVelocity;
    public float tagging = 1f;
    public bool detectCollisions = true; 
    public float Gravity {get{return Physics.gravity.y;}}
    float bumpTime = 0f; 


    protected bool hasInit = false;

    protected float m_CapsuleHeight;
    protected Vector3 m_CapsuleCenter;
    protected bool inWater = false; 
    protected int jumpCount = 0; 
    protected Transform Transform;
    protected float speed;
    protected RaycastHit hit;
    protected float fallStartLevel;
    protected bool ragdoll;
    protected bool falling;
    protected float slideLimit {get{return controller.slopeLimit - .1f;}}
    protected float rayDistance;
    protected Vector3 contactPoint;
    protected bool playerControl = false;
    protected float jumpTimer;
    protected bool hasCrouched = false; 
    protected float largestInputY;
    Vector3 animatorLocalPos;      
    Transform animatorParent;      

    void Awake() {
        if(!hasInit) { Init();}
    }

    void Destroy() {

    }

    public virtual void Init() {
        
        if(hasInit) { return; }

        player = GetComponent<SPPlayer>();
        velocity = GetComponent<SPVelocity>();

        mainCollider = GetComponent<Collider>();
        if(mainCollider == null) {
            var capsule = gameObject.AddComponent<CapsuleCollider>();
            capsule.height = 1.5f;
            capsule.radius = .2f;
            mainCollider = capsule;
        }

        controller = GetComponent<CharacterController>();
        if(controller == null) {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 1.25f;
            controller.radius = .1f;
        }

        rb = GetComponent<Rigidbody>();

        if(animator == null) animator = GetComponentInChildren<SPAnimator>(true);
        if(animator) {
            rigidbodies = animator.GetComponentsInChildren<Rigidbody>(true);
            animatorParent = animator.transform.parent;
            animatorLocalPos = animator.transform.localPosition;
        }

        Transform = transform;
        rayDistance = controller.height * .5f + controller.radius;

        //ANIMATOR
        m_CapsuleHeight = controller.height;
        m_CapsuleCenter = controller.center;
    
        hasInit = true; 

        Ragdoll(false);

    }

    public virtual void ToggleController(bool toggle) {

        controller.enabled = toggle;
        controller.detectCollisions = toggle;
        enabled = toggle;
    }


    public void CallFixedUpdate() {

        if(!Active) {
            return;
        }

        if(bumped) {
            bumpTime -= Time.fixedDeltaTime;
            if(bumpTime < 0f) {
                bumped = false; 
            }
        }

        Move();
    }
    
    public virtual void Move() {

    }


    public void Ragdoll() {Ragdoll(!ragdoll);}
    public void Ragdoll(bool toggle) {

        if(!hasInit) {Init();}

        Debug.Log($"RAGDOLL: {toggle}" , this);
        ragdoll = toggle;

        if(!animator) { return; }

        ToggleController(!toggle);

        mainCollider.enabled = !toggle;

        animator.enabled = !toggle;
        animator.transform.parent = toggle ? null : animatorParent;

        if(!toggle) {
            animator.transform.localPosition = animatorLocalPos;
            animator.transform.localRotation = Quaternion.identity;
        }

        for(int i = 0; i < rigidbodies.Length; i++) {

            rigidbodies[i].isKinematic = !toggle; 
            rigidbodies[i].detectCollisions = toggle; 

            if(toggle) {

                if(player) {
                    rigidbodies[i].velocity = player.Velocity.Velocity - player.Root.forward + Vector3.down;
                } else {
                    rigidbodies[i].velocity = transform.forward + Vector3.down * Random.Range(.75f,1.25f);
                }

            } else {
                // rigidbodies[i].velocity = Vector3.zero;
            }
        }
    }


    public virtual void Teleport(Vector3 newPosition) {
        bool wasEnabled = Active;

        ToggleController(false);

        transform.position = newPosition;
        rb.position = newPosition;
        
        ToggleController(wasEnabled);

    }

    public virtual void ResetPlayer(bool isDead = false) {

        if(isDead) {
            SetDetectCollisions(false);
            //player.playerCollider.enabled = false; 
        }

        moveDirection = Vector3.zero; 
        airAccel = 1f;
    }

    
    public virtual void AddForce(Vector3 force, ForceMode forceMode = ForceMode.Force) {
        forceVelocity += force;
        forceVelocity = Vector3.ClampMagnitude(forceVelocity, 250f);
    }
 

    public virtual void SetDetectCollisions(bool toggle) {
        controller.detectCollisions = toggle;
    }
    public virtual void ToggleNoClip(bool toggle) {
        
        if (toggle) {
            airAccel = 1f;
            grounded = false;
        }

        noclip = toggle;
    }

    protected virtual void OnDrawGizmosSelected() {

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.up * .05f, transform.position + lookAt * .15f + Vector3.up * .05f);
    }
    
    protected virtual void OnCollisionEnter(Collision collision) {

        if(collision.gameObject.layer != SPLayers.PlayerLayer) {
            return;
        }

        SPController other = collision.gameObject.GetComponent<SPController>();

        if(!other) {
            return;
        }
        
        if(other == gameObject) {
            return;
        }

        Bump(collision, other.GetComponent<SPPlayer>());

    }

    protected virtual void Bump(Collision collision, SPPlayer newPlayer = null) {

        bumped = true; 
        bumpTime = 2f; 

        rb.velocity = newPlayer.Velocity.Velocity * 1.25f + Vector3.up * 10f * Mathf.Clamp01(newPlayer.Velocity.Velocity.magnitude);


    }

}
