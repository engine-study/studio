using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPSound : MonoBehaviour
{

    public static SPSound Instance;

    [SerializeField] protected AudioSource audioSource;
    
    void Awake() {
        Instance = this;
    }

    public static void PlaySound(AudioClip clip, float volume = 1f, float pitch = 1f) {
        Instance.audioSource.volume = volume;
        Instance.audioSource.pitch = pitch;
        Instance.audioSource.PlayOneShot(clip);
    }
}
