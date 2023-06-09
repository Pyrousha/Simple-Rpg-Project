using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorController_2DTopDown : MonoBehaviour
{
    [field: SerializeField] public Animator Anim { get; private set; }
    private Rigidbody2D rb;
    private PlayerController_2D_TopDown playerController;

    private bool isPlayer = false;

    [SerializeField] private float idleCutoffSpeed = 0.1f;

    public enum MoveStateEnum
    {
        UpIdle,
        DownIdle,
        LeftIdle,
        RightIdle,
        Up,
        Down,
        Left,
        Right
    }
    public MoveStateEnum State { get; private set; }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController_2D_TopDown>();
        if (playerController != null)
            isPlayer = true;
    }

    void FixedUpdate()
    {
        Vector3 dirFacing;
        if (!isPlayer)
            dirFacing = rb.velocity;
        else
            dirFacing = playerController.DirFacing;

        if (dirFacing.magnitude >= idleCutoffSpeed)
        {
            //Moving
            if (Mathf.Abs(dirFacing.x) * 1.1f >= Mathf.Abs(dirFacing.y))
            {
                //play horizontal move animation
                if (dirFacing.x > 0)
                    ChangeAnimationState(MoveStateEnum.Right);
                else
                    ChangeAnimationState(MoveStateEnum.Left);
            }
            else
            {
                //play vertical move animation
                if (dirFacing.y > 0)
                    ChangeAnimationState(MoveStateEnum.Up);
                else
                    ChangeAnimationState(MoveStateEnum.Down);
            }
        }
        else
        {
            //Not moving
            switch (State)
            {
                case MoveStateEnum.Up:
                    ChangeAnimationState(MoveStateEnum.UpIdle);
                    break;
                case MoveStateEnum.Down:
                    ChangeAnimationState(MoveStateEnum.DownIdle);
                    break;
                case MoveStateEnum.Left:
                    ChangeAnimationState(MoveStateEnum.LeftIdle);
                    break;
                case MoveStateEnum.Right:
                    ChangeAnimationState(MoveStateEnum.RightIdle);
                    break;
            }
        }
    }

    private void ChangeAnimationState(MoveStateEnum _newState)
    {
        if (State == _newState)
            return;

        State = _newState;
        Anim.Play(_newState.ToString(), 0);
    }
}
