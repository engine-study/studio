using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;
public class SPBlockDraggable : SPWindowSelectable, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public bool CanDrag{get{return DragValid && canDrag && StateDraggable == SPDraggableState.Draggable;}}
    public bool DragValid{get{return Block != null && Block.IsValid && IsInteractable && (holdingSlot == null || holdingSlot.IsInteractable);}}
    public bool CanDropInSlot{get{return SPUIBase.IsMouseOnScreen && potentialSlot != null && potentialSlot.IsInteractable;}}
    public bool CanDropInWorld{get{return SPUIBase.IsMouseOnScreen && draggingOverWorld;}} //SPOntBlock.userBlocks < 10 && 

    public SPBlock Block;
    
    [Header("Draggable")]
    public bool draggingOutOfSlotDeletes;
    [SerializeField] protected SPBlockSlot holdingSlot;
    [SerializeField] protected SPDraggableRect draggableRect;
    [SerializeField] protected RectTransform rectTarget;

    [Header("Debug")]
    [SerializeField] protected SPBlockSlot potentialSlot;
    [SerializeField] protected Vector3 draggedPosition;

    protected SPBlockSlot lastPotentialSlot;
    bool draggingOverWorld;

    protected Transform originalParent;
    protected Vector3 originalPos, originalScale;
    protected Vector2 originalPivot, originalAnchorMax, originalAnchorMin;
    protected Vector2 originalOffsetMax, originalOffsetMin, originalSizeDelta;

    protected int originalSiblingIndex;


    List<Graphic> graphicRaycasts;

    public override void Init() {

        if(hasInit) {
            // Debug.LogError("Double init");
            return;
        }

        graphicRaycasts = GetComponentsInChildren<Graphic>(true).ToList();
        for(int i = graphicRaycasts.Count - 1; i > -1; i--) { if(!graphicRaycasts[i].raycastTarget) {graphicRaycasts.RemoveAt(i);}}

        if(!holdingSlot) {
            holdingSlot = GetComponentInParent<SPBlockSlot>();
        }

        base.Init();

        ToggleDrag(false, false);
    }

    protected override void OnEnable()
    {
        base.OnEnable();


    }

    protected override void OnDisable()
    {
        base.OnDisable();

        if(IsDragging) {
            draggableRect.RecoverRect();
        }
    }

    public void ToggleWindowBlock(bool toggle, SPWindowSelectable newBlock) {

    }


    public override void GiveSpecialCursor() {
        if(CanDrag)
            SPCursorTexture.UpdateCursor(SPCursorState.HandCanDrag);
        else {
            SPCursorTexture.UpdateCursor(SPCursorState.Hand);
        }
    }

    public virtual void OnBeginDrag(PointerEventData eventData) {

        if(!CanDrag)
            return;

        ToggleDrag(true);

    }

    public virtual void OnDrag(PointerEventData eventData) {

        //Debug.Log("OnDrag: " + eventData.selectedObject?.name + " selectedObject.");
        //Debug.Log("OnDrag: " + eventData.pointerDrag?.name + " pointerDrag.");

        if(!IsDragging) {
            return;
        }

        //cancel the drag if its no longer valid
        if(!DragValid) {
            ToggleDrag(false);
            return;
        }

        PointerEventData pointerData = new PointerEventData(EventSystem.current) {pointerId = -1,};
        pointerData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        int UILayer = LayerMask.NameToLayer("UI");

        //Debug.Log("Results");

        draggingOverWorld = true;
        lastPotentialSlot = potentialSlot;
        potentialSlot = null;

        for(int i = 0; i < results.Count; i++) {

            potentialSlot = results[i].gameObject.GetComponent<SPBlockSlot>();

            if(potentialSlot) {
                draggingOverWorld = false;
                break;
            }

            //check if we are dragging over UI
            if(results[i].gameObject.layer == UILayer && !results[i].gameObject.transform.IsChildOf(transform)) {
                draggingOverWorld = false;
            }


        }

        //if we had a slot that we dragged out of, let it know
        if(lastPotentialSlot != null && potentialSlot != lastPotentialSlot) {
            lastPotentialSlot.OnSlotHoverExit?.Invoke(this);
        }

        //if we have dragged over a new slot, let it know
        if(potentialSlot != null && potentialSlot != lastPotentialSlot) {
            potentialSlot.OnSlotHoverEnter?.Invoke(this);
        }

        if(CanDropInSlot) {
            //drop into a UI slot
            draggedPosition = SPUIBase.ScreenToCanvas(Input.mousePosition + Vector3.up * 25f, SPUIBase.DraggableParent);
            SPCursorTexture.UpdateCursor(SPCursorState.Pointer);
            // SPUIBase.UpdateCursor(SPCursorState.Drop);
            // draggedPosition = SPUIBase.ScreenToCanvas(Input.mousePosition + Vector3.up, SPUIBase.DraggableParent);
        } else if(CanDropInWorld) {
            //drop into the world
            draggedPosition = SPUIBase.ScreenToCanvas(Input.mousePosition + Vector3.up * 25f, SPUIBase.DraggableParent);
            SPCursorTexture.UpdateCursor(SPCursorState.Pointer);
        } else {
            //can't drop onto anything, over UI
            draggedPosition = SPUIBase.ScreenToCanvas(Input.mousePosition + Vector3.up, SPUIBase.DraggableParent);
            SPCursorTexture.UpdateCursor(SPCursorState.Drag);
        }


        //rectTarget.anchoredPosition = SPUI.Instance.ViewportToCanvas(SPUI.Instance.Camera.ScreenToViewportPoint(Input.mousePosition));
        //rectTarget.transform.position = (Vector3)eventData.pointerCurrentRaycast.worldPosition;

        //this only works with screen space camera
        //draggedPosition = SPUI.Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,2.5f));
        //rectTarget.transform.position = draggedPosition;

        //this doesnt work
        //Vector2 viewport = SPUI.Camera.ScreenToViewportPoint(Input.mousePosition);
        //draggedPosition = SPUI.ViewportToCanvas(viewport);

        //newest method, checks what mode the canvas is in 
        //put the icon a little above the mouse
        //rectTarget.transform.position = draggedPosition;


        rectTarget.anchoredPosition = draggedPosition;
        rectTarget.localScale = Vector3.one;

    }

    
    /*
    public virtual void OnDrop(PointerEventData eventData) {
        
        Debug.Log("OnDropDraggable: " + eventData.selectedObject?.name + " selectedObject.");
        Debug.Log("OnDropDraggable: " + eventData.pointerDrag?.name + " pointerDrag.");

        GiveDefaultCursor();

    }
    */


    public virtual void OnEndDrag(PointerEventData eventData) {

        //Debug.Log("OnEndDragDraggable: " + eventData.selectedObject?.name + " selectedObject.");
        //Debug.Log("OnEndDragDraggable: " + eventData.pointerDrag?.name + " pointerDrag.");

        if(!IsDragging) {
            return;
        }

        //emergency quit if we can't drag anymore
        if(!DragValid) {
            ToggleDrag(false);
            return;
        }

        //don't let people drag things back into the same slot they were in 
        if(potentialSlot != null && potentialSlot == holdingSlot) {
            ToggleDrag(false);
            return;
        }

        if(CanDropInSlot) {
            //tell the new slot to set themselves up
            potentialSlot.BlockDropped(this);

        } else if(CanDropInWorld) {
            //SPGame.SpawnOntBunch(Block, new Vector3(draggedPosition.x, draggedPosition.y, -0.5f));  

            //Vector3 spawnPos = SPUI.ScreenToWorld(Input.mousePosition, SPUI.DraggableParent);
            //Vector3 spawnPos = SPPlayer.LocalPlayer.transform.position + SPPlayer.LocalPlayer.transform.forward * .5f + Vector3.up * 1f;
            Vector3 spawnPos = SPInput.MouseWorldPos + Vector3.up * 1.5f;
            // SPGame.SpawnOnt(Block, spawnPos);  

        } else {

        }

        //turn off our slot, we have been moved to another one
        if(holdingSlot) {
            holdingSlot.ToggleSlotBlock(false, null);
        }

        ToggleDrag(false);

    }

    public void ToggleDrag(bool toggle, bool isRealDragEvent = true) {

        if(toggle && IsDragging) {
            Debug.LogError("Trying to drag while dragging");
        }
        //isDragging = toggle;

        //ToggleState(toggle ? SPSelectableState.Inactive : SPSelectableState.Default);
        ToggleDraggableState(toggle ? SPDraggableState.Dragging : SPDraggableState.Draggable);

        potentialSlot = null;

        if(toggle) {
            
            //Debug.Log("OnBeginDrag: " + eventData.selectedObject?.name + " selectedObject.");
            //Debug.Log("OnBeginDrag: " + eventData.pointerDrag?.name + " pointerDrag.");

            if(originalParent != null) {
                Debug.LogError("Starting new drag without ending last one.");
            }

            originalPos = rectTarget.anchoredPosition3D;
            originalScale = rectTarget.localScale;
            originalParent = rectTarget.parent;
            originalSiblingIndex = rect.transform.GetSiblingIndex();

            originalAnchorMin = rectTarget.anchorMin;
            originalAnchorMax = rectTarget.anchorMax;
            originalPivot = rectTarget.pivot;

            originalOffsetMax = rectTarget.offsetMax;
            originalOffsetMin = rectTarget.offsetMin;
            originalSizeDelta = rectTarget.sizeDelta;

            rectTarget.parent = SPUIBase.DraggableParent;

            //reset all
            rectTarget.localScale = Vector3.one;
            rectTarget.localRotation = Quaternion.identity;
            rectTarget.localPosition = Vector3.zero;
            
            rectTarget.anchorMin = new Vector2(.5f, .5f);
            rectTarget.anchorMax = new Vector2(.5f, .5f);
            rectTarget.pivot = new Vector2(0.5f, 0.5f);

            rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,50f);
            rectTarget.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);


            draggedPosition = SPUIBase.ScreenToCanvas(Input.mousePosition + Vector3.up * 25f, SPUIBase.DraggableParent);
            rectTarget.anchoredPosition = draggedPosition;

            //rectTarget.localScale = originalScale * .5f;


        } else {

            if(originalParent != null) {

                rectTarget.parent = originalParent;

                rectTarget.localScale = originalScale;
                rectTarget.localRotation = Quaternion.identity;

                rectTarget.SetSiblingIndex(originalSiblingIndex);
                
                rectTarget.anchorMin = originalAnchorMin;
                rectTarget.anchorMax = originalAnchorMax;
                rectTarget.pivot = originalPivot;

                rectTarget.sizeDelta = originalSizeDelta;
                rectTarget.offsetMax = originalOffsetMax;
                rectTarget.offsetMin = originalOffsetMin;

                rectTarget.anchoredPosition3D = originalPos;

                originalParent = null;

            } else {

            }



        }

        for(int i = 0; i < graphicRaycasts.Count; i++) { graphicRaycasts[i].raycastTarget = !toggle;}

        if(SPUIBase.I && isRealDragEvent)
            SPUIBase.SetDragging(toggle);

    }
   
}
