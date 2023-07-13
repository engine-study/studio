using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHideAfter : MonoBehaviour
{
    public float waitTime = 2f;
    protected SPWindow window;
    
    Coroutine hide;
    bool hasInit;
    void Init() {

        if(window == null) {
            window = GetComponent<SPWindow>();
            if(window) {
                window.OnToggleWindow += Refresh;
            }
        }
        hasInit = true;
    }

    void OnEnable() {
        if(!hasInit) {
            Init();
        }

        hide = StartCoroutine(HideAfterCoroutine());
    }
    
    void OnDisable() {
        hide = null;
    }
    
    void Refresh(bool toggle) {
        if(toggle) {
            if(hide != null) {
                StopCoroutine(hide);
            }

            hide = StartCoroutine(HideAfterCoroutine());
        }
    }

    IEnumerator HideAfterCoroutine() {
        yield return new WaitForSeconds(waitTime);
        gameObject.SetActive(false);
    }
}
