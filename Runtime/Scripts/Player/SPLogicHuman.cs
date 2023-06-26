using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLogicHuman : SPLogic {
     
    protected bool mouseInput = true; 
    protected bool keyInput = true; 

    [SerializeField] protected bool mouseInputting = false; 

    public override void UpdateInput() {
        
        base.UpdateInput();

        //if control is taken away from the player, this simulates them slowing down 
        input = Vector3.MoveTowards(input, Vector3.zero, Time.deltaTime);

        if(!SPUIBase.CanInput) {
            return;
        }
    

        inputRaw = Vector3.zero;
        
        // int mouseButton = SPGlobal.IsMobile ? 0 : 1;
        mouseInputting = SPUIBase.CanInput && !SPUIBase.IsPointerOverUIElement && mouseInput && (Input.GetMouseButton(0) || Input.GetMouseButton(1));

        if(mouseInputting) {
            
            inputRaw = SPInput.MousePlanePos - transform.position;
            //inputRaw.y = 0; 
            inputRaw = inputRaw.normalized;
            
        } else if(keyInput) {

            inputRaw.x = Input.GetAxis("Horizontal");
            inputRaw.z = Input.GetAxis("Vertical");
            
            inputRaw.y = Input.GetAxis("NoClipVertical");
            inputRaw = SPHelper.CartesianToWorld(inputRaw);

            inputRaw = inputRaw.normalized;
        }

        //CROUCHING
        if (Input.GetKey(KeyCode.LeftControl)) {
            crouchFlag = true;
        } else {
            crouchFlag = false;
        }

        jumpFlag = Input.GetKey(KeyCode.Space) || Input.GetKeyDown(KeyCode.Space);
        hoverFlag = Input.GetKey(KeyCode.Space);

        input = inputRaw;
    }

    
    // public override void UpdateInput() {
    //     base.UpdateInput();

    //     if (!Active && SPUIBase.CanInput) {
    //         moveZorb = false; 
    //         jump = false; 
    //         inputRaw = Vector3.zero;
    //         return;
    //     }

    //     moveZorb = Input.GetMouseButton(1); //&& !SPUIBase.IsPointerOverUIElement;
    //     jump = Input.GetKeyDown(KeyCode.Space);

    //     inputRaw = Vector3.zero;

    //     inputRaw.x = Input.GetAxisRaw("Horizontal");
    //     inputRaw.z = Input.GetAxisRaw("Vertical");

    //     inputRaw = SPHelper.LocalToCamera(inputRaw); //SPHelper.CartesianToWorld(inputRaw.normalized);

    //     if(Input.GetKeyDown(KeyCode.LeftShift)) {
    //         inputRaw *= 5f;
    //     }

    //     // if(Input.GetKey(KeyCode.W)) {
    //     //     inputRaw = Vector3.forward;
    //     // } if(Input.GetKey(KeyCode.S)) {
    //     //     inputRaw = Vector3.back;
    //     // } if(Input.GetKey(KeyCode.A)) {
    //     //     inputRaw = Vector3.left;
    //     // } if(Input.GetKey(KeyCode.D)) {
    //     //     inputRaw = Vector3.right;
    //     // }

    // }


}
