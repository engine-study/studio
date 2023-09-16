using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPRotate : MonoBehaviour
{
    public Vector3 rotateSpeed;
    public Space space;
    void Update() {
        transform.Rotate(rotateSpeed * Time.deltaTime, space);
    }
}
