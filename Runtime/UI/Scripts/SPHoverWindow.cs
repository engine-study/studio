using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPHoverWindow : SPWindow
{
    public static SPHoverWindow Instance;

    [Header("Hover")]
    [SerializeField] RectTransform child;

    [Header("Debug")]
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform anchor;

    public override void Init() {
        if(hasInit) {return;}
        base.Init();
        Instance = this;
        canvas = GetComponentInParent<Canvas>(true);
        ToggleWindowClose();
    }

    protected override void Destroy() {
        base.Destroy();
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
        
        float sign = Mathf.Sign(anchor.anchoredPosition.x) * -1f;
        transform.position = anchor.position + Vector3.right * sign * (child.rect.width * .5f + 25f) * canvas.scaleFactor;

    }
}
