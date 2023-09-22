using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SPClickableGameobject : MonoBehaviour
{
    public bool Hovering {get{return hover;}}

    [Header("Clickable")]
    [SerializeField] bool hover;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected UnityEvent OnHover;
    [SerializeField] protected UnityEvent OnClick;

    void Update() {

        bool isHovering = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);  
        RaycastHit hit;  
        if (Physics.Raycast(ray, out hit)) {  
            if (hit.collider.attachedRigidbody == rb) {  
                isHovering = true;
            }  
        }  
        

        if(hover != isHovering) {ToggleHover(isHovering);}

        if(hover && Input.GetMouseButtonDown(0)) {
            OnClick?.Invoke();
        }
    }

    void ToggleHover(bool toggle) {
        hover = toggle;
        OnHover?.Invoke();
    }
}
