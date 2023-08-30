using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPStrobeUI : MonoBehaviour
{
  
    [SerializeField] bool strobeOnEnable = true;
    [SerializeField] SPWindow target;
    [SerializeField] public float duration = -1;
    Coroutine animationCoroutine;

    public static void ToggleStrobe(SPWindow window, bool run = true, float newDuration = .75f) {

        SPStrobeUI strobe = window.GetComponent<SPStrobeUI>();

        if(strobe == null) {
            strobe = window.gameObject.AddComponent<SPStrobeUI>();
        }

        strobe.duration = newDuration;

        if(run) {
            
            strobe.strobeOnEnable = true;

            if(window.gameObject.activeInHierarchy) {
                strobe.StartStrobe();
            }

        } else {
            strobe.StopStrobe();
            strobe.strobeOnEnable = false;
        }
        
    }

    void OnEnable() {
        
        if(strobeOnEnable) {
            StartStrobe();
        }

    }

    void OnDisable() {
        animationCoroutine = null;
    }

    public void StartStrobe() {

        if(target == null) {
            target = GetComponent<SPWindow>();
        }

        if (animationCoroutine != null) { StopCoroutine(StrobeCoroutine()); }
        animationCoroutine = StartCoroutine(StrobeCoroutine());
        
    }

    public void StopStrobe() {

        if(animationCoroutine != null) {StopCoroutine(animationCoroutine);}
        animationCoroutine = null;

        if(target == null) {
            target = GetComponent<SPWindow>();
        }

        target.UpdateColor();
    }

    IEnumerator StrobeCoroutine() {

        bool strobe = false;

        Color colorA = target.Theme.defaultTheme.bgColor;
        Color colorB = target.Theme.defaultTheme.color;

        colorA.a = target.Theme.defaultTheme.color.a;
        colorB.a = target.Theme.defaultTheme.bgColor.a;

        SPWindowTheme.SPTheme strobeTheme = new SPWindowTheme.SPTheme(colorA, colorB);

        float length = 0;
        while(duration == -1 || length < duration || strobe) {
 
            strobe = !strobe;
            yield return new WaitForSeconds(.2f);

            length += .2f;

            if(strobe) {target.UpdateColor(strobeTheme);}
            else {target.UpdateColor();}
            
        }
    }
}
