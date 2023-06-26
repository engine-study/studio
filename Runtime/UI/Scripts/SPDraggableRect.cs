using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPDraggableRect : MonoBehaviour
{
    public SPBlockDraggable dragParent;
    public void RecoverRect() {
        StartCoroutine(RecoverCoroutine());
    }

    IEnumerator RecoverCoroutine() {
        yield return null;
        dragParent.ToggleDrag(false);
    }
}
