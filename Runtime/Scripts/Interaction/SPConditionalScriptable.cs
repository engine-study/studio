using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPConditionalScriptable : ScriptableObject
{

    [Header("Conditional")]
    protected bool cachedBool;

    public virtual bool IsAllowed(IActor actor, IInteract interact) {cachedBool = Evaluate(actor, interact); return cachedBool;}
    protected abstract bool Evaluate(IActor actor, IInteract interact);
    
}
