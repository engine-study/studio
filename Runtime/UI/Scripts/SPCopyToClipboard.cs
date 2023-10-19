using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCopyToClipboard : MonoBehaviour
{
    [Header("Copy")]
    public SPWindowSelectable selectable;

    public void Copy() {
        GUIUtility.systemCopyBuffer = selectable.Field;
    }
}
