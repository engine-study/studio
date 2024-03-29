using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SPTextSequence : SPWindow
{
    public System.Action OnUpdated;
    public System.Action OnDone;
    public System.Action OnNext;
    public System.Action OnPrevious;

    public int Index {get{return index;}}
    public bool IsDone {get{return isDone;}}
    public bool HasReachedFinal {get{return hasReachedFinal;}}
    public SPRawText Text {get{return rawText;}}
    public List<SPTextScriptable> Texts {get{return texts;}}

    [Header("Text Sequence")]
    [SerializeField] SPRawText rawText;
    [SerializeField] SPButton next,previous;
    [SerializeField] List<SPTextScriptable> texts;
    [SerializeField] int index = 0;
    [SerializeField] bool isDone = false;
    [SerializeField] bool hasReachedFinal = false;

    public override void Init() {
        if(hasInit) {return;}
        base.Init();
        Set();
    }

    public void SetText(SPTextScriptable [] newTexts) {
        index = 0;
        texts = newTexts.ToList();
        isDone = false;
        hasReachedFinal = false;
        Set();
    }

    void Update() {
        if(SPUIBase.CanInput) {

            if(Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
                next.Click();
                Next();
            } else if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)){
                previous.Click();
                Previous();
            }
        }
    }
    public void Next() {
        index = Mathf.Clamp(index + 1, 0, texts.Count-1);
        Set();
        OnNext?.Invoke();
    }

    public void Previous() {
        index = Mathf.Clamp(index - 1, 0, texts.Count-1);
        Set();
        OnPrevious?.Invoke();
    }

    void Set() {

        isDone = index >= texts.Count-1;
        if(!hasReachedFinal) {hasReachedFinal = isDone;}

        next?.ToggleWindow(!isDone);
        previous?.ToggleWindow(index > 0);
        
        if(texts == null || texts.Count == 0) { rawText.UpdateField(""); return;}

        rawText.UpdateField(texts[index].Text);

        if(isDone)
            OnDone?.Invoke();

        OnUpdated?.Invoke();
        
    }
}
