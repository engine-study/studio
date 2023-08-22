using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPResourceJuicy : MonoBehaviour
{
    Vector3 start;
    Transform target;
    [SerializeField] Vector2 speed = new Vector2(.5f, .75f);
    [SerializeField] private Vector3 rotation;
    [SerializeField] private AudioClip [] sfx_spawn, sfx_recieve;
    
    public static SPResourceJuicy SpawnResource(string prefabNameInFolder, Transform newTarget, Vector3 spawnPos = default(Vector3), Quaternion rotation = default(Quaternion)) {
        
        SPResourceJuicy res = (Instantiate(Resources.Load(prefabNameInFolder)) as GameObject).GetComponent<SPResourceJuicy>();

        if(res == null) {
            Debug.LogError("Could not find " + prefabNameInFolder);
            return null;
        }

        res.target = newTarget;
        res.transform.position = spawnPos;
        res.transform.rotation = rotation;

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

        float randomHeight = Random.Range(4f, 5f);
        float speedTime = Random.Range(speed.x, speed.y);

        float lerp = 0f;
        while(lerp < 1f) {
            
            transform.position = Vector3.Lerp(start, target.position, lerp) + Vector3.up * randomHeight * Mathf.Sin(lerp * Mathf.PI);
            transform.Rotate(rotation * Time.deltaTime );

            lerp += Time.deltaTime * speedTime;

            yield return null;
        }

        if(sfx_recieve.Length > 0) SPAudioSource.Play(transform.position, sfx_recieve);

        Destroy(gameObject);
    } 
}
