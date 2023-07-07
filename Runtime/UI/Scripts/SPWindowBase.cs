using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowBase : SPWindow
{
    [Header("Entity")]
    public SPBase baseObject;
    public SPWindowPosition position;


    public override void Init()
    {
        base.Init();
    }
    public virtual void UpdateObject(SPBase newBase) {
        baseObject = newBase;
        if(baseObject) {
            if(position) {
                position.SetFollow(baseObject.transform);   
            }

        } else {

        }
    }
}
