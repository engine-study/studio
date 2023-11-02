using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPExclusiveWindows : SPWindowParent
{
    [Header("Exclusive")]
    public List<SPWindow> exclusiveWindows;
    SPWindow activeWindow;

    public override void Init() {

        if(hasInit) {return;}

        base.Init();

        exclusiveWindows = new List<SPWindow>();
        for(int i = 0; i < transform.childCount; i++) {
            SPWindow menuWindow = transform.GetChild(i).GetComponent<SPWindow>();
            if(menuWindow != null) exclusiveWindows.Add(menuWindow);
        }
        
        foreach(SPWindow w in exclusiveWindows) {
            w.ToggleWindowClose();
        }

    }

    public void ToggleWindowExclusive(SPWindow window) {
        ToggleWindowExclusive(!window.gameObject.activeInHierarchy, window);
    }

    public void Update() {
        if(activeWindow && Input.GetKeyDown(KeyCode.Escape)) {
            ToggleWindowExclusive(false, activeWindow);
        }
    }

    public void ToggleWindowExclusive(bool toggle, SPWindow window) {

        if(toggle) {

            window.ToggleWindow(true);
            activeWindow = window;

            foreach(SPWindow w in exclusiveWindows) {
                if(w != window && w.gameObject.activeInHierarchy) {
                    w.ToggleWindowClose();
                }
            }
        } else {
            window.ToggleWindow(false);
        }
    
    }

}
