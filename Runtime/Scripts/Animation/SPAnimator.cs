using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

[RequireComponent(typeof(Animator))]
public class SPAnimator : MonoBehaviour
{

    public List<SPAnimationEvent> animationEvents;
    public System.Action<Object> OnEffect;
    [SerializeField] Animator animator;
    private RuntimeAnimatorController backupController;

    void Awake() {

        animator = GetComponent<Animator>();
        backupController = animator.runtimeAnimatorController;

    }

    public void PlayClip(string name) {
        animator.CrossFade(name, .1f);
    }

    public void ToggleAnimationEvent(bool toggle, SPAnimationEvent animationEvent) {
        if(toggle) {
            animationEvents.Add(animationEvent);
        } else {    
            animationEvents.Remove(animationEvent);
        }
    }

    public void SetSpeed(float newSpeed) {
        animator.speed = newSpeed;
    }
    public void OverrideController(AnimatorOverrideController overrideController) {
        animator.runtimeAnimatorController = overrideController ? overrideController : backupController;
    }
    public void SpawnObject(object newObject) {

    }

    public void Event(Object newObject) {
        
        for(int i = 0; i < animationEvents.Count; i++) {
            animationEvents[i].Fire();
        }

        OnEffect?.Invoke(newObject);
    }

    public void Shake(float shake) {
        // Debug.Log("Animation Shake", gameObject);
        SPCamera.AddShake(shake);
    }
}
