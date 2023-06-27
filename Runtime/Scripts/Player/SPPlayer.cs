using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPPlayer : SPBase
{
    public bool IsLocalPlayer {get{return isLocalPlayer;}}
    public static SPPlayer LocalPlayer {get{return localPlayer;}}
    protected static SPPlayer localPlayer;

    public System.Action<IAction> OnPlayerAction;

    public SPController Controller { get { return controller; } }
    public SPAnimation Animation { get { return anim; } }
    public SPAnimator Animator { get { return animator; } }
    public SPActor Actor { get { return actor; } }

    public bool Alive { get { return alive; } }
    public Vector3 Vector { get { return vector; } }
    public Vector3 Velocity { get { return velocity; } }
    protected Vector3 vector;
    protected Vector3 velocity;
    protected bool isMoving, noclip;
    protected bool alive = true;
    protected bool isLocalPlayer = false;

    [Header("Player")]
    [SerializeField] protected bool isNPC;

    [HideInInspector] public SPLogic logic;
    [HideInInspector] public SPController controller;
    [HideInInspector] public SPAnimation anim;
    [HideInInspector] public SPActor actor;
    [HideInInspector] public SPAnimator animator;
    [HideInInspector] public SPInteractReciever reciever;
    [HideInInspector] public SPPlayerResources resources;


    protected override void Awake()
    {

        base.Awake();

        if (resources == null)
        {
            resources = GetComponentInChildren<SPPlayerResources>();
        }

        anim = GetComponentInChildren<SPAnimation>();
        if (anim == null)
        {
            Debug.LogWarning(gameObject.name + ": No animation system", gameObject);
            anim = gameObject.AddComponent<SPAnimation>();
        }

        animator = GetComponentInChildren<SPAnimator>(true);

        if (reciever == null)
        {
            reciever = gameObject.GetComponentInChildren<SPInteractReciever>();
            reciever.OnInteractUpdate += UpdateInteract;
        }

        if (actor == null)
        {
            actor = gameObject.AddComponent<SPActor>();
            actor.enabled = false;
            actor.sender = this;
            actor.reciever = reciever;
            actor.OnAction += OnPlayerAction;
        }

        base.Awake();
    }

    public static void SetLocalPlayer(SPPlayer newLocal) {
        localPlayer = newLocal;
        newLocal.isLocalPlayer = true;
    }


    protected override void Destroy()
    {
        base.Destroy();

        reciever.OnInteractUpdate -= UpdateInteract;
        actor.OnAction -= OnPlayerAction;

    }


    protected override void NetworkInit()
    {
        base.NetworkInit();

        alive = true;

        if (IsLocal)
        {
            logic.Init();
            controller.Init();
            Controller.ToggleController(IsLocal);
        }

        if (IsLocal)
        {
            RespawnLocal();
        }

    }


    protected override void Update()
    {

        base.Update();

        if (IsLocal)
        {

            UpdateInput();
            UpdateLogic();

        }
        else
        {

        }

    }



    protected virtual void UpdateLogic()
    {
        logic.UpdateInput();
    }

    public virtual void LateUpdate()
    {
        UpdateAnimation();
    }

    public virtual void FixedUpdate()
    {

        if (!Alive)
            return;

        if (IsLocal)
        {
            Controller.CallFixedUpdate();
        }

        CacheFixedValues();

    }

    float fixedDelta = 0f;
    protected float rotateVel;
    protected Vector3 lastWorldPos, lastForward; //velocity of character
    protected Quaternion lastRot;

    void CacheFixedValues()
    {

        /*
        if(!Alive)
            return;
        */

        //add time since the last position update
        fixedDelta = Time.fixedDeltaTime;

        //Used to calculate ragdoll velocity and animation
        velocity = Vector3.Lerp(velocity, (Root.position - lastWorldPos) / fixedDelta, .5f);

        //Used for player animation
        float newRotateVel = (Quaternion.Angle(lastRot, Root.rotation));
        if (Vector3.Dot(lastForward, Root.right) > 0) newRotateVel *= -1f;
        rotateVel = Mathf.Lerp(rotateVel, newRotateVel / fixedDelta, .5f);

        isMoving = Vector.x != 0 || Vector.z != 0;

        lastWorldPos = Root.position;
        lastRot = Root.rotation;
        lastForward = Root.forward;

        fixedDelta = 0f;

    }

    public virtual void UpdateInput()
    {

    }



    public void UpdateInteract()
    {
        if (IsLocal)
        {
            // SPAttention.TogglePanel(reciever.InteractWithOnt != null, reciever.InteractWithOnt);
        }
    }



    protected virtual void UpdateAnimation()
    {

        anim.UpdateAnimation();

    }

    public virtual void Kill()
    {

        alive = false;
        resources.sfx.PlaySound(resources.deathSound, 1f);
        vector = Vector3.zero;

        anim.SetIsDead(true);

    }


    public void RespawnLocal()
    {
        if (IsLocal)
        {
            Respawn(transform.position);
        }
    }

    public virtual void Respawn(Vector3 spawnPos)
    {

        alive = true;

        Teleport(spawnPos);

        if (IsLocal)
        {
            //Debug.Log("Respawning at " + animalType.ToString() + " herd.");

        }
        else
        {

        }

        anim.ToggleDeath(false);

    }




    public override void Teleport(Vector3 newPosition, Quaternion newRotation)
    {

        Controller.Teleport(newPosition);

        base.Teleport(newPosition, newRotation);
    }

    public virtual void ToggleNoClip(bool toggle)
    {
        noclip = toggle;
        Controller.ToggleNoClip(toggle);
    }

    public virtual void ToggleStatic(bool toggle)
    {

        ToggleNoClip(toggle);

        if (IsLocal)
        {
            Controller.ToggleController(!toggle);
        }
    }


}
