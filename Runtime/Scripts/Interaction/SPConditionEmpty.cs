using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionEmpty", menuName = "Engine/Action/ConditionEmpty", order = 1)]
public class SPConditionEmpty : SPConditionalScriptable
{
   
    protected override bool Evaluate(IActor actor, IInteract interact) {
        // if(interact.GameObject().transaction.position)
        return false;
    }

}
