using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowViews : SPWindowParent
{
   [Header("Window Views")]
   public SPWindow [] views;

   public void SetView(int index) {
    for(int i = 0; i < views.Length; i++) {
        views[i].ToggleWindow(i == index);
    }
   }

   
}
