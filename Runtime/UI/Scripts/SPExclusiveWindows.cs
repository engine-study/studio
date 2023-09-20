using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPExclusiveWindows : MonoBehaviour
{
    public List<SPWindow> windows;

    void Awake() {

        windows = new List<SPWindow>();
        for(int i = 0; i < transform.childCount; i++) {
            SPWindow menuWindow = transform.GetChild(i).GetComponent<SPWindow>();
            if(menuWindow != null) windows.Add(menuWindow);
        }
        
        foreach(SPWindow w in windows) {
            w.ToggleWindowClose();
        }

    }

    public void ToggleWindowExclusive(SPWindow window) {
        ToggleWindowExclusive(!window.gameObject.activeInHierarchy, window);
    }

    public void ToggleWindowExclusive(bool toggle, SPWindow window) {

        if(toggle) {

            window.ToggleWindow(true);

            foreach(SPWindow w in windows) {
                if(w != window && w.gameObject.activeInHierarchy) {
                    w.ToggleWindowClose();
                }
            }
        } else {
            window.ToggleWindow(false);
        }
    
    }

}
