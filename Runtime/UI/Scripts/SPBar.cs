using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SPBar : SPWindow
{
    public Image fill;

    public void SetFill(float lerp) {
        fill.fillAmount = lerp;
    }
}
