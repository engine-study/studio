using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHoverDescription : MonoBehaviour
{
    public string description;

    public void ToggleHover() {
        if(string.IsNullOrEmpty(description)) {return;}
        SPRawText text = SPHoverWindow.Instance.GetComponentInChildren<SPRawText>(true);
        text?.UpdateField(description);
    }
}
