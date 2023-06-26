using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPControllerLite : SPController
{

    [Header("Lite")]
    public Vector3 euler;
    public Vector3 desiredVelocity;
    public bool flying;
    public override void Init()
    {
        base.Init();

        rb.isKinematic = false;
        rb.useGravity = !flying;
    }

    public override void Move()
    {
        base.Move();
      
        //simple version
        //player.transform.forward = logic.InputVector;

        if(bumped || Logic.InputVector == Vector3.zero) {

            lookAt = transform.forward;
            lookAt.y = 0;
            // rb.velocity *= .9f;

        } else {

            lookAt = Logic.InputVector;
            
            desiredVelocity = Logic.InputVector;
            desiredVelocity.y = 0f;

            rb.AddForce(desiredVelocity * 2f - rb.velocity, ForceMode.Impulse);
            // rb.velocity = logic.InputVector + Vector3.up * Physics.gravity.y;

        }

        rb.AddForce(Physics.gravity, ForceMode.Acceleration);

        // lookAt = (Camera.main.transform.position - transform.position).normalized;

        //45 degree angle clamp
        euler = Quaternion.LookRotation(lookAt).eulerAngles;
        
        euler.x = Mathf.Round(euler.x / 40) * 40;
        euler.y = Mathf.Round(euler.y / 40) * 40;
        euler.z = Mathf.Round(euler.z / 40) * 40;

        // rb.rotation = Quaternion.Euler(euler);        
        transform.eulerAngles = euler;
        rb.angularVelocity = Vector3.zero;

    }

    public override void ToggleController(bool toggle)
    {
        base.ToggleController(toggle);

        controller.enabled = false; 
    }


}
