using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPResourceJuicy : MonoBehaviour
{
    Vector3 start;
    Transform target;
    [SerializeField] private Vector3 rotation;
    [SerializeField] private AudioClip [] sfx_spawn, sfx_recieve;
    
    public static SPResourceJuicy GiveResource(string prefabNameInFolder, Transform target, Vector3 spawnPos = default(Vector3), Quaternion rotation = default(Quaternion)) {
        
        SPResourceJuicy res = (Instantiate(Resources.Load(prefabNameInFolder)) as GameObject).GetComponent<SPResourceJuicy>();

        if(res == null) {
            Debug.LogError("Could not find " + prefabNameInFolder);
            return null;
        }

        res.transform.position = spawnPos;
        res.transform.rotation = rotation;

        res.GiveResource(target);

        return res;

    }

    public void GiveResource(Transform newTarget) {
        target = newTarget;
        StartCoroutine(GiveAnimation());
    }

    IEnumerator GiveAnimation() {

        start = transform.position;
        
        SPAudioSource.Play(transform.position, sfx_spawn);

        float randomHeight = Random.Range(4f, 5f);

        float lerp = 0f;
        while(lerp < 1f) {
            
            transform.position = Vector3.Lerp(start, target.position, lerp) + Vector3.up * randomHeight * Mathf.Sin(lerp * Mathf.PI);
            transform.Rotate(rotation * Time.deltaTime );

            lerp += Time.deltaTime * .75f;

            yield return null;
        }

        SPAudioSource.Play(transform.position, sfx_recieve);

        Destroy(gameObject);
    } 
}
