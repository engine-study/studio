using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "AnimatorState", menuName = "Engine/Animation/AnimatorState", order = 1)]
public class SPAnimatorState : ScriptableObject
{
    public SPAnimationProp Prop {get{return prop;}}

    [SerializeField] protected float animationSpeed = 1f;
    [SerializeField] protected AnimatorOverrideController overrideController;
    [SerializeField] protected SPAnimationProp prop;

    public void Apply(SPAnimator animator) {

        // Debug.Log("Applying", animator);

        animator.OverrideController(overrideController);
        animator.SetSpeed(animationSpeed);
    }

    public void Remove(SPAnimator animator) {

        // Debug.Log("Removing", animator);
        animator.OverrideController(null);
        animator.SetSpeed(1f);

    }

}
