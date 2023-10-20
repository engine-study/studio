using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHoverDescription : MonoBehaviour
{
    public string description;

    public void ToggleHover() {
        SPRawText text = SPHoverWindow.Instance.GetComponentInChildren<SPRawText>(true);
        text?.UpdateField(description);
    }
}
