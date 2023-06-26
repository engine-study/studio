using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;
public class SPBlockSlot : SPWindowSelectable
{

    public SPBlockDraggable BlockDraggable {get{return draggableBlock;}}

    [Header("Block Slot")]
    [SerializeField] protected SPStrobeUI strobe;
    [SerializeField] protected SPBlockDraggable draggableBlock;
    [SerializeField] protected GameObject slotGraphic;

    [Header("Event")]
    [SerializeField] public UnityEvent OnSlotChanged;
    public Action<SPBlockDraggable> OnSlotHoverEnter;
    public Action<SPBlockDraggable> OnSlotHoverExit;
    public Action<SPBlockDraggable> OnSlotDrop;
    public Action<SPBlockDraggable> OnSlotUpdate;

    public Action OnSubmitted;

    /*
    public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerEnter(eventData);

    }

    public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData) {
        base.OnPointerUp(eventData);


    }
    */

    public override void Init()
    {
        if(hasInit) {
            return;
        }

        SPUIBase.OnDragStart += DragStarted;
        SPUIBase.OnDragEnd += DragEnded;

        base.Init();
    }

    protected override void Destroy()
    {
        SPUIBase.OnDragStart -= DragStarted;
        SPUIBase.OnDragEnd -= DragEnded;
        
        base.Destroy();
    }

    void DragStarted() {

         if(!gameObject.activeInHierarchy) {
            return;
        }

        // if our block is not the one being dragged, hide it to show we can be dropped into
        if(!BlockDraggable.IsDragging && this.IsInteractable) {
            BlockDraggable.ToggleWindowClose();
        }

        strobe.StartStrobe();
    }

    void DragEnded() {

        
        BlockDraggable.ToggleWindow(true);

        if(!gameObject.activeInHierarchy) {
            return;
        }

        strobe.StopStrobe();
    }

    
    public override void ToggleCanDrag(bool toggle) {
        base.ToggleCanDrag(toggle);
        draggableBlock.ToggleCanDrag(toggle);

    }



    public void ToggleSlotBlock(bool toggle, SPBlockDraggable newBlock) {
        
        // base.ToggleWindowBlock(toggle, newBlock);

        //we already had something in our slot, release it
        if(BlockDraggable.gameObject.activeSelf) {

        }

        if(toggle && newBlock != null) {
            BlockDraggable.ToggleWindowBlock(true, newBlock);
        } else {
            BlockDraggable.ToggleWindowBlock(false, null);
        }

        // slotGraphic.gameObject.SetActive(IsInteractable && (newBlock == null || !newBlock.IsValid));

        OnSlotUpdate?.Invoke(newBlock);
    }

    public void BlockDropped(SPBlockDraggable newDraggable) {
        if(newDraggable != null) {
            ToggleSlotBlock(true, newDraggable);
            OnSlotDrop?.Invoke(newDraggable);
            OnSlotChanged?.Invoke();
            OnSubmitted?.Invoke();
            SPUIBase.PlayConfirm();

        } else {
            SPUIBase.PlayError();
            //block.ToggleBlock()
        }


    }
    
    /*
    public virtual void OnDrop(PointerEventData eventData) {
        
        Debug.Log("OnDropSlot: " + eventData.selectedObject?.name + " selectedObject.");
        Debug.Log("OnDropSlot: " + eventData.pointerDrag?.name + " pointerDrag.");

        SPBlockDraggable newBlock = eventData.pointerDrag?.GetComponent<SPBlockDraggable>();

    }
    */

    
}
