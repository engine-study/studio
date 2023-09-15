using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text", menuName = "Engine/UI/Text", order = 1)]
public class SPTextScriptable : ScriptableObject
{
    public string Text {get{return text;}}
    [TextArea(1,50)]
    [SerializeField] string text;
}
