using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum SPCursorState {Default, Text, Disabled, Hand, Move, TextSelect, Add, Drag, Drop, HandCanDrag, Pointer, PointerSlot, Action, LeftClick, RightClick}
[CreateAssetMenu(fileName = "CursorTextures", menuName = "Engine/UI/CursorTextures", order = 1)]
public class SPCursorData : ScriptableObject
{
    [SerializeField] public Texture2D [] cursorTextures;
    [SerializeField] public Vector2 [] cursorHotspots;
}
