﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR; 
using UnityEngine.Experimental.XR;

public enum de_button { Space, Secondary, LeftClick, RightClick, Primary, Action, SwitchMode, Kill, Camera, Menu, Shoot, Submit, Tab, Chat, Ctrl, Copy, Rotate, Variable, Touchpad, _Count }
public enum de_axis { PadXPositive, PadXNegative, PadYPositive, PadYNegative, LeftClick, RightClick,_Count }
public enum de_controller {None = -1, Oculus, Vive, Valve, Windows}
public class SPInput : MonoBehaviour {


    private static SPInput Instance;
    public static Camera Camera;
    public static Vector3 MouseWorldPos {get{return Instance.mouseWorldPos;}}
    public static Vector3 MousePlanePos {get{return Instance.mousePlanePos;}}
    public static Vector3 MouseNormal {get{return Instance.mouseNormal;}}
    public static bool MouseHit {get{return Instance.didHit;}}
    public static bool MouseGround {get{return Instance.didHit && Instance.hit.collider.gameObject.layer == SPLayers.GroundLayer;}}
    public static GameObject MouseGameObject {get{return Instance.hit.collider.gameObject;}}
    public static Ray MouseRay {get{return Instance.mouseRay;}}
    public static de_controller detectedControllers; 
    public static string [] controllerNames;
    public static bool [] axisDownLeft = new bool[(int)de_axis._Count];
    public static bool [] axisDownRight = new bool[(int)de_axis._Count];
    public static float [] scrollTimeLeft = new float[(int)de_axis._Count];
    public static float [] scrollTimeRight = new float[(int)de_axis._Count];
    protected static float minScrollTime = .225f; 

    

    Vector3 mouseWorldPos, mousePlanePos, mouseNormal;
    int hits;
    bool didHit;
    Ray mouseRay;
    RaycastHit [] results = new RaycastHit[20];
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    float enter = 0.0f;
    RaycastHit hit;


    void Awake() {
        Instance = this; 
        Camera = Camera.main;
    }

    void Update() {

        float planeHeight = 0f;
    
        plane = new Plane(Vector3.up, Vector3.up * planeHeight);

        mouseRay = Camera.ScreenPointToRay(Input.mousePosition + Vector3.forward * 1000f);
        bool mouseDown = Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);

        //project the mouse position onto the ground plane
        if (plane.Raycast(mouseRay, out enter)) {
            //Get the point that is clicked
            Vector3 hitPoint = mouseRay.GetPoint(enter);
            mousePlanePos = hitPoint;
        } else {
            mousePlanePos = Vector3.zero;
        }

        //raycast the mouse against colliders in the scene
        if(true) {

            //hits = Physics.RaycastNonAlloc(mouseRay, results, 1000f, LayerMask.NameToLayer("Everything"), QueryTriggerInteraction.Ignore);
            didHit = Physics.Raycast(mouseRay, out hit, 5000f, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            if(didHit) {
                mouseWorldPos = hit.point;////results[0].point;
                mouseNormal = hit.normal;
            } else {
                mouseWorldPos = mousePlanePos;
                mouseNormal = Vector3.zero;
            }

        } 
        

    }

    public static bool GetScrollWheel(bool forwardScroll = true) {
        return forwardScroll ? Input.GetAxis("Mouse ScrollWheel") > 0f : Input.GetAxis("Mouse ScrollWheel") < 0f;
    }

    void OnDrawGizmosSelected() {
        
        if(!Application.isPlaying) {
            return;
        }
    
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(MousePlanePos, 1f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(MouseWorldPos, .5f);

        if(didHit) {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(hit.point, .25f);
        } else {
            Gizmos.color = Color.gray;
            Gizmos.DrawWireSphere(Vector3.zero, .25f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine( MouseRay.origin, MouseRay.origin + MouseRay.direction * 10000f );

    }

}
