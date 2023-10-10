using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPPlayer : SPBaseActor {
    public bool IsLocalPlayer { get { return isLocalPlayer; } }
    public static SPPlayer LocalPlayer { get { return localPlayer; } }
    public static bool CanInput {get{return LocalPlayer && LocalPlayer.Input();}}
    protected static SPPlayer localPlayer;


    public SPLogic Logic { get { return logic; } }
    public SPController Controller { get { return controller; } }
    public SPAnimation Animation { get { return anim; } }
    public SPAnimator Animator { get { return animator; } }
    public SPPlayerResources Resources { get { return resources; } }

    public bool Alive { get { return alive; } }
    public Vector3 Vector { get { return vector; } }
    public SPVelocity Velocity { get { return anim.Velocity; } }
    protected virtual bool Input() {return LocalPlayer.HasInit;}

    [Header("Player")]
    [SerializeField] protected bool isNPC;

    [SerializeField] private SPLogic logic;
    [SerializeField] private SPController controller;
    [SerializeField] private SPAnimation anim;
    [SerializeField] private SPAnimator animator;
    [SerializeField] private SPPlayerResources resources;

    [Header("Debug")]
    [SerializeField] bool alive = true;
    [SerializeField] bool isLocalPlayer = false;
    Vector3 vector;
    bool isMoving, noclip;

    protected override void Awake() {

        base.Awake();

        if (resources == null) {
            resources = GetComponentInChildren<SPPlayerResources>(true);
        }

        if(anim == null) anim = GetComponentInChildren<SPAnimation>();
        if (anim == null) {
            Debug.LogWarning(gameObject.name + ": No animation system", gameObject);
            anim = gameObject.AddComponent<SPAnimation>();
        }

        if(animator == null) animator = GetComponentInChildren<SPAnimator>(true);
        
        if (logic == null) {
            logic = GetComponent<SPLogic>();
        }
    }

    public static void SetLocalPlayer(SPPlayer newLocal) {

        if (localPlayer != null) {
            Debug.LogError("Multiple local players", localPlayer);
        }

        localPlayer = newLocal;
        newLocal.isLocalPlayer = true;
        SPEvents.OnLocalPlayerSpawn?.Invoke();
    }


    protected override void Destroy() {
        base.Destroy();

    }


    protected override void NetworkInit() {
        base.NetworkInit();

        alive = true;


    }

    protected override void PostInit() {
        base.PostInit();

        controller.Init();
        Controller.ToggleController(IsLocalPlayer);

        Actor.ToggleActor(IsLocalPlayer);

        if (IsLocalPlayer) {
            logic.Init();
        }

        if (IsLocalPlayer) {
            RespawnLocal();
        }

        enabled = IsLocalPlayer;
    }


    protected override void Update() {

        base.Update();

        if (!Alive)
            return;

        if (IsLocalPlayer) {

            UpdateInput();
            UpdateLogic();

        } else {

        }

    }



    protected virtual void UpdateLogic() {
        logic.UpdateInput();
    }

    public virtual void LateUpdate() {

    }

    public virtual void FixedUpdate() {

        if (!Alive)
            return;

        if (IsLocalPlayer) {
            Controller.CallFixedUpdate();
        }

    }

    float fixedDelta = 0f;
    protected float rotateVel;
    protected Vector3 lastWorldPos, lastForward; //velocity of character
    protected Quaternion lastRot;

    protected virtual void UpdateInput() {

    }

    public virtual void Kill() {

        alive = false;
        resources.sfx.PlaySound(resources.deathSound, 1f);
        vector = Vector3.zero;

        anim.SetIsDead(true);

    }


    public void RespawnLocal() {
        if (IsLocalPlayer) {
            Respawn(transform.position);
        }
    }

    public virtual void Respawn(Vector3 spawnPos) {

        alive = true;

        Teleport(spawnPos);

        if (IsLocalPlayer) {
            //Debug.Log("Respawning at " + animalType.ToString() + " herd.");

        } else {

        }

        anim.ToggleDeath(false);

    }




    public override void Teleport(Vector3 newPosition, Quaternion newRotation) {

        Controller.Teleport(newPosition);

        base.Teleport(newPosition, newRotation);
    }

    public virtual void ToggleNoClip(bool toggle) {
        noclip = toggle;
        Controller.ToggleNoClip(toggle);
    }

    public virtual void ToggleStatic(bool toggle) {

        ToggleNoClip(toggle);

        if (IsLocalPlayer) {
            Controller.ToggleController(!toggle);
        }
    }


}
