using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SPCursorState {Default, Text, Disabled, Hand, Move, TextSelect, Add, Drag, Drop, HandCanDrag, Pointer, PointerSlot, Action, LeftClick, RightClick, HandAction}
[CreateAssetMenu(fileName = "CursorTextures", menuName = "Engine/UI/CursorTextures", order = 1)]
public class SPCursorData : ScriptableObject
{
    [EnumNamedArray( typeof(SPCursorState) )]
    [SerializeField] public Texture2D [] cursorTextures;
    [EnumNamedArray( typeof(SPCursorState) )]
    [SerializeField] public Vector2 [] cursorHotspots;
}
