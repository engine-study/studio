using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLoading : MonoBehaviour
{
    public Transform warningLoading;

    void OnEnable() {
        warningLoading.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(0f,360f));
    }

    void Update() {
        warningLoading.transform.Rotate(Vector3.forward * -360f * Time.deltaTime);
        
    }
}
