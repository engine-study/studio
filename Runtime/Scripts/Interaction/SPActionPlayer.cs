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
        ToggleCastState(toggle, actor, interactable);

    }

    public override void DoAction(bool toggle, IActor actor, IInteract interactable) {
        base.DoAction(toggle, actor, interactable);
        ToggleActionState(toggle, actor, interactable);
    }

    
    public void ToggleCastState(bool toggle, IActor actor, IInteract interactable) {
        SPAnimator anim = (actor.Owner() as SPPlayer).Animator as SPAnimator;

        ToggleProp(toggle, anim);

        if(toggle) {
            anim.IK.SetLook(null);
            FadeAnimation(actor, interactable, "Cast");
        } else {
            FadeAnimation(actor, interactable, "Idle");
        }
    }

    public void ToggleActionState(bool toggle, IActor actor, IInteract interactable) {
        SPAnimator anim = (actor.Owner() as SPPlayer).Animator as SPAnimator;

        ToggleProp(toggle, anim);

        if(toggle) {
            anim.IK.SetLook(null);
            FadeAnimation(actor, interactable, "Action");
        } else {
            FadeAnimation(actor, interactable, "Idle");
        }
    }

    void ToggleProp(bool toggle, SPAnimator animator) {
        if(animatorState.Prop != null) {
            animator.ToggleProp(toggle, animatorState.Prop);
        }
    }
    
    public override void EndAction(IActor actor, IInteract interactable, ActionEndState reason) {
        base.EndAction(actor, interactable, reason);

        ToggleActionState(false, actor, interactable);

    }

    protected void FadeAnimation(IActor actor, IInteract interactable, string state, float fade = .1f) {
        SPPlayer player = actor.Owner() as SPPlayer;
        AnimationMesh anim = player.Animation as AnimationMesh;

        if(anim) {
            animatorState?.Apply(anim.AnimatorScript);
            // anim.Animator.Play("Action");
            anim.Animator.CrossFade(state, fade);
        }
    }

    protected void ReleaseState(IActor actor, IInteract interactable, string state, float fade = .1f) {
        
    }

}
