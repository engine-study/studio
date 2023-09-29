using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCamera : MonoBehaviour
{

    public static SPCamera I;
    public static Camera Camera {get{return I.camera;}}
    public static float Shake {get{return I.shake;}}
    public static Transform Follow {get { return I.followTransform; } }
    public float FOV {get{return fov;}}

    [Header("Camera")]
    [SerializeField] new Camera camera;
    [SerializeField] Transform x, y, z;
    [SerializeField] AudioListener listener;

    [Header("Settings")]
    [SerializeField] float moveSpeed = 5f; 
    [SerializeField] float scrollSensitivity = 1f;
    [SerializeField] float rotateSensitivity = 360f;
    [SerializeField] float rotateRound = 45f;
    [SerializeField] float rotateSpeed = 90f;
    [SerializeField] float minFOV = 5f, maxFOV = 25f;
    [SerializeField] float shakeFalloff = 10f;
    float fovMultiple = 1f;
    float fovLerp = 15f;

    [Header("Screenshot")]
    [SerializeField] int screenshotSize = 1;
    int screenshotCount = 0;

    [Header("Debug")]
    [SerializeField] float fov = 15f;
    [SerializeField] bool follow = true; 
    [SerializeField] bool canScroll = true; 
    [SerializeField] bool inCinematic = false; 
    [SerializeField] Transform followTransform;
    [SerializeField] Vector3 pos;
    [SerializeField] Quaternion rot;
    [SerializeField] float shake = 0f;
    [SerializeField] float scrollRot, scrollLock;

    public static void ToggleCamera(bool toggle) {
        I.gameObject.SetActive(toggle);
    }

    public static void Teleport(Vector3 targetPos) { Teleport(targetPos, I.rot);}
    public static void Teleport(Vector3 targetPos, Quaternion targetRot) {
        Debug.Log("Camera Teleport: " + targetPos.ToString());
        I.transform.position = targetPos;
        I.rot = targetRot;
    }

    public static void SetFollow(Transform newFollow) {
        I.follow = newFollow != null;
        I.followTransform = newFollow;
    }
    public static void SetTarget(Vector3 targetPos) {SetTarget(targetPos, I.rot);}
    public static void SetTarget(Vector3 targetPos, Quaternion targetRot) {
        // Debug.Log("Camera Target: " + targetPos.ToString());
        I.pos = targetPos;
        I.rot = targetRot;
    }

    void Awake() {

        Debug.Log("Camera Awake");

        I = this; 
        fov = camera.orthographic ? camera.orthographicSize : camera.fieldOfView;
        fovLerp = fov;
        
        pos = transform.position;
        rot = transform.rotation;

        transform.localPosition = Vector3.zero;

    }
  
    public void Update() {

        UpdateInput();

        UpdateCamera();

        UpdateShake();
     
    }


    public static void SetFOVGlobal(float newFOV, bool instant = false) {

        #if UNITY_EDITOR
        if(!Application.isPlaying) {
            I = FindObjectOfType<SPCamera>();
            I.camera.fieldOfView = newFOV;
            I.camera.orthographicSize = newFOV;
        }
        #endif

        I.SetFOV(newFOV, instant);
      
    }

    public void SetFOV(float newFOV, bool instant = false) {
        // Debug.Log("Camera FOV: " + newFOV);
        fov = Mathf.Clamp(newFOV, minFOV, maxFOV);
        if(instant) {
            fovLerp = fov;
            if(camera.orthographic) {camera.orthographicSize = fov;}
            else {camera.fieldOfView = fov;}
            
        }
    }


    void UpdateInput() {

        if(SPUIBase.CanInput) {
            if(Input.GetKeyDown(KeyCode.Minus)) {
                SetFOVGlobal(1f);
            } else if(Input.GetKeyDown(KeyCode.Equals)) {
                SetFOVGlobal(-1f); 
            }
                
            if(Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.LeftControl)) && Application.isEditor) {
                screenshotCount++;
                ScreenCapture.CaptureScreenshot("../Screenshots/" + screenshotCount + ".png", screenshotSize);
            }
            
        }    
        

        if(SPUIBase.IsMouseOnScreen && canScroll) {

            if(Input.GetKey(KeyCode.LeftControl)) {
                
                scrollRot += Input.mouseScrollDelta.y * rotateSensitivity * Time.deltaTime;
                scrollLock = Mathf.Round(scrollRot / rotateRound) * rotateRound;
                // scrollLock = Mathf.Round(scrollRot);

                // rotation = rotation * Quaternion.Euler(Vector3.up * Input.mouseScrollDelta.y * 25f);
                // transform.Rotate(0f,Input.mouseScrollDelta.y * 25f,0f);

            } else if(Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.LeftAlt)) {

                SetFOVGlobal(fov + Input.mouseScrollDelta.y * -scrollSensitivity);

            }
        }
        
    }

    void UpdateCamera() {

        Vector3 newPos = Vector3.zero;
        //POSITION
        if(follow && followTransform) {
            newPos = followTransform.position;
            listener.transform.position = newPos;
        } else {
            newPos = pos;
            listener.transform.position = newPos;
        }

        float distanceClamped = Mathf.Max(.25f,Vector3.Distance(transform.position,newPos));

        transform.position = Vector3.MoveTowards(transform.position, newPos, distanceClamped * 2f * Time.deltaTime * moveSpeed);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, rot * Quaternion.Euler(Vector3.up * scrollLock), rotateSpeed * Time.deltaTime);

        //FOV
        fov = Mathf.Clamp(fov, minFOV, maxFOV);
        fovLerp = Mathf.MoveTowards(fovLerp, fov * fovMultiple, Time.deltaTime * 100f);
        camera.orthographicSize = fovLerp;
        camera.fieldOfView = fovLerp;

        //fovLerp = Mathf.Lerp(camera.orthographicSize, fov * fovMultiple, .2f);


    }

    void UpdateShake() {

        if(shake > 0f) {

            camera.transform.localPosition = Random.insideUnitSphere * shake;
            shake -= Time.deltaTime;

            if(shake < 0f) {
                camera.transform.localPosition = Vector3.zero;
                shake = 0f; 
            }

        }
    }

    public static void ToggleScroll(bool toggle) {
        I.canScroll = toggle;
        if(toggle) {} 
        else {I.scrollRot = 0;}
    }

    public static void SetPosition(Vector3 position) {
        I.transform.position = position;
    }

    public void SetRotation(Quaternion rotation) {
        I.transform.rotation = rotation;
    }

    public static void SetShakeGlobal(float set) {
        I.shake = Mathf.Clamp01(set);
    }

    public static void AddShakeGlobal(float add) {
        I.shake = Mathf.Clamp01(I.shake + add);
    }

    public static void AddShake(float add, Vector3 position) {
        I.shake = Mathf.Clamp01(I.shake + add) * Mathf.Clamp01(I.shakeFalloff - ((Vector3.Distance(I.transform.position, position) - I.shakeFalloff * 5f)));
    }
}
