using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLine : MonoBehaviour
{
    [Header("Line")]
    public LineRenderer Line;

    [Header("Debug")]
    [SerializeField] Transform start;
    [SerializeField] Transform target;
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 targetPos;
    bool targets = false;
    public void Toggle(bool toggle) {
        enabled = toggle;
        Line.enabled = toggle;
    }

    public void SetTarget(Transform from, Transform to) {
        start = from;
        target = to;

        if(!from || !to) {Debug.LogError("Null from or to", this);}
        targets = from && to;
    }

    public void SetTarget(Vector3 from, Vector3 to) {
        startPos = from;
        targetPos = to;
        targets = false;
    }

    void LateUpdate() {

        Vector3 from;
        Vector3 to;

        if(targets) {
            from = start.position;
            to = target.position;
        } else {
            from = startPos;
            to = targetPos;
        }

        Vector3[] positions = new Vector3[] { from + Vector3.up * .25f, to + Vector3.up * .25f, to };
        Line.SetPositions(positions);
    }
}
