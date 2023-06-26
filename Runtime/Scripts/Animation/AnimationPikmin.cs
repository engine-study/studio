using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPikmin : SPAnimation
{

    float distanceTraveled;
    float timeIdle = 0f; 
    float idleStart = 2f;
    float startIdling;
    Vector3 position;

    protected override void UpdateVisuals()
    {
        base.UpdateVisuals();


        float sinAmount = Mathf.PI;
        velocityMagnitude = Mathf.Clamp01(velocityMagnitude - .2f);

        if(velocityMagnitude == 0f) {

            if(timeIdle == 0f) {
                startIdling = Random.Range(idleStart, idleStart * 2f);
            }

            timeIdle += Time.deltaTime;

            //animate idle
            if(timeIdle > startIdling) {
                distanceTraveled += 20f * Time.deltaTime;
            }

        } else {

            if(timeIdle > startIdling) {
                player.resources.fx_spawn.Play();
            }

            timeIdle = 0f;

            distanceTraveled += velocityMagHorizontal * .15f;

            if(grounded) {
                // distanceTraveled += velocityMagHorizontal * .15f;
            } else {

            }

        }

        sinAmount = distanceTraveled;

        position = Vector3.Lerp(Vector3.zero, Vector3.up * (Mathf.Sin(sinAmount) + 1f) * .15f, sinAmount * 10f);
        player.Visual.transform.localPosition = position;


        //45 degree angle clamp
        
        // Vector3 euler = Quaternion.LookRotation((Camera.main.transform.position - transform.position).normalized).eulerAngles;

        Vector3 euler = Camera.main.transform.rotation.eulerAngles;
        // Vector3 euler = Quaternion.Lerp(Camera.main.transform.rotation, Quaternion.Inverse(Root.rotation),.33f).eulerAngles;
        // Vector3 euler = Root.root.eulerAngles;

        euler.x = Mathf.Round(euler.x / 50) * 50;
        euler.y = Mathf.Round(euler.y / 50) * 50;
        euler.z = Mathf.Round(euler.z / 50) * 50;

        player.Visual.transform.rotation = Quaternion.Euler(euler);
    }
}
