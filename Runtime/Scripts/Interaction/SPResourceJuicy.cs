using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPResourceJuicy : MonoBehaviour
{
    Vector3 start;
    Transform target;
    [SerializeField] float time = 2f;
    [SerializeField] public Vector3 offset = Vector3.zero;
    [SerializeField] public Vector3 rotation;
    [SerializeField] public AudioClip [] sfx_spawn, sfx_recieve;
    
    public static SPResourceJuicy SpawnResource(string prefabNameInFolder, Transform newTarget, Vector3 spawnPos = default(Vector3), Quaternion spawnRot = default(Quaternion)) {
        
        SPResourceJuicy res = (Instantiate(Resources.Load(prefabNameInFolder)) as GameObject).GetComponent<SPResourceJuicy>();

        if(res == null) {
            Debug.LogError("Could not find " + prefabNameInFolder);
            return null;
        }

        res.target = newTarget;
        res.transform.position = spawnPos;
        res.transform.rotation = spawnRot;

        return res;

    }

    public void SendResource() {
        SendResource(target);
    }
    public void SendResource(Transform newTarget) {
        target = newTarget;
        StartCoroutine(GiveAnimation());
    }

    IEnumerator GiveAnimation() {

        start = transform.position;
        if(sfx_spawn.Length > 0) SPAudioSource.Play(transform.position, sfx_spawn);

        float randomHeight = Random.Range(.5f, .5f);
        float count = 0f;
        float lerp = 0f;

        while(lerp < 1f) {
            
            transform.position = Vector3.Lerp(start + offset, target.position + offset, lerp); //+ Vector3.up * randomHeight * lerp
            transform.Rotate(rotation * Time.deltaTime );

            count += Time.deltaTime;
            lerp = Mathf.Clamp01(count/time);

            yield return null;
        }

        if(sfx_recieve.Length > 0) SPAudioSource.Play(transform.position, sfx_recieve);

        Destroy(gameObject);
    } 
}
