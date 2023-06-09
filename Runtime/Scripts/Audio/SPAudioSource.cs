using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAudioSource : MonoBehaviour
{
  
    
    [SerializeField] protected AudioSource audioSource;

    public void PlaySound(AudioClip [] clips, float volume = 1f, bool pitchShift = true) {

        PlaySound(clips[Random.Range(0,clips.Length)], volume, pitchShift);
    }


    public void PlaySound(AudioClip clip, float volume = 1f, bool pitchShift = true) {
        if(clip == null) {
            return;
        }
        
        audioSource.pitch = pitchShift ? Random.Range(.95f,1.05f) : 1f;
        audioSource.PlayOneShot(clip, volume);
    }

}
