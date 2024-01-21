using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{

    private bool jumpInput;

    private float timeSinceLastStepAudio = 0f;

    #region [ JUMPING VARS ]

    float jumpTimeRemaining;
    private bool IsJumping = false;
    private int JumpCount = 0;
    private float timeInAir = 0f;
    private float originalDrag;

    #endregion



    public PlayerGroundedState(Player player, CharacterControlData_SO playerData, PlayerStateMachine stateMachine, string animBoolName) : base(player, playerData, stateMachine, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();


    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void BasicUpdates()
    {
        base.BasicUpdates();
        
    }

    public override void PhysicsUpdates()
    {
        base.PhysicsUpdates();


        timeInAir = IsGrounded ? 0f : (Time.deltaTime + timeInAir);// tracks how long in air

        player.SetMovement(currentMaxSpeed , movementDirection);

    }

    public override RaycastHit IsGroundedCheckFixedUpdate()
    {
        return base.IsGroundedCheckFixedUpdate();
    }

    #region [[ MOVEMENT ]]
    public void SetMovementLock()
    {
        player.isMovementLocked = true;
    }

    

    #endregion

    #region [[ CROUCHING ]]

    private void UpdateCrouch()
    {
        if (state.isMovementLocked || state.isLookingLocked || !characterCtrlData.CanCrouch)
        {
            return;
        }

        if (state.IsJumping || !state.IsGrounded)
        {
            if (state.IsCrouched || targetCrouchState)
            {
                targetCrouchState = false;
                inCrouchTransition = true;
            }
        }
        else if (characterCtrlData.CrouchingToggle)
        {
            if (PlayerInputCtrl.Instance._CrouchInput) // Toggle crouch Setting
            {

                PlayerInputCtrl.Instance._CrouchInput = false;

                targetCrouchState = !targetCrouchState;
                inCrouchTransition = true;
            }
        }
        else
        {
            if (PlayerInputCtrl.Instance._CrouchInput != targetCrouchState) // crouch state change
            {
                targetCrouchState = PlayerInputCtrl.Instance._CrouchInput;
                inCrouchTransition = true;
            }
        }

        if (inCrouchTransition)
        {
            crouchTransitionProgress = Mathf.MoveTowards(crouchTransitionProgress, targetCrouchState ? 0 : 1f, Time.deltaTime / characterCtrlData.CrouchingTransitionTime);


            state.myCollider.height = CurrentHeight; // CAMERA AND COLLIDER UPDATE
            state.myCollider.center = Vector3.up * CurrentHeight * 0.5f;
            myCamera.transform.localPosition = Vector3.up * (CurrentHeight + characterCtrlData.VerticalOffset);

            if (Mathf.Approximately(crouchTransitionProgress, targetCrouchState ? 0f : 1f))// is crouch finished
            {
                state.IsCrouched = targetCrouchState;
                inCrouchTransition = false;
            }
        }
    }

    #endregion

    #region [[ SPRINTING ]]
    private void UpdateSprinting(RaycastHit groundCheck)
    {

        if (PlayerInputCtrl.Instance._MoveInputDir.magnitude < float.Epsilon) // Stop running if no input
        {
            state.IsSprinting = false;
        }

        if (!state.IsGrounded) // can only sprint on the ground
        {
            state.IsSprinting = false;
            return;
        }

        if (!characterCtrlData.CanRun) // check so if we can sprint
        {
            state.IsSprinting = false;
            return;
        }

        state.IsSprinting = PlayerInputCtrl.Instance._RunInput;
    }

    #endregion

    #region [[ GROUNDED CHECKS ]]

    public RaycastHit GroundedCheck()
    {
        RaycastHit groundCheckResult = UpdateIsGrounded();

        bool wasGrounded = player.isGrounded;
        bool wasSprinting = player.isSprinting;

        UpdateSprinting(groundCheckResult);


        if (wasSprinting != player.isSprinting)
        {
           // OnSprintChanged.Invoke(isSprinting);
        }



        return groundCheckResult;
    }

    RaycastHit UpdateIsGrounded()
    {
        RaycastHit hitResult;

        if (jumpTimeRemaining > 0f)
        {
            player.isGrounded = false;
            return new RaycastHit();
        }

        Vector3 startPos = player.rb.position + Vector3.up * player.CurrentHeight * 0.5f;
        float groundCheckRadius = (playerData.Radius + playerData.GroundedCheckBuffer);
        float groundCheckDistance = (player.CurrentHeight * 0.5f) + playerData.GroundedCheckBuffer;

        Debug.DrawRay(startPos, Vector3.down, Color.red, groundCheckDistance);
        if (Physics.Raycast(startPos, Vector3.down, out hitResult, groundCheckDistance,
             playerData.GroundLayer, QueryTriggerInteraction.Ignore))
        {

            isGrounded = true;
            JumpCount = 0;
            jumpTimeRemaining = 0f;

            //AUTO PARENTING FOR MOVING-PLATFORMS LIKE ELEVATORS ECT...

        }

        else

            isGrounded = false;

        return hitResult;
    }

    #endregion

    #region [[ JUMPING ]]
    private Vector3 UpdateJumping(Vector3 movementDir)
    {
        bool triggeredJumpThisFrame = false;

        if (PlayerInputCtrl.Instance._JumpInput)
        {
            PlayerInputCtrl.Instance._JumpInput = false;
            // JUMP CHECKS
            bool triggerJump = true;
            int numberJumpsPermitted = characterCtrlData.CanDoubleJump ? 2 : 1;

            if (JumpCount >= numberJumpsPermitted)
            {
                triggerJump = false;
            }

            if (!state.IsGrounded && !state.IsJumping)
            {
                triggerJump = false;
            }

            //CAN WE JUMP
            if (triggerJump)
            {
                //have we used all our jumps?
                if (JumpCount == 0)
                {

                    triggeredJumpThisFrame = true;
                }

                state.rb.drag = 0f;
                jumpTimeRemaining += characterCtrlData.JumpTime;
                state.IsJumping = true;
                state.myCollider.material = characterCtrlData.MaterialJumping;
                ++JumpCount;

                //DaxCharacterControlerEventHandler.Instance.CallOnJumpEvent();
                OnJump.Invoke(state.rb.position);
            }

        }

        if (state.IsJumping)
        {
            // upadate remaining jump time if not jumping this frame
            if (!triggeredJumpThisFrame)
            {
                jumpTimeRemaining -= Time.deltaTime;
            }

            //Jumping finished
            if (jumpTimeRemaining <= 0)
            {
                state.IsJumping = false;
            }
            else
            {
                movementDir.y = characterCtrlData.JumpVelocity;
            }

        }
        return movementDir;
    }
    #endregion
}
