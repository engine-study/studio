using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPMouseRotate : MonoBehaviour
{
    public bool useMomentum = false;
    public bool momentumShift = false;
    public Vector3 Momentum {get{return momentum;}}
    public Vector3 Delta {get{return rot-lastRot;}}
    [SerializeField] float horizontalSpeed = 2.0f;
    [SerializeField] float verticalSpeed = 2.0f;
    [SerializeField] float drag = 1.0f;
    [SerializeField] float shiftAmount = 1.0f;
    [SerializeField] float shiftMagnitude = 1.0f;
    Vector3 rot = new Vector3();
    Vector3 lastRot = new Vector3();
    Vector3 momentum = new Vector3();
    Vector3 startPos;
    void Update() {

        lastRot = rot;

        float h = horizontalSpeed * -Input.GetAxis("Mouse X") * Time.deltaTime;
        float v = verticalSpeed * Input.GetAxis("Mouse Y") * Time.deltaTime;

        rot = Vector3.up * h + Vector3.right * v;

        if(useMomentum) {
            momentum = Vector3.ClampMagnitude(momentum + rot, horizontalSpeed + verticalSpeed);
            momentum = Vector3.MoveTowards(momentum, Vector3.zero, drag * Time.deltaTime);
            transform.Rotate(momentum, Space.World);

            if(momentumShift) {
                Vector3 shift = new Vector3(-h, v, 0f);
                shift = Vector3.ClampMagnitude(shift * shiftAmount, shiftMagnitude);
                transform.localPosition = Vector3.Lerp(transform.localPosition, shift,.1f);
            }

        } else {
            transform.Rotate(rot, Space.World);
        }

    }
}
