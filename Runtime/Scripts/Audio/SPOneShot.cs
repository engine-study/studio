using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPOneShot : MonoBehaviour
{
    public AudioClip [] clips;
    SPAudioSource source;
    void OnEnable()
    {
        if(source == null) {
            source = gameObject.GetComponent<SPAudioSource>();
        }

        source.PlaySound(clips);
    }

}
