using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPLogicFollower : SPLogic
{
    float distanceCheck;
    float distance;
    float minDistance = 2.5f; 
    float maxDistance = 7.5f; 
    public override void UpdateInput()
    {
        base.UpdateInput();

        distanceCheck += Time.deltaTime;

        if(distanceCheck > .5f) {

            Vector3 target = SPPlayer.LocalPlayer ? SPPlayer.LocalPlayer.Root.position : transform.position;

            distanceCheck = 0f;
            distance = Vector3.Distance(target, player.Root.position);

            if(distance > minDistance && distance < maxDistance) {
                inputRaw = (target - player.Root.position) ;
                // inputRaw.y = 0;
                inputRaw = inputRaw.normalized * Mathf.Clamp(distance, minDistance * 1.5f, maxDistance * .75f);
            } else {
                inputRaw = Vector3.zero;
            }

        }
      
        input = inputRaw;

    }
}
