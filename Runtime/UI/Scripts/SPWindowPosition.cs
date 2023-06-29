using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPWindowPosition : MonoBehaviour
{
    [SerializeField] protected Transform follow;
    [SerializeField] protected SPWindow window;

    void Start() {
        window = GetComponent<SPWindow>();
        SetFollow(follow);
    }

    public void SetFollow(Transform newFollow) {
        follow = newFollow;
        enabled = follow != null;

        if(enabled) {
            UpdatePosition();
        }
    }    

    void LateUpdate() {
        UpdatePosition();
    }

    void UpdatePosition() {
        SPUIBase.WorldToCanvas(follow.position, window.Rect);
    }
}
