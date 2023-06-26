using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SPConditionalScriptable : ScriptableObject
{

    [Header("Conditional")]
    protected bool cachedBool;

    public virtual bool IsAllowed() {cachedBool = Evaluate(); return cachedBool;}
    protected abstract bool Evaluate();
    
}
