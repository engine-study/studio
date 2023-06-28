using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPUIInstance : MonoBehaviour
{
    public static SPUIInstance Instance;

    public static Camera Camera {get{return Instance.mainCamera;}}
    public static RectTransform DraggableParent {get{return Instance.draggableParent;}}
    public static Canvas Canvas {get{return Instance.mainCanvas;}}
    public static RectTransform CanvasRect {get{return Instance.canvasRect;}}
    public static AudioSource AudioSource {get{return Instance.audioSource;}}

    [Header("References")]
    [SerializeField] protected Canvas mainCanvas;
    [SerializeField] protected Camera mainCamera;
    [SerializeField] protected RectTransform canvasRect;
    [SerializeField] protected RectTransform draggableParent; 
    [SerializeField] protected AudioSource audioSource;

    void Awake() {
        Instance = this;
    }

    void OnDestroy() {
        Instance = null;
    }
}
