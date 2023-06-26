using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPFXComponent : MonoBehaviour
{

    //helps turn on animation, sound, and particles

    Dictionary<string, GameObject> fx = new Dictionary<string, GameObject>();
    public GameObject FX; 

    public SPAnimator animator;

    void Awake() {

        if(animator) {
            animator.OnEffect += SpawnFX;
        }

    }

    void SpawnFX(Object newObject) {
        
        GameObject fxGameobject = newObject as GameObject;

        if(!fxGameobject) {
            Debug.LogError("Can't process non gameobejcts", gameObject);
            return;
        }

        
    }

    void CreateFX(GameObject gameObject) {
        
    }

    void PlayFX(GameObject go) {

    }

    void DestroyFX(Object newObject) {

    }

}
