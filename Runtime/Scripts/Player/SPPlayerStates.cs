using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class CarryingState : SPState
{
    private IInteract carriedObject;

    public CarryingState(IInteract obj)
    {
        carriedObject = obj;
    }

    public override void Enter(SPPlayer actor)
    {
        base.Enter(actor);
        // Perform any setup for the Carrying state
        // Attach carriedObject to the Actor (e.g., as a child object)
    }

    public override void Update(SPPlayer actor)
    {
        base.Update(actor);
        // Handle state transitions
        if (Input.GetKeyDown(KeyCode.T))
        {
            actor.SetState(new ThrowingState(carriedObject));
        }
    }

    public override void Exit(SPPlayer actor)
    {
        base.Exit(actor);
        // Perform any cleanup for the Carrying state
    }
}

public class ThrowingState : SPState
{
    private IInteract thrownObject;

    public ThrowingState(IInteract obj)
    {
        thrownObject = obj;
    }

    public override void Enter(SPPlayer actor)
    {
        base.Enter(actor);
        // Perform any setup for the Throwing state
        // Detach thrownObject from the Actor
        // Apply force to the thrownObject
    }

    public override void Update(SPPlayer actor)
    {
        base.Update(actor);
        // Handle state transitions (return to IdleState or other appropriate state)
        actor.SetState(new IdleState());
    }

    public override void Exit(SPPlayer actor)
    {
        base.Exit(actor);
        // Perform any cleanup for the Throwing state
    }
}


public class IdleState : SPState
{
    public override void Enter(SPPlayer actor)
    {
        base.Enter(actor);
        // Perform any setup for the Idle state
    }

    public override void Update(SPPlayer actor)
    {
        base.Update(actor);

    }

    public override void Exit(SPPlayer actor)
    {
        base.Exit(actor);
        // Perform any cleanup for the Idle state
    }
}
*/