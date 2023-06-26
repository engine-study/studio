using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCollisionAudio : MonoBehaviour
{
    [Header("Collision")]   
    public AudioSource audioSource;
    public Rigidbody rb;
    public AudioClip [] hits;
    float lastCollision;
    float minTime = .1f; 
    void Start() {
        if(rb == null) {
            rb = GetComponent<Rigidbody>();
        }

        if(audioSource == null) {
            audioSource = GetComponent<AudioSource>();
        }
    }

    protected void OnCollisionEnter(Collision collision) {
        if(rb.velocity.magnitude > .25f) {
            PlayCollision();
        }
    }

    public void PlayCollision() {
        if(Time.time - lastCollision < minTime) {
            return;
        }

        lastCollision = Time.time;
        audioSource.PlayOneShot(hits[Random.Range(0, hits.Length)]);

    }
}
