using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[CreateAssetMenu(fileName = "AnimatorState", menuName = "Engine/Animation/AnimatorState", order = 1)]
public class SPAnimatorState : ScriptableObject
{
    public SPAnimationProp Prop {get{return prop;}}

    [SerializeField] public float animationSpeed = 1f;
    [SerializeField] public AnimatorOverrideController overrideController;
    [SerializeField] public SPAnimationProp prop;

}
