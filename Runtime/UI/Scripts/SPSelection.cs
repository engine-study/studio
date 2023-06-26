using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPSelection : MonoBehaviour
{

    [Header("Debug")]
    public Entity lastFocus;
    public Entity hoverObject;
    public GameObject realCursor;
    // void Update() {

    
    //     if(!CanInput)
    //         return;

    //     //realCursor.transform.position = Camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,10f));

    //     lastFocus = hoverObject;
    //     if(IsPointerOverUIElement) {
    //         hoverObject = null; 
    //     } else {
    //         hoverObject = GetOntFromRadius(realCursor.transform.position, .25f);
    //     }


    //     //show right click menu
    //     /*
    //     if((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) && Input.GetMouseButtonDown(1)) { //Input.GetMouseButtonDown(1)
    //         ToggleContextMenu();
    //     } else if(Input.GetMouseButtonUp(0) && contextMenuParent.activeSelf) {
    //         ToggleContextMenu();
    //     } 
    //     */
        

    //     lastItemSelected = ontSelected;

    //     if(IsPointerOverUIElement || IsDragging) {



    //     } else {

    //         //if we choose to click, pick a new selected item
    //         if(Input.GetMouseButtonDown(0)) {
    //             //itemSelected = GetItemFromRadius(realCursor.transform.position, .5f);
    //             ontSelected = hoverObject;

    //             if(ontSelected && ontSelected.IsClickable()) {

    //                 UpdateSelectUI(ontSelected);

    //                 /*
    //                 if(itemSelected.Block.IsLocal) {
    //                     ToggleUI(profileParent, true);
    //                 } 
    //                 */

    //             } else {

    //                 if(lastItemSelected != null) {
    //                     //UpdateSelectUI(SPPlayer.LocalPlayer);
    //                 }
                    
    //                 // UpdateSelectUI(SPPlayer.LocalPlayer.Core);
    //                 profile.ToggleWindowClose();
                    
    //             }

    //             PlayAccept();
    //         } 

    //         bool showHoverFlag = hoverObject && hoverObject.Interactable(); // && hoverObject.Block != null && hoverObject.Block.IsValid;
    //         SPCursorTexture.UpdateCursor( showHoverFlag ? SPCursorState.Hand : SPCursorState.Default); 

    //         UpdateHoverUI(hoverObject?.Core);

    //     }
       
        
    //     //update the highlighted/focus object if we don't have something selected
    //     //UpdateHoverUI(hoverObject);
    // }

    Collider [] hits;

    Entity GetOntFromRadius(Vector3 position, float radius) {

        if(hits == null) { hits = new Collider[10]; }

        int amount = Physics.OverlapSphereNonAlloc(realCursor.transform.position, radius, hits, LayerMask.NameToLayer("Nothing"), QueryTriggerInteraction.Collide);
        int selectedItem = -1;
        float minDistance = 999f;
        Entity bestItem = null;

        for(int i = 0; i < amount; i++) {
            Entity checkItem = hits[i].GetComponentInParent<Entity>();

            if(!checkItem)
                continue;

            float distance = Vector3.Distance(realCursor.transform.position,checkItem.transform.position);
            if(distance < minDistance) {
                minDistance = distance;
                selectedItem = i;
                bestItem = checkItem;
            }
        }

        return bestItem;

    }

    //  void LateUpdate() {

    //     Cursor.visible = true; 
    //     //Cursor.visible = contextMenuParent.activeSelf || itemSelected; 

    //     //scale on click
    //     cursorHover.Cursor.localScale = Vector3.Lerp(cursorHover.Cursor.localScale, Input.GetMouseButton(0) ? Vector3.one * .8f : Vector3.one, .3f); 

    //     Vector2 hoverPosition = hoverObject ? Camera.WorldToScreenPoint(hoverObject.Center) : Input.mousePosition; //camera.ScreenToViewportPoint(Input.mousePosition);
    //     Vector2 selectionPosition = Input.mousePosition; //camera.ScreenToViewportPoint(Input.mousePosition);

    //     //hover position
    //     hoverPosition = ScreenToCanvas(hoverPosition, CursorParent);
    //     cursorHover.Rect.anchoredPosition = hoverPosition;

    //     //selected position
    //     if(ontSelected) {
    //         selectionPosition = Camera.WorldToScreenPoint(ontSelected.Center);
    //         selectionPosition = ScreenToCanvas(selectionPosition, CursorParent); 

    //         cursorSelect.Rect.anchoredPosition = selectionPosition;
    //     } else {
    //         cursorSelect.Rect.anchoredPosition = hoverPosition;
    //     }

    //     //Vector2 direction = ((Vector2)itemLink.transform.position - (Vector2)itemSelected.transform.position).normalized;
    //     //itemSelection.transform.position = itemSelected.transform.position;
    //     //line.SetPosition(0,itemSelected.transform.position + (Vector3)(direction * itemSelection.transform.localScale.x));
    //     //line.SetPosition(1,itemLink.transform.position - (Vector3)(direction * itemSelection.transform.localScale.x));


    //     /*
    //     if(resolutionText.gameObject.activeSelf) {
    //         resolutionText.text = Screen.width + "x" + Screen.height + (dither.enabled ? "[D]" : "");
    //     }
    //     */

    //     fakeSubmit = false; 
    // }
}
