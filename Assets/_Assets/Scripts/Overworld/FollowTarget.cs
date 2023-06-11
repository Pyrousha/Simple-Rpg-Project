using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;

    [Header("References")]
    [SerializeField] private Transform target;
    [SerializeField] private float sqrTargDistAway;


    private Rigidbody rb;

    private Vector3 dirFacing = new Vector3(0, 1, 0);
    public Vector3 DirFacing => dirFacing;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Get gravityless velocity
        Vector3 noGravVelocity = rb.velocity;
        noGravVelocity.z = 0;

        //XZ Friction + acceleration

        Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targPos = new Vector2(target.position.x, target.position.y);

        Vector3 currInput;
        if (Vector2.SqrMagnitude(currPos - targPos) <= sqrTargDistAway)
        {
            //Close enough, no need to move
            currInput = Vector3.zero;
        }
        else
        {
            //Move towards target
            currInput = (targPos - currPos).normalized;

            dirFacing = currInput;
        }


        //Apply ground fricion
        Vector3 velocity_local_friction = noGravVelocity.normalized * Mathf.Max(0, noGravVelocity.magnitude - frictionSpeed_ground);

        Vector3 updatedVelocity = velocity_local_friction;

        if (currInput.magnitude > 0.05f) //Pressing something, try to accelerate
        {
            Vector3 velocity_friction_and_input = velocity_local_friction + currInput * accelSpeed_ground;

            if (velocity_local_friction.magnitude <= maxSpeed)
            {
                //under max speed, accelerate towards max speed
                updatedVelocity = velocity_friction_and_input.normalized * Mathf.Min(velocity_friction_and_input.magnitude, maxSpeed);
            }
            else //Over max speed
            {
                if (velocity_friction_and_input.magnitude <= maxSpeed) //Input below max speed
                    updatedVelocity = velocity_friction_and_input;
                else
                {
                    //Can't use input directly, since would be over max speed

                    //Would accelerate more, so don't user player input
                    if (velocity_friction_and_input.magnitude > velocity_local_friction.magnitude)
                        updatedVelocity = velocity_local_friction;
                    else
                        //Would accelerate less, user player input (since input moves velocity more to 0,0 than just friciton)
                        updatedVelocity = velocity_friction_and_input;
                }
            }
        }

        //Apply velocity
        updatedVelocity.z = rb.velocity.z;
        rb.velocity = updatedVelocity;
    }
}
