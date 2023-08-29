using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBaseActor : SPBase
{
    public SPActor Actor { get { return actor; } }
    public SPReciever Reciever { get { return reciever; } }
    public System.Action<IAction> OnPlayerAction;

    [Header("Actor")]
    private SPActor actor;
    private SPReciever reciever;

    protected override void Awake()
    {

        base.Awake();

        if (reciever == null) {reciever = gameObject.GetComponentInChildren<SPReciever>(); }

        if (actor == null) { actor = gameObject.GetComponent<SPActor>(); }
        actor.ToggleActor(false);
    
    }

    public void SetReciever(SPReciever r) {
        
        actor.ToggleReciever(false, reciever);
        reciever = r;
        actor.ToggleReciever(true, reciever);

    }

    protected override void PostInit() {
        base.PostInit();

        Actor.Init();
        actor.OnAction += OnPlayerAction;

    }

    protected override void Destroy()
    {
        base.Destroy();

        actor.OnAction -= OnPlayerAction;

    }

}
