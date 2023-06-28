using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCamera : MonoBehaviour
{

    public static SPCamera I;
    public static Camera Camera {get{return I.camera;}}
    public static float Shake {get{return I.shake;}}

    [Header("Camera")]
    [SerializeField] private new Camera camera;
    [SerializeField] private AudioListener listener;

    [Header("Settings")]
    [SerializeField] private float moveSpeed = 5f; 
    [SerializeField] private float scrollSpeed = 1f;
    [SerializeField] private float minFOV = 5f, maxFOV = 25f;
    private float fovMultiple = 1f;


    float shake = 0f;
    float scrollRot, scrollLock;
    private float fovLerp = 15f;

    [Header("Screenshot")]
    [SerializeField] private int screenshotSize = 1;
    int screenshotCount = 0;

    [Header("Debug")]
    [SerializeField] private float fov = 15f;
    [SerializeField] private bool follow = true; 
    [SerializeField] private bool canScroll = true; 
    [SerializeField] private Transform followTransform;
    [SerializeField] private Vector3 position;
    [SerializeField] private Quaternion rotation;

    public static void ToggleCamera(bool toggle) {
        I.gameObject.SetActive(toggle);
    }

    public static void ToggleFollowPlayer(bool toggle) {
        Debug.Log("Camera Follow: " + toggle);
        I.follow = toggle;
    }

    public static void Teleport(Vector3 targetPos) { Teleport(targetPos, I.transform.rotation);}
    public static void Teleport(Vector3 targetPos, Quaternion targetRot) {
        Debug.Log("Camera Teleport: " + targetPos.ToString());
        I.transform.position = targetPos;
        I.transform.rotation = targetRot;
    }

    public static void SetTarget(Vector3 targetPos) {SetTarget(targetPos, I.transform.rotation);}
    public static void SetTarget(Vector3 targetPos, Quaternion targetRot) {
        Debug.Log("Camera Target: " + targetPos.ToString());
        I.position = targetPos;
        I.rotation = targetRot;
    }

    void Awake() {

        Debug.Log("Camera Awake");

        I = this; 
        fov = camera.orthographic ? camera.orthographicSize : camera.fieldOfView;

        position = transform.position;
        rotation = transform.rotation;

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

        // Debug.Log("Camera FOV: " + newFOV);
        I.fov = Mathf.Clamp(newFOV, I.minFOV, I.maxFOV);
        if(instant) {
            I.fovLerp = I.fov;
        }
    }


    void UpdateInput() {

        if(SPUIBase.CanInput) {
            if(Input.GetKeyDown(KeyCode.Minus)) {
                SetFOVGlobal(1f);
            } else if(Input.GetKeyDown(KeyCode.Equals)) {
                SetFOVGlobal(-1f); 
            }
                
            if(Input.GetKeyDown(KeyCode.BackQuote) && (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Application.isEditor) {
                screenshotCount++;
                ScreenCapture.CaptureScreenshot("../Screenshots/" + screenshotCount + ".png", screenshotSize);
            }
            
        }    
        

        if(SPUIBase.IsMouseOnScreen && canScroll) {

            if(Input.GetKey(KeyCode.LeftShift)) {
                scrollRot += Input.mouseScrollDelta.y * 10f;
                scrollLock = Mathf.Round(scrollRot / 90) * 90;

                // rotation = rotation * Quaternion.Euler(Vector3.up * Input.mouseScrollDelta.y * 25f);
                // transform.Rotate(0f,Input.mouseScrollDelta.y * 25f,0f);
            } else {
                SetFOVGlobal(fov + Input.mouseScrollDelta.y * -scrollSpeed);
            }
        }
        
    }

    void UpdateCamera() {

        Vector3 newPos = Vector3.zero;
        //POSITION
        if(follow) {
            newPos = followTransform.position;
            listener.transform.position = newPos;
        } else {
            newPos = position;
            listener.transform.position = newPos;
        }

        float distanceToTarget = Mathf.Max(.25f,Vector3.Distance(transform.position,newPos));

        transform.position = Vector3.MoveTowards(transform.position, newPos, distanceToTarget * 2f * Time.deltaTime * moveSpeed);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation * Quaternion.Euler(Vector3.up * scrollLock), 720f * Time.deltaTime);

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

    public void SetPosition(Vector3 position) {
        transform.position = position;
    }

    public static void SetShake(float set) {
        I.shake = Mathf.Clamp01(set);
    }

    public static void AddShake(float add) {
        I.shake = Mathf.Clamp01(I.shake + add);
    }
}
