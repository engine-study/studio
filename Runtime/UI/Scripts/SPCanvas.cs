using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SPCanvas : MonoBehaviour
{

    [Header("SPCanvas")]
    public Canvas canvas; 
    public GraphicRaycaster raycaster;
    // public RenderMode mode;

    public static List<SPCanvas> Canvases;

    void Start() {
    
        if(Canvases == null) {
            Canvases = new List<SPCanvas>();
        }
        
        Canvases.Add(this);

        if(canvas == null) {
            canvas = GetComponent<Canvas>();
        }
        if(raycaster == null) {
            raycaster = GetComponent<GraphicRaycaster>();
        }

        // if(mode == RenderMode.ScreenSpaceOverlay) {
        //     canvas.worldCamera = null;
        // } else {
        //     canvas.worldCamera = Camera.main;
        // }
    }

    public void ToggleVisible(bool toggle) {
        canvas.enabled = toggle;
        raycaster.enabled = toggle;
    }

    void OnDestroy() {
        Canvases.Remove(this);
    }
}
