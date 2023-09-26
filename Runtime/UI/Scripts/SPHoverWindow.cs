using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHoverWindow : SPWindow
{
    public static SPHoverWindow Instance;

    [Header("Hover")]
    [SerializeField] RectTransform child;

    [Header("Debug")]
    [SerializeField] RectTransform anchor;

    protected override void Awake() {
        base.Awake();
        Instance = this;
        ToggleWindowClose();
    }

    protected override void OnDestroy() {
        base.OnDestroy();
        Instance = null;
    }

    public void SetWindow(SPWindow window) {
        SetAnchor(window?.Rect);
    }

    public void SetAnchor(RectTransform rect) {

        anchor = rect;
        ToggleWindow(rect != null);

    }
    
    void Update() {
        
        Rect.anchoredPosition = rect.anchoredPosition;

    }
}
