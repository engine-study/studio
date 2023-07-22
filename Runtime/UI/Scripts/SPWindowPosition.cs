using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OffsetType { Camera, World, Local }
public class SPWindowPosition : MonoBehaviour {
    [SerializeField] protected Transform follow;
    [SerializeField] protected SPWindow window;
    [SerializeField] protected Vector3 offset = Vector3.zero;
    [SerializeField] protected OffsetType offsetSpace = OffsetType.Camera;

    RectTransform rect;
    bool hasInit = false;

    void Start() {
        if (!hasInit) {
            Init();
        }
    }

    void Init() {

        if (hasInit) {
            return;
        }

        if (window == null) {
            window = GetComponent<SPWindow>();
        }

        if (window == null) {
            rect = GetComponent<RectTransform>();
        } else {
            rect = window.Rect;
        }

        Debug.Assert(rect != null, "Null rect", this);

        hasInit = true;
    }

    void OnEnable() {
        if (hasInit) {
            SetFollow(follow);
        }
    }

    public void SetFollow(Transform newFollow, Vector3 newOffset) {
        offset = newOffset;
        SetFollow(newFollow);
    }

    public void SetFollow(Transform newFollow) {

        if (!hasInit) {
            Init();
        }

        follow = newFollow;
        enabled = follow != null;

        if (enabled) {
            UpdatePosition();
        }
    }

    void LateUpdate() {
        UpdatePosition();
    }

    void UpdatePosition() {
        if (offsetSpace == OffsetType.Camera) {
            SPUIBase.WorldToCanvas(follow.position + SPUIBase.Camera.transform.TransformPoint(offset) - SPUIBase.Camera.transform.position, rect);
        } else if (offsetSpace == OffsetType.World) {
            SPUIBase.WorldToCanvas(follow.position + offset, rect);
        } else if (offsetSpace == OffsetType.Local) {
            SPUIBase.WorldToCanvas(follow.InverseTransformPoint(offset), rect);
        }
    }
}
