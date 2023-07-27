using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UITheme", menuName = "Engine/UI/Theme", order = 1)]
public class SPThemeScriptable : ScriptableObject {

    public SPWindowTheme Theme {get{return theme;}}
    
    [Header("Theme")]
    [SerializeField] protected SPWindowTheme theme;

}
