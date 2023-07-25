using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPAudioSource : MonoBehaviour {

    public AudioSource Source { get { return audioSource; } }
    [SerializeField] protected AudioSource audioSource;

    public static void Play(Vector3 position, AudioClip[] clips, float volume = 1f, bool pitchShift = true) {
        SPAudioMother.GetSource(position).PlaySound(clips, volume, pitchShift);
    }

    public void PlaySound(AudioClip[] clips, float volume = 1f, bool pitchShift = true) {
        PlaySound(clips[Random.Range(0, clips.Length)], volume, pitchShift);
    }


    public void PlaySound(AudioClip clip, float volume = 1f, bool pitchShift = true) {
        if (clip == null) {
            return;
        }

        audioSource.pitch = pitchShift ? Random.Range(.95f, 1.05f) : 1f;
        audioSource.PlayOneShot(clip, volume);
    }

}
