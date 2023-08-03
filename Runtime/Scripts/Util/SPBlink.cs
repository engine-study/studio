using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPBlink : MonoBehaviour
{
    public float waitTime = .5f;
    public int count = -1;
    public GameObject target;

    int counter;
    void OnEnable() {
        StartCoroutine(BlinkCoroutine());
    }

    IEnumerator BlinkCoroutine() {

        counter = count;

        while(count == -1 || counter > 0) {
            target.SetActive(true);
            yield return new WaitForSeconds(waitTime);
            target.SetActive(false);
            yield return new WaitForSeconds(waitTime);
            counter--;
        }
    }
}
