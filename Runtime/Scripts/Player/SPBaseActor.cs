using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBaseActor : SPBase
{
    public SPActor Actor { get { return actor; } }
    public System.Action<IAction> OnPlayerAction;

    [Header("Actor")]
    public SPActor actor;
    public SPInteractReciever reciever;

    protected override void Awake()
    {

        base.Awake();

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


    }

    
    protected override void Destroy()
    {
        base.Destroy();

        reciever.OnInteractUpdate -= UpdateInteract;
        actor.OnAction -= OnPlayerAction;

    }

    protected virtual void UpdateInteract() {

    }

}
