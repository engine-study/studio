using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public enum PlayerBody { None, LeftHand, RightHand, Head }

[RequireComponent(typeof(Animator))]
public class SPAnimator : MonoBehaviour {


    public SPIK IK {get{return ik;}}

    [Header("Animator")]
    public Transform [] bodyParts;
    public SPIK ik;
    public SPAnimationProp defaultPropPrefab;

    [Header("Debug")]
    public SPAnimationProp prop;
    public Dictionary<string, SPAnimationProp> props;
    public System.Action<Object> OnEffect;
    [SerializeField] Animator animator;
    private RuntimeAnimatorController backupController;

    void Awake() {

        props = new Dictionary<string, SPAnimationProp>();

        animator = GetComponent<Animator>();
        var state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, Random.Range(0f,1f));

        backupController = animator.runtimeAnimatorController;

        if(defaultPropPrefab)
            ToggleProp(true, defaultPropPrefab);

    }

    public static void SetToPlayerLayer(GameObject newObject) {SetToLayer(newObject, SPLayers.PlayerLayer);}
    public static void SetToLayer(GameObject newObject, int layer) {
        Renderer [] children = newObject.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < children.Length; i++) {
            children[i].gameObject.layer =  layer;
        }
    }


    public void ToggleProp(bool toggle, SPAnimationProp propPrefab) {

        // Debug.Log("Prop: " + toggle.ToString() + " " + propPrefab.gameObject.name.ToString());
        if (toggle) {
            
            if(prop != null && prop.gameObject.name != propPrefab.gameObject.name) {
                prop.gameObject.SetActive(false);
                prop = null;
            }

            if (props.ContainsKey(propPrefab.gameObject.name)) {
                
                prop = props[propPrefab.gameObject.name];

            } else {
                prop = Instantiate(propPrefab, transform.position, transform.rotation, transform);
                props.Add(propPrefab.gameObject.name, prop);

                if (prop.bodyParent != PlayerBody.None) {
                    prop.bodyProp.parent = bodyParts[(int)prop.bodyParent];
                    prop.bodyProp.localPosition = Vector3.zero;
                    prop.bodyProp.localRotation = Quaternion.identity;
                }
            }

            prop.gameObject.SetActive(true);
            SetToLayer(prop.gameObject, gameObject.layer);

        } else {

            if(prop != null)
                prop.gameObject.SetActive(false);

            if(defaultPropPrefab && propPrefab != defaultPropPrefab) {
                ToggleProp(true, defaultPropPrefab);
            } else {
                prop = null;
            }

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
        SPCamera.AddShake(shake, transform.position);
    }
}
