using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHideAfter : MonoBehaviour
{
    public float waitTime = 2f;
    void OnEnable() {
        StartCoroutine(HideAfterCoroutine());
    }
    
    IEnumerator HideAfterCoroutine() {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }
}
