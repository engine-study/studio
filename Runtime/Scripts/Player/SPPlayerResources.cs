using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SPPlayerResources : MonoBehaviour
{
    public SPPlayer player;

    [Header("Resources")]
    [SerializeField] public SPAudioSource sfx; 
    [SerializeField] public Collider triggerOnline, triggerLocal; 

    [SerializeField] public TextMeshPro nameText; 
    [SerializeField] public ParticleSystem fx_Jump, fx_spawn;
    [SerializeField] public AudioClip spawnSFX;
    [SerializeField] public AudioClip deathSound;
    [SerializeField] public  AudioClip [] jumpSFX;
    [SerializeField] public AudioClip [] stepSFX;

    
    void Awake() {

        player = GetComponentInParent<SPPlayer>();
        player.OnPostInit += Init;
        triggerLocal.gameObject.SetActive(false);

    }

    void OnDestroy() {
        player.OnPostInit -= Init;
    }

    void Init() {

        triggerLocal.gameObject.SetActive(player.IsLocalPlayer);
        
        if(nameText) {
            nameText.gameObject.SetActive(false);
        }
    }
  

}
