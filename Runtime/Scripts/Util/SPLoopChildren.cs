using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLoopChildren : MonoBehaviour
{
    public float waitTime = 2f;
    public float overlapTime = .1f; 
    [SerializeField] private GameObject [] gos;
    void OnEnable() {
        StartCoroutine(LoopCoroutine());
    }

    IEnumerator LoopCoroutine() {

        for(int i = 0; i < gos.Length; i++) {
            gos[i].SetActive(false);
        }

        while(true) {

            for(int i = 0; i < gos.Length; i++) {

                gos[i].SetActive(true);

                yield return new WaitForSeconds(waitTime);

                gos[(i+1) % gos.Length].SetActive(true);

                if(overlapTime > 0f) {
                    yield return new WaitForSeconds(overlapTime);
                }

                gos[i].SetActive(false);

            }

        }
    }
}
