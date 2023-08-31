using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores two curves: one representing the x value along a normalized curve, and the other representing the y value.
//Get(float) returns a Vector2 consisting of the output of two curves
[CreateAssetMenu(fileName = "Lerp", menuName = "Engine/Animation/Lerp", order = 1)]
public class SPLerpCurve : ScriptableObject
{
    public AnimationCurve curve;
 
    public float Evaluate(float t) {
        if (t < 0) { return curve.Evaluate(0); }
        if (t > 1) { return curve.Evaluate(1); }
        return curve.Evaluate(t);
    }
}