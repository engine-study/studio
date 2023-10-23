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
    [SerializeField] RectTransform target;
    public Vector3 useAnchor;

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

    public void SetWindow(SPWindow window, Vector2 newAnchor) {
        SetWindow(window?.Rect, newAnchor);
    }

    public void SetWindow(RectTransform rect, Vector2 newAnchor) {

        target = rect;
        useAnchor = newAnchor;

        ToggleWindow(rect != null);

    }
    
    void Update() {

        float signX = Mathf.Sign(target.anchoredPosition.x) * -1f;
        float signY = Mathf.Sign(target.anchoredPosition.y) * 1f;

        Vector3 xPos = Vector3.right * signX * (child.rect.width * .5f + 25f) * useAnchor.x;
        Vector3 yPos = Vector3.up * signY * (child.rect.height * .5f + 25f) * useAnchor.y;

        transform.position = target.position + ((xPos+yPos) * canvas.scaleFactor);

    }
}
