using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBlink : MonoBehaviour
{
    public float waitTime = .5f;
    public GameObject target;
    void OnEnable() {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine() {

        while(true) {
            target.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            target.SetActive(false);
            yield return new WaitForSeconds(waitTime);
        }
    }
}
