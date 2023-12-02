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
    public Renderer [] renderers;
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

            activeBody = newBody;

            if(activeBody == null) {return;}

            mr = activeBody.GetComponent<MeshRenderer>();
            targetMR.GetComponent<MeshFilter>().sharedMesh = mr.GetComponent<MeshFilter>().sharedMesh;

        } else if(bodyType == PlayerBody.Body) {

            activeBody = newBody;
            if(activeBody == null) {return;}

            targetSMR.sharedMesh = newBody.GetComponent<MeshFilter>()?.sharedMesh;

        } else if(bodyType == PlayerBody.Material) {
            
            activeBody = newBody;

            if(activeBody == null) { return; }

            Material mat = activeBody.GetComponent<Renderer>().sharedMaterial;

            //replace all materials with flash material
            for (int i = 0; i < renderers.Length; i++) {
                Material[] sharedMaterialsCopy = renderers[i].sharedMaterials;
                for (int j = 0; j < sharedMaterialsCopy.Length; j++) { sharedMaterialsCopy[j] = mat; }
                renderers[i].sharedMaterials = sharedMaterialsCopy;
            }

        } else if(bodyType == PlayerBody.Effect) {

            if(activeBody != null) {
                Destroy(activeBody);
            }

            activeBody = newBody;
            if(activeBody == null) { return; }

            activeBody = Instantiate(newBody, bodyParent.transform);
        }


        if(activeBody == null) {
            return;
        }

        SPAnimator.SetToCharacterRenderLayer(activeBody);
        OnUpdated?.Invoke();
        
    }
}
