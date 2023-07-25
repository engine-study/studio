using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public enum PlayerBody { None, LeftHand, RightHand, Head }

[RequireComponent(typeof(Animator))]
public class SPAnimator : MonoBehaviour {


    [Header("Animator")]
    public Transform [] bodyParts;
    public SPIK ik;

    [Header("Debug")]
    public SPAnimationProp prop;
    public Dictionary<string, SPAnimationProp> props;
    public System.Action<Object> OnEffect;
    [SerializeField] Animator animator;
    private RuntimeAnimatorController backupController;

    void Awake() {

        props = new Dictionary<string, SPAnimationProp>();

        animator = GetComponent<Animator>();
        backupController = animator.runtimeAnimatorController;

    }


    public void ToggleProp(bool toggle, SPAnimationProp propPrefab) {

        Debug.Log("Prop: " + toggle.ToString() + " " + propPrefab.gameObject.name.ToString());

        
        if (toggle) {
            
            if(prop != null && prop.gameObject.name != propPrefab.gameObject.name) {
                prop.gameObject.SetActive(false);
                prop = null;
            }

            if (props.ContainsKey(propPrefab.gameObject.name)) {
                
                prop = props[propPrefab.gameObject.name];
                prop.gameObject.SetActive(true);

            } else {
                prop = Instantiate(propPrefab, transform.position, transform.rotation, transform);
                props.Add(propPrefab.gameObject.name, prop);

                if (prop.bodyParent != PlayerBody.None) {
                    prop.bodyProp.parent = bodyParts[(int)prop.bodyParent];
                    prop.bodyProp.localPosition = Vector3.zero;
                    prop.bodyProp.localRotation = Quaternion.identity;
                }
            }
            
        } else {
            prop.gameObject.SetActive(false);
            prop = null;
        }
      
    }

    public void PlayClip(string name) {
        animator.CrossFade(name, .1f);
    }

    public void SetSpeed(float newSpeed) {
        animator.speed = newSpeed;
    }
    public void OverrideController(AnimatorOverrideController overrideController) {
        animator.runtimeAnimatorController = overrideController ? overrideController : backupController;
    }
    public void SpawnObject(object newObject) {

    }

    public void Event() {
        prop?.Fire();
    }

    public void Event(Object newObject) {
        prop?.Fire();
        OnEffect?.Invoke(newObject);
    }

    public void Shake(float shake) {
        // Debug.Log("Animation Shake", gameObject);
        SPCamera.AddShake(shake);
    }
}
