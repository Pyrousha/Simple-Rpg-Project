using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Overworld : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float maxSpeed;
    [SerializeField] private float turnSpeed_radsPerFixedUpdate;
    [SerializeField] private float accelSpeed_ground;
    [SerializeField] private float frictionSpeed_ground;

    [Header("References")]
    private Transform target;


    private Rigidbody2D rb;

    private Vector3 dirFacing = new Vector3(0, 1, 0);
    public Vector3 DirFacing => dirFacing;

    private Vector3 velocityOnPaused;


    void Awake()
    {
        PauseController.Instance.OnPausedStateChanged_Event += OnGamePaused;

        //TODO: Make an aggro system instead of this
        target = PartyManager.Instance.GetFirstAlivePlayer().transform;

        rb = GetComponent<Rigidbody2D>();
    }

    void OnDestroy()
    {
        PauseController.Instance.OnPausedStateChanged_Event -= OnGamePaused;
    }

    void OnGamePaused(bool _newIsPaused)
    {
        if (_newIsPaused)
        {
            velocityOnPaused = rb.velocity;
            rb.velocity = Vector2.zero;
        }
        else //on unpause, set velocity back
            rb.velocity = velocityOnPaused;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PauseController.Instance.IsPaused)
            return;

        //Get gravityless velocity
        Vector3 noGravVelocity = rb.velocity;
        noGravVelocity.z = 0;

        //XZ Friction + acceleration

        Vector2 currPos = new Vector2(transform.position.x, transform.position.y);
        Vector2 targPos = new Vector2(target.position.x, target.position.y);

        Vector3 toTarget = (targPos - currPos).normalized;
        Vector3 newFacingAngle = Vector3.RotateTowards(dirFacing, toTarget, turnSpeed_radsPerFixedUpdate, 1);
        newFacingAngle.z = 0;
        dirFacing = newFacingAngle;

        Vector3 currInput = newFacingAngle;


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
        // updatedVelocity.z = rb.velocity.z;
        rb.velocity = updatedVelocity;
    }
}
