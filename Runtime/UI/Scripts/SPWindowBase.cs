using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowBase : SPWindow
{
    [Header("SPBase")]
    public SPBase baseObject;
    public SPWindowPosition position;

    public virtual void UpdateObject(SPBase newBase) {
        baseObject = newBase;
        if(baseObject) {
            position?.SetFollow(baseObject.transform);   
        } else {

        }
    }
}
