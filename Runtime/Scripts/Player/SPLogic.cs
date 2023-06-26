using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLogic : MonoBehaviour
{
 

    public Vector3 InputVector {get{return input;}}

    [Header("Logic")]
    [SerializeField] protected SPPlayer player;
    [SerializeField] protected Vector3 input;
    public bool crouchFlag = false;
    public bool jumpFlag, hoverFlag;

    [SerializeField] protected Vector3 inputRaw;

    public virtual void Init() {
        player = GetComponent<SPPlayer>();
    }

    public virtual void UpdateInput() {

    }

    void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + Vector3.up * .1f, transform.position + input * .1f + Vector3.up * .1f);
    }
}
