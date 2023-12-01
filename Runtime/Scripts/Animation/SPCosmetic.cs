using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCosmetic : MonoBehaviour
{
    public System.Action OnUpdated;

    [Header("Settings")]
    public PlayerBody bodyType;
    public GameObject defaultBody;
    public Transform bodyParent;
    public GameObject activeBody;
    public SPAnimator animator;
    public MeshRenderer targetMR;
    public SkinnedMeshRenderer targetSMR;

    [Header("Debug")]

    MeshRenderer mr;
    SkinnedMeshRenderer smr;
    
    void Awake() {
        animator = GetComponentInParent<SPAnimator>(true);
        SetCosmetic(defaultBody);
    }
    public void SetCosmetic(GameObject newBody) {

        if(bodyType == PlayerBody.Head) {

            if(activeBody) { activeBody.SetActive(false);}

            activeBody = newBody;

            if(activeBody == null) {return;}

            mr = activeBody.GetComponent<MeshRenderer>();
            activeBody.SetActive(true);
            activeBody.transform.parent = bodyParent;
            activeBody.transform.localPosition = Vector3.zero;
            activeBody.transform.localRotation = Quaternion.identity;

        } else if(bodyType == PlayerBody.Body) {

            activeBody = newBody;
            if(activeBody == null) {return;}
            targetSMR.sharedMesh = newBody.GetComponent<MeshFilter>()?.sharedMesh;

        } else if(bodyType == PlayerBody.Material) {
            
            if(activeBody) { activeBody.SetActive(false);}

            activeBody = newBody;

            if(activeBody == null) { return; }

        } else {
           
        }


        if(activeBody == null) {
            return;
        }

        SPAnimator.SetToCharacterRenderLayer(activeBody);
        OnUpdated?.Invoke();
        
    }
}
