// original by Eric Haines (Eric5h5)
// adapted by @torahhorse
// http://wiki.unity3d.com/index.php/FPSWalkerEnhanced

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class SPControllerHuman : SPController
{

    public static float walkSpeed = 5.0f;
    public static float runSpeed = 8f;
    public static float diagonalLimit = .7071f; 
    public bool Grounded {get{return grounded;}}

    [Header("Player")]
    public float slowSpeed = 1.5f;
    public float noClipSpeedMultiplier = 1.0f; 
    public float platformSpeedMultiplier = 1.0f; 
    // If true, diagonal speed (when strafing + moving forward or back) can't exceed normal move speed; otherwise it's about 1.4 times faster
    private bool limitDiagonalSpeed = true;
    public bool enableRunning = false;
    float jumpSpeed = 7.5f;
    bool slowJumpFlag = false; 
    bool running = false; 
    bool wasGrounded; 


    // Units that player can fall before a falling damage function is run. To disable, type "infinity" in the inspector
    private float fallingDamageThreshold = 10.0f;

    // If the player ends up on a slope which is at least the Slope Limit as set on the character controller, then he will slide down
    public bool slideWhenOverSlopeLimit = true;

    // If checked and the player is on an object tagged "Slide", he will slide down it regardless of the slope limit
    public bool slideOnTaggedObjects = false;

    public float slideSpeed = 5.0f;

    // Small amounts of this results in bumping when walking down slopes, but large amounts results in falling too fast
    public float antiBumpFactor = .75f;
    // Player must be grounded for at least this many physics frames before being able to jump again; set to 0 to allow bunny hopping
    public static float antiBunnyHopLength = 0.05f;
    public static float antiBunnyHopFactor = 0f;
    public float bunnyHopCollisionSlow = 1f;

   
    public override void Init() {
        
        if(hasInit) {
            return;
        }

        base.Init();

        jumpTimer = antiBunnyHopLength;
        rb.isKinematic = true;

    }
    
    public float TopSpeed {get{return maxAirAccel;}}
    float hoverSpeed = -1.5f;
  

    public override void Move() {

        base.Move();

        // If both horizontal and vertical are used simultaneously, limit speed (if allowed), so the total doesn't exceed normal move speed
        float inputModifyFactor = 1f; //(inputX != 0.0f && inputZ != 0.0f && limitDiagonalSpeed) ? diagonalLimit : 1.0f;
 
        forceVelocity *= .95f;

        if (grounded) {


            bool sliding = false;
            // See if surface immediately below should be slid down. We use this normally rather than a ControllerColliderHit point,
            // because that interferes with step climbing amongst other annoyances
            if (Physics.Raycast(Transform.position, -Vector3.up, out hit, rayDistance)) {
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
            // However, just raycasting straight down from the center can fail when on steep slopes
            // So if the above raycast didn't catch anything, raycast down from the stored ControllerColliderHit point instead
            else {
                Physics.Raycast(contactPoint + Vector3.up, -Vector3.up, out hit);
                if (Vector3.Angle(hit.normal, Vector3.up) > slideLimit)
                    sliding = true;
            }
 
            // If we were falling, and we fell a vertical distance greater than the threshold, run a falling damage routine
            if (falling) {
                falling = false;
                if (Transform.position.y < fallStartLevel - fallingDamageThreshold)
                    FallingDamageAlert (fallStartLevel - Transform.position.y);
            }
 
            if( enableRunning) {
                //slowJumpFlag = (Input.GetKey(KeyCode.LeftShift) || crouching);
            	//speed = slowJumpFlag ? slowSpeed * platformSpeedMultiplier : walkSpeed * platformSpeedMultiplier;

                running = Input.GetKey(KeyCode.LeftShift);
            	speed = running ? runSpeed * platformSpeedMultiplier : walkSpeed * platformSpeedMultiplier;
            }
 
            // If sliding (and it's allowed), or if we're on an object tagged "Slide", get a vector pointing down the slope we're on
            if ( (sliding && slideWhenOverSlopeLimit) || (slideOnTaggedObjects && hit.collider.tag == "Slide") ) {
                Vector3 hitNormal = hit.normal;
                moveDirection = new Vector3(hitNormal.x, -hitNormal.y, hitNormal.z);
                Vector3.OrthoNormalize (ref hitNormal, ref moveDirection);
                moveDirection *= slideSpeed;
                playerControl = false;
            }
            // Otherwise recalculate moveDirection directly from axes, adding a bit of -y to avoid bumping down inclines
            else {
                moveDirection = new Vector3(Logic.InputVector.x * inputModifyFactor * speed, -antiBumpFactor, Logic.InputVector.z * inputModifyFactor * speed);
                playerControl = true;
            }

            //Finish jumping when we aren't holding space
            if (!Logic.jumpFlag || jumpTimer < antiBunnyHopFactor) {  
                jumpTimer += Time.fixedDeltaTime;
                ResetJump();

                if(wasGrounded == false && bhopping)
                    FinishBhop();
            }
            //JUMP
            else if (Logic.jumpFlag && !inWater && !crouching) {
                Jump(); 
            } else if(!inWater) {
                ResetJump(); 
            }
        }
        
        if(!grounded) {

            speed = slowJumpFlag ? slowSpeed * platformSpeedMultiplier : walkSpeed * platformSpeedMultiplier;

            // If we stepped over a cliff or something, set the height at which we started falling
            if (!falling) {
                falling = true;
                fallStartLevel = Transform.position.y;
            }

            airAccel = Mathf.Clamp(airAccel * Logic.InputVector.z, minAirAccel,TopSpeed);

            moveDirection.x = Logic.InputVector.x * speed * inputModifyFactor;
            
            //moveDirection.z = Mathf.Clamp(speed * airAccel * inputModifyFactor, speed * inputModifyFactor ,Mathf.Infinity);
            moveDirection.z = Mathf.Clamp( Logic.InputVector.z * speed * airAccel * inputModifyFactor, Logic.InputVector.z * speed * inputModifyFactor ,Mathf.Infinity);
            moveDirection.y += Gravity * Time.fixedDeltaTime;

            // rb.AddForce(Physics.gravity, ForceMode.Acceleration);
        }

        //Apply tagging values
        if (!noclip) {
            moveDirection.x *= tagging;
            moveDirection.z *= tagging;
        }

        /* 
        if(transform.position.y < de_gamemode.deathHeight) {
            de_player.MasterLocal.KillLocalPlayer(DeathCause.Falling);
            return; 
        }
        */

        if(Logic.crouchFlag != crouching) {
            ScaleCapsuleForCrouching(crouching);
        }

        //player.playerCollider.enabled = true; 

        // check if we're in noclip mode
        if (!noclip && !inWater) {

            //moveDirection = myTransform.TransformDirection(moveDirection);
            moveDirection.y = moveDirection.y + rb.velocity.y; //+ Gravity * Time.fixedDeltaTime
            // moveDirection.y = Mathf.Max(moveDirection.y - Gravity * Time.fixedDeltaTime, -Gravity);

            // if(Logic.hoverFlag && moveDirection.y < 0) {moveDirection.y = Mathf.Max(moveDirection.y,hoverSpeed);}

            Vector3 moveDelta = moveDirection * Time.fixedDeltaTime;
            moveDelta += forceVelocity * Time.fixedDeltaTime;

            SetDetectCollisions(detectCollisions);

            // Move the controller, and set grounded true or false depending on whether we're standing on something
            if(detectCollisions) {
                wasGrounded = grounded; 
                grounded = (controller.Move(moveDelta) & CollisionFlags.Below) != 0;

                // grounded = false;
                // rb.MovePosition(rb.position + moveDirection);

                if(!wasGrounded && grounded) {
                    ToggleJump(false);
                }

                if (controller.collisionFlags == CollisionFlags.CollidedSides) {
                    airAccel -= bunnyHopCollisionSlow * Time.fixedDeltaTime;
                }
            } else {
                transform.position += moveDelta;
            }

        } else if(noclip) {

            airAccel = 1f; 
            SetDetectCollisions(false);

            moveDirection.y = 0f;

            //set to camera normals 
            //moveDirection = Camera.main.transform.TransformDirection(moveDirection);
            //let players use Q and E to go up and down 
            moveDirection += Vector3.up * Logic.InputVector.y * speed;
            moveDirection.y = Mathf.Clamp(moveDirection.y, -speed, speed);
            moveDirection *= noClipSpeedMultiplier;

            transform.position += moveDirection * Time.fixedDeltaTime;
           
        } else if(inWater) {

            //help players falling from gravity with no airaccel to gain it through their downward momentum
            if (!inWater) {
                airAccel = Mathf.Max(Mathf.Abs(moveDirection.y / speed/ inputModifyFactor), airAccel);
                airAccel = Mathf.Clamp(airAccel, minAirAccel, TopSpeed); 
                inWater = true; 
            }

            SetDetectCollisions(detectCollisions);

            //moveDirection = Camera.main.transform.TransformDirection(moveDirection);
            moveDirection.y = 0f;
            moveDirection.y = Mathf.Max(moveDirection.y + Gravity * Time.fixedDeltaTime, +Gravity);

            Vector3 moveDelta = moveDirection * Time.fixedDeltaTime;
            moveDelta += forceVelocity * Time.fixedDeltaTime;

            grounded = (controller.Move(moveDelta) & CollisionFlags.Below) != 0;

            if (controller.collisionFlags != CollisionFlags.None) {
                airAccel -= bunnyHopCollisionSlow * Time.fixedDeltaTime;
            }
        }




        velocity = controller.velocity; 

        moveDirectionNoY = moveDirection;
        moveDirectionNoY.y = 0f; 

        if(moveDirectionNoY != Vector3.zero) {
            transform.LookAt(transform.position + moveDirectionNoY);
        }

    }

    

    public void ToggleJump(bool toggle) {

    }

    public void SetGrounded(bool toggle) {
        grounded = toggle;
    }

    public void Launch(float ySpeed, float zSpeed) {
        grounded = false; 
        moveDirection.y = Mathf.Max(moveDirection.y, ySpeed);// Mathf.Max( FirstPersonDrifter.instance.moveDirection.y,0f) + ySpeed;
        airAccel =  Mathf.Max(airAccel, Mathf.Clamp(airAccel + zSpeed,minAirAccel,TopSpeed));

        if(zSpeed > 0)
            largestInputY = 1f;
    }
    public void Jump() {
        
        if(slowJumpFlag)
            ResetJump();

        //this increases bhop speed on jump
        if (jumpCount != 0 && slowJumpFlag == false && canBhop) {
            airAccel += bhopBoost;
            airAccel = Mathf.Clamp(airAccel, 1f, TopSpeed);
            ToggleJump(true);

        } else {
            ToggleJump(false);
        }
        
        grounded = false; 
        largestInputY = Logic.InputVector.z;

        jumpCount++;
        jumpTimer = 0;

        moveDirection.y = jumpSpeed;

    }

    void FinishBhop() {
        ToggleJump(false);
        ResetJump();
    }


    void ResetJump() {
        airAccel = 1f;
        jumpCount = 0;

    }

    // Store point that we're in contact with for use in FixedUpdate if needed
    void OnControllerColliderHit (ControllerColliderHit hit) {

        contactPoint = hit.point;

        Rigidbody body = hit.collider.attachedRigidbody;

        
        if (body != null && !body.isKinematic) {
            //body.velocity += controller.velocity;
            body.AddForce((body.position - hit.point).normalized * controller.velocity.magnitude, ForceMode.Impulse);
        }
 
    }

    // If falling damage occured, this is the place to do something about it. You can make the player
    // have hitpoints and remove some of them based on the distance fallen, add sound effects, etc.
    void FallingDamageAlert (float fallDistance)
    {
        //print ("Ouch! Fell " + fallDistance + " units!");   
    }
    const float k_Half = 0.5f;
    void ScaleCapsuleForCrouching(bool crouch) {

        if (grounded && crouch && !hasCrouched) {

            controller.height = controller.height * .6f;
            controller.center = controller.center * .6f;

            hasCrouched = true;
            
        }
        else if(!crouch && hasCrouched){

            /*
            Ray crouchRay = new Ray(transform.position + Vector3.up * controller.radius * k_Half, Vector3.up); //transform paramater used to be m_Rigidbody
            float crouchRayLength = m_CapsuleHeight - controller.radius * k_Half;
            
            if (Physics.SphereCast(crouchRay, controller.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                hasCrouched = true;
                return;
            }*/
            controller.height = m_CapsuleHeight;
            controller.center = m_CapsuleCenter;

            hasCrouched = false;
        }
     
        crouching = crouch;

    }

    void PreventStandingInLowHeadroom() {
        // prevent standing up in crouch-only zones
        if (!crouching) {
            Ray crouchRay = new Ray(transform.position + Vector3.up * controller.radius * k_Half, Vector3.up); //transform paramater used to be m_Rigidbody
            float crouchRayLength = m_CapsuleHeight - controller.radius * k_Half;
            if (Physics.SphereCast(crouchRay, controller.radius * k_Half, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore)) {
                crouching = true;
            }
        }
    }

    public void SetBhop(bool toggle) {
        if(toggle) {
            antiBunnyHopFactor = -1f; 
        } else {
            antiBunnyHopFactor = antiBunnyHopLength; 
        }
    }

}