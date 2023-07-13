using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPIK : MonoBehaviour
{
    [Header("IK")]
    public Transform head; 
    protected Animator animator;

    [Header("Debug")]
    public bool ikActive = false;
    public Transform lookObj = null;
    // public Transform rightHandObj = null;
    float blendHead, blendHeadTarget;
    void Start ()
    {
        animator = GetComponent<Animator>();

        //lookObj can never be null
        lookObj = transform;
    }

    public void SetLook(Transform newLook) {

        ikActive = newLook != null;

        if(newLook) {
            lookObj = newLook;
            blendHeadTarget = 1f;
        } else {
            blendHeadTarget = 0f;
        }
    }

    void Update() {
        blendHead = Mathf.MoveTowards(blendHead, blendHeadTarget, Time.deltaTime * 2f);
    }

    //a callback for calculating IK
    void OnAnimatorIK() {

        animator.SetLookAtPosition(lookObj.position);
        animator.SetLookAtWeight(blendHead);
        
    }    
}
