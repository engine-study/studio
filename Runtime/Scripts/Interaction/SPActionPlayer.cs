using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "ActionPlayer", menuName = "Engine/Action/ActionPlayer", order = 1)]
public class SPActionPlayer : SPAction
{
    [Header("Player")]
    public SPAnimatorState animatorState;

    public override void DoCast(bool toggle, IActor actor, IInteract interactable) {
        base.DoCast(toggle, actor, interactable);
        if(toggle) {
            ApplyState(actor, interactable, "Cast");
        } else {
            ApplyState(actor, interactable, "Idle");
        }
    }

    public override void DoAction(bool toggle, IActor actor, IInteract interactable) {
        base.DoAction(toggle, actor, interactable);
        
        if(toggle) {
            ApplyState(actor, interactable, "Action");
        } else {
            ApplyState(actor, interactable, "Idle");
        }

    }
    
    public override void EndAction(IActor actor, IInteract interactable, ActionEndState reason) {
        base.EndAction(actor, interactable, reason);    

        ApplyState(actor, interactable, "Idle");
    }

    
    protected void ApplyState(IActor actor, IInteract interactable, string state, float fade = .1f) {
        SPPlayer player = actor.Player() as SPPlayer;
        AnimationMesh anim = player.Animation as AnimationMesh;

        if(anim) {
            animatorState?.Apply(anim.AnimatorScript);
            // anim.Animator.Play("Action");
            anim.Animator.CrossFade(state, fade);
        }

    }

}
