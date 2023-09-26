using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SPHoverProvider : MonoBehaviour
{

    [Header("Hover")]
    [SerializeField] protected UnityEvent OnHoverStart;
    [SerializeField] protected UnityEvent OnHoverEnd;

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
            SPHoverWindow.Instance.SetWindow(selectable);
        } else {
            OnHoverEnd?.Invoke();
            SPHoverWindow.Instance.SetWindow(null);
        }

    }

}
