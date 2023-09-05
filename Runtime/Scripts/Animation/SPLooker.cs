using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLooker : MonoBehaviour
{
    Vector3 lookVector;
    Quaternion lookRotation;
    Quaternion lastRot;
    float rotationSpeed = 720f;

    void Awake() {
        lastRot = Quaternion.identity;
        lookRotation = Quaternion.identity;
        enabled = false; 
    }
    public void SetLookRotation(Vector3 newLookAt) {

        lastRot = lookRotation;

        var _lookY = newLookAt;
        _lookY.y = transform.position.y;

        if(_lookY == transform.position) {
            return;
        }

        Vector3 eulerAngles = Quaternion.LookRotation(_lookY - transform.position).eulerAngles;
        lookVector = (_lookY - transform.position).normalized;
        lookRotation = Quaternion.Euler(eulerAngles.x, (int)Mathf.Round(eulerAngles.y / 90) * 90, eulerAngles.z);

        if(lookRotation != lastRot) {
            enabled = true; 
        }
    }

    void Update() {

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        if(transform.rotation == lookRotation) {
            enabled = false;
        }
    }
}
