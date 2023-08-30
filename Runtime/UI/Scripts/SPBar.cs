using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPBar : SPWindow
{
    public bool animateLerp = true;
    float target;
    public Image fill;
    public AudioClip fillSound;

    public void SetFill(float lerp, bool instant = false) {

        target = Mathf.Clamp01(lerp);

        if(animateLerp && !instant && fill.fillAmount != target) {
            enabled = true;
        } else {
            fill.fillAmount = lerp;
        }

    }

    void Update() {
        fill.fillAmount = Mathf.MoveTowards(fill.fillAmount, target, Time.deltaTime * .5f);

        if(fill.fillAmount == target) {
            enabled = false;
        }
    }


}
