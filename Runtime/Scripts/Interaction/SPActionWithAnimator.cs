using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "ActionAnimator", menuName = "Engine/Action/ActionAnimator", order = 1)]
public class SPActionWithAnimator : SPAction
{
    [Header("Player")]
    public SPAnimatorState animatorState;

    public void SetState(bool toggle, IActor actor, string animation) {

        // Debug.Log("Actor: " + actor.Owner());
        SPAnimator animator = (SPAnimator)actor?.Owner();
        if(animator == null) {Debug.LogError(actor?.Owner()?.name +  ": Not SPAnimator or no Owner", actor?.Owner()); return;}

        ToggleProp(true, animator);
        SetAnimation(animator,animation);
        if(animator.IsHumanoid) {
            if(animatorState) animator.ToggleState(toggle, animatorState);
        }

        if(toggle) {
            animator.IK.SetLook(null);
        } else {

        }
    }

    public override void DoCast(bool toggle, IActor actor) {
        base.DoCast(toggle, actor);
        SetState(toggle, actor, toggle ? "Cast" : "Idle");

    }

    public override void DoAction(bool toggle, IActor actor) {
        base.DoAction(toggle, actor);
        SetState(toggle, actor, toggle ? "Action" : "Idle");
    }

    void ToggleProp(bool toggle, SPAnimator animator) {
        if(animatorState.Prop != null   ) {
            animator.ToggleProp(toggle, animatorState.Prop);
        }
    }
    
    public override void EndAction(IActor actor, ActionEndState reason) {
        base.EndAction(actor, reason);

        DoAction(false, actor);

    }

    protected void SetAnimation(SPAnimator Animator, string state, float fade = 0f) {

        Animator.PlayClip(state, fade);
        // Animator.CrossFade(state, fade);

    }

    protected void ReleaseState(IActor actor, IInteract interactable, string state, float fade = .05f) {
        
    }

}
