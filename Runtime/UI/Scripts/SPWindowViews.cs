using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SPWindowViews : SPWindowParent {
    [Header("Window Views")]
    public SPWindow[] views;
    [SerializeField] string EnumName;
    [SerializeField] SPWindowSelectable header;
    Array enumValues;

    public override void Init() {
        if(HasInit) return;
        base.Init();

        Type enumType = Type.GetType(EnumName);
        if (enumType != null && enumType.IsEnum) {
            enumValues = Enum.GetValues(enumType);
        }
    }
    
    public void SetView(int index) {
        for (int i = 0; i < views.Length; i++) {
            views[i]?.ToggleWindow(i == index);
        }

        if(enumValues != null && header && index < enumValues.Length) {
            header.UpdateField(enumValues.GetValue(index).ToString());
        }
    
    }

}
