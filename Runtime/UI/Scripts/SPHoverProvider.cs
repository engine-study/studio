using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SPHoverProvider : MonoBehaviour
{
    [Header("Hover")]
    public bool displayHoverWindow = true;
    public Vector2 anchor;
    [SerializeField] RectTransform rectOverride;

    [Header("Events")]
    [SerializeField] UnityEvent OnHoverStart;
    [SerializeField] UnityEvent OnHoverEnd;

    SPWindowSelectable selectable;
    
    void Awake() {
        selectable = GetComponent<SPWindowSelectable>();
        selectable.OnHoverUpdate += ToggleHover;
    }

    void OnDestroy() {
        selectable.OnHoverUpdate -= ToggleHover;
    }

    
    public void ToggleHover(bool toggle) {

        if(toggle) {

            OnHoverStart?.Invoke();
            
            if(displayHoverWindow) {
                if(rectOverride) {SPHoverWindow.Instance.SetWindow(rectOverride, anchor);}
                else {SPHoverWindow.Instance.SetWindow(selectable.Rect, anchor);}
            }
        } else {
            
            OnHoverEnd?.Invoke();

            if(displayHoverWindow) {
                SPHoverWindow.Instance.SetWindow((RectTransform)null, anchor);
            }
        }

    }

}
