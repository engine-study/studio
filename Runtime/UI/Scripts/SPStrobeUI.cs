using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPStrobeUI : MonoBehaviour
{
  
    [SerializeField] protected bool strobeOnEnable = true;
    [SerializeField] protected SPWindow target;
    Coroutine animationCoroutine;

    public static void ToggleStrobe(SPWindow window, bool run = true) {

        SPStrobeUI strobe = window.GetComponent<SPStrobeUI>();

        if(strobe == null) {
            strobe = window.gameObject.AddComponent<SPStrobeUI>();
        } 

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

        if(animationCoroutine == null) {
            animationCoroutine = StartCoroutine(StrobeCoroutine());
        }
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

        while(true) {

            // Debug.Log("Strobe");

            strobe = !strobe;
            
            yield return new WaitForSeconds(.25f);

            if(strobe) {target.UpdateColor(strobeTheme);}
            else {target.UpdateColor();}
            
        }
    }
}
