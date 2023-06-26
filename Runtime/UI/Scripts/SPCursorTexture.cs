using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SPCursorState {Default, Text, Disabled, Hand, Move, TextSelect, Add, Drag, Drop, HandCanDrag, Pointer, PointerSlot, Action, LeftClick, RightClick}
public class SPCursorTexture : MonoBehaviour
{
    public static SPCursorTexture I;
    public static SPCursorState CursorState;

    [SerializeField] protected Texture2D [] cursorTextures;
    [SerializeField] protected Vector2 [] cursorHotspots;

    void Awake() {
        I = this;
    }
    public static void UpdateCursor(SPCursorState newState) {

        if(!I)
            return;

        CursorState = newState;

        Cursor.SetCursor(I.cursorTextures[(int)newState], I.cursorHotspots[(int)newState], CursorMode.Auto);

    }

}
