using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;


public enum PlayerBody { None, LeftHand, RightHand, Head }

[RequireComponent(typeof(Animator))]
public class SPAnimator : MonoBehaviour {

    public System.Action OnStep;
    public SPIK IK {get{return ik;}}
    public Transform Head {get{return head;}}
    
    [Header("Animator")]
    [SerializeField] Transform [] bodyParts;
    [SerializeField] Transform head;
    [SerializeField] SPIK ik;
    [SerializeField] SPAnimationProp defaultPropPrefab;
    [SerializeField] RuntimeAnimatorController defaultController;

    [Header("Debug")]
    [SerializeField] SPAnimationProp prop;
    [SerializeField] SPAnimatorState state;
    [SerializeField] Animator animator;
    [SerializeField] Dictionary<string, SPAnimationProp> props;

    bool hasInit;
    public System.Action<Object> OnEffect;

    void Awake() {
        if(!hasInit) {Init();}
    }

    public void Toggle(bool toggle) {animator.enabled = toggle;}
    void Init() {

        if(hasInit) return;

        props = new Dictionary<string, SPAnimationProp>();
        animator = GetComponent<Animator>();

        hasInit = true;

        if(defaultController) SetController(defaultController);
        else defaultController = animator.runtimeAnimatorController;

        if(defaultPropPrefab) ToggleProp(true, defaultPropPrefab);

        //start the animation of the character at a random time range
        var state = animator.GetCurrentAnimatorStateInfo(0);
        animator.Play(state.fullPathHash, 0, Random.Range(0f,2.5f));

    }

    public void SetDefaultController(RuntimeAnimatorController newController) {

        if(!hasInit) {Init();}

        defaultController = newController; 
        // if(animator.runtimeAnimatorController?.name == defaultController?.name) {SetController(newController);} 
        SetController(defaultController);
    }
    
    public void SetDefaultProp(SPAnimationProp newDefault) {

        if(!hasInit) {Init();}

        if(prop?.Name == defaultPropPrefab?.Name) {ToggleProp(true, newDefault);}
        defaultPropPrefab = newDefault;
    }

    public static void SetToCharacterRenderLayer(GameObject newObject) {SetToLayer(newObject, SPLayers.CharacterLayer);}
    public void SetToAnimatorLayer(GameObject newObject) {SetToLayer(newObject, gameObject.layer);}
    public static void SetToLayer(GameObject newObject, int layer) {
        Renderer [] children = newObject.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < children.Length; i++) {
            children[i].gameObject.layer =  layer;
        }
    }

    public void ToggleState(bool toggle, SPAnimatorState newState) {

        state = newState;
        
        if(toggle) {
            // Debug.Log("Applying", animator);
           OverrideController(newState.overrideController);
           SetSpeed(newState.animationSpeed);
        } else {
            // Debug.Log("Removing", animator);
           OverrideController(null);
           SetSpeed(1f);
        }
    }

    public void ToggleProp(bool toggle, SPAnimationProp propPrefab) {

        // Debug.Log("Prop: " + toggle.ToString() + " " + propPrefab.gameObject.name.ToString());
        if (toggle && propPrefab) {
            
            if(prop != null && prop.Name != propPrefab?.Name) {
                prop.gameObject.SetActive(false);
                prop = null;
            }

            if (props.ContainsKey(propPrefab.Name)) {
                
                prop = props[propPrefab.Name];

            } else {
                prop = Instantiate(propPrefab, transform.position, transform.rotation, transform);
                prop.name = propPrefab.Name; //remove Copy from name
                props.Add(propPrefab.Name, prop);
                
                prop.SetAnimator(this);

                if (prop.bodyParent != PlayerBody.None) {
                    prop.bodyProp.parent = bodyParts[(int)prop.bodyParent];
                    prop.bodyProp.localPosition = Vector3.zero;
                    prop.bodyProp.localRotation = Quaternion.identity;
                }
            }

            prop.gameObject.SetActive(true);
            SetToLayer(prop.gameObject, gameObject.layer);

        } else {

            if(prop != null) {
                prop.gameObject.SetActive(false);
            }

            if(defaultPropPrefab && propPrefab != defaultPropPrefab) {
                ToggleProp(true, defaultPropPrefab);
            } else {
                prop = null;
            }
        }
    }

    public void PlayClip(string name, float crossFade = 0f) {
        if(crossFade == 0f) {animator.Play(name);}
        else {animator.CrossFade(name, crossFade);}
    }

    public void SetSpeed(float newSpeed) {
        animator.speed = newSpeed;
    }

    public void OverrideController(AnimatorOverrideController overrideController) { SetController(overrideController ?? defaultController);}
    public void SetController(RuntimeAnimatorController newController) {animator.runtimeAnimatorController = newController;}
    public void SpawnObject(object newObject) {

    }
    public void Event(string actionName) { prop?.Fire(actionName);}
    public void Action(string actionName) { prop?.Fire(actionName);}
    public void Event() { prop?.Fire("Action");}
    public void Action() { prop?.Fire("Action");}
    public void FootL() { OnStep?.Invoke(); }
    public void FootR() { OnStep?.Invoke(); }

    public void Event(Object newObject) {
        prop?.Fire("Action");
        OnEffect?.Invoke(newObject);
    }

    public void Shake(float shake) {
        // Debug.Log("Animation Shake", gameObject);
        SPCamera.AddShake(shake, transform.position);
    }
}
