using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPCursorTexture : MonoBehaviour
{
    public static SPCursorTexture I;
    public static SPCursorState CursorState;
    public SPCursorData data;

    void Awake() {
        I = this;
    }
    public static void UpdateCursor(SPCursorState newState) {

        if(!I)
            return;

        CursorState = newState;
        Cursor.SetCursor(I.data.cursorTextures[(int)newState], I.data.cursorHotspots[(int)newState], CursorMode.Auto);

    }

}
