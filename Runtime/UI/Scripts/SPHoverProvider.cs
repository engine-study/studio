using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SPHoverProvider : MonoBehaviour
{
    [Header("Hover")]
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
            SPHoverWindow.Instance.SetWindow(rectOverride ?? selectable.Rect, anchor);
        } else {
            OnHoverEnd?.Invoke();
            SPHoverWindow.Instance.SetWindow((RectTransform)null, anchor);
        }

    }

}
