using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLooker : MonoBehaviour
{
    [SerializeField] Vector3 lookVector;
    [SerializeField] Quaternion lookRotation;
    float rotationSpeed = 720f;

    void Awake() {
        lookRotation = Quaternion.identity;
        enabled = false; 
    }
    public void SetLookRotation(Vector3 newLookAt) {


        var _lookY = newLookAt;
        _lookY.y = transform.position.y;

        if(_lookY == transform.position) {
            return;
        }

        Vector3 eulerAngles = Quaternion.LookRotation(_lookY - transform.position).eulerAngles;
        lookVector = (_lookY - transform.position).normalized;
        lookRotation = Quaternion.Euler(eulerAngles.x, (int)Mathf.Round(eulerAngles.y / 90) * 90, eulerAngles.z);

        enabled = true; 
        
    }

    void Update() {

        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);
        if(transform.rotation == lookRotation) {
            enabled = false;
        }
    }

    void OnDrawGizmosSelected() {
        Gizmos.DrawLine(transform.position + Vector3.up, transform.position + lookVector + Vector3.up);
    }
}
