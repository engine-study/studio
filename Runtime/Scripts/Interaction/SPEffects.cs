using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPEffects : MonoBehaviour
{
    public bool isEnabled = true;
    public Transform visuals;    
    public AudioClip[] audio;
    float hideAfter = 2.5f;
    Coroutine coroutine;

    public void Play() {
        if (!gameObject.activeInHierarchy) { return; }
        coroutine = StartCoroutine(EffectsCoroutine());
    }
        
    IEnumerator EffectsCoroutine() {

        if(visuals) {
            SPFlashShake flash = gameObject.AddComponent<SPFlashShake>();
            flash.SetTarget(visuals.gameObject);
            flash.Flash();
        }

        SPAudioSource.Play(transform.position, audio);
        foreach(ParticleSystem ps in GetComponentsInChildren<ParticleSystem>()) {ps.Play(true);}

        yield return new WaitForSeconds(hideAfter);
        GameObject.Destroy(gameObject);
    }
}
