using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPVelocityFace : MonoBehaviour
{
    Transform Transform {get { return transform; } }
    public float moveSpeed;
    Quaternion lookRotation;
    Vector3 lookVector;
    Vector3 moveDest;
    float rotationSpeed = 720f;

    //this lerps the character transform
    public void UpdatePosition() {

        //UPDATE ROTATION
            var _lookY = moveDest;
            _lookY.y = Transform.position.y;

            if (_lookY != Transform.position) {
                lookRotation = Quaternion.LookRotation(_lookY - Transform.position);
                lookVector = (_lookY - Transform.position).normalized;
         
        }

        //ROTATE
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, rotationSpeed * Time.deltaTime);

        if(Transform.position == moveDest) {
            return;
        }

        //MOVE
        Vector3 newPosition = Vector3.MoveTowards(Transform.position, moveDest, moveSpeed * Time.deltaTime);
        Transform.position = newPosition;

    }
}
