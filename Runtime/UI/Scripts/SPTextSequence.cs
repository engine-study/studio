using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPTextSequence : MonoBehaviour
{
    
    [Header("Text Sequence")]
    public SPRawText rawText;
    public SPButton next,previous;
    public List<SPTextScriptable> texts;
    public int index = 0;
    public bool isDone = false;

    void Start() {
        Set();
    }
    public void Next() {
        index = Mathf.Clamp(index + 1, 0, texts.Count-1);
        Set();
    }

    public void Previous() {
        index = Mathf.Clamp(index - 1, 0, texts.Count-1);
        Set();
    }

    void Set() {
        rawText.UpdateField(texts[index].Text);
        isDone = index >= texts.Count-1;
        next?.ToggleWindow(!isDone);
        previous?.ToggleWindow(index > 0);
    }
}
