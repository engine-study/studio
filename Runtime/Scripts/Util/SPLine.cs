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

    public void Toggle(bool toggle) {
        enabled = toggle;
        Line.enabled = toggle;
    }

    public void SetTarget(Transform from, Transform to) {
        start = from;
        target = to;
    }

    void LateUpdate() {
        Vector3[] positions = new Vector3[] { start.position + Vector3.up * .1f, target.position + Vector3.up * .1f, target.position };
        Line.SetPositions(positions);
    }
}
