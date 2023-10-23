using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPFlashShake : MonoBehaviour {
    public static Material FlashMat;

    [Header("Settings")]
    [SerializeField] private GameObject target;

    [Header("Debug")]
    [SerializeField] private GameObject clone;
    [SerializeField] private MeshRenderer[] renderers;
    // [SerializeField] private MeshFilter mesh;
    // [SerializeField] private MeshRenderer mr;


    Coroutine flash;

    public void SetTarget(GameObject newTarget) {

        if (newTarget == target) {
            return;
        }

        if (flash != null) {
            StopCoroutine(flash);
        }

        if (clone != null) {
            GameObject.Destroy(clone);
            clone = null;
        }

        target = newTarget;

    }

    void OnDisable() {
        if (clone != null) {
            clone.SetActive(false);
        }
    }

    public void Flash() {

        if (FlashMat == null) { FlashMat = Resources.Load("FlashDither") as Material;}
        if (!gameObject.activeInHierarchy) { return; }
        if (clone == null) { SpawnClone();}

        if (flash != null) { StopCoroutine(flash);}
        flash = StartCoroutine(FlashCoroutine());

    }

    void SpawnClone() {
        if (target == null) {
            // Debug.LogError("No target", this);
            return;
        }

        clone = Instantiate(target, target.transform.position, target.transform.rotation);
        clone.transform.parent = transform;
        clone.SetActive(true);

        renderers = clone.GetComponentsInChildren<MeshRenderer>();

        if (renderers.Length == 0) { Debug.Log("No renderers", this);}

        //replace all materials with flash material
        for (int i = 0; i < renderers.Length; i++) {
            Material[] sharedMaterialsCopy = renderers[i].sharedMaterials;
            for (int j = 0; j < sharedMaterialsCopy.Length; j++) { sharedMaterialsCopy[j] = FlashMat; }
            renderers[i].sharedMaterials = sharedMaterialsCopy;
        }

    }

    IEnumerator FlashCoroutine() {
        int loop = 3;
        while (loop > 0) {

            clone.SetActive(true);
            yield return new WaitForSeconds(.1f);
            clone.SetActive(false);
            loop--;
            yield return new WaitForSeconds(.1f);
        }
    }

}
