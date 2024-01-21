using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerState
{
    protected Player player;
    protected PlayerStateMachine stateMachine;
    protected string stateName;

    protected bool isPlayerJumpCheck;

    protected CharacterControlData_SO playerData;
    protected float startTime;
    protected bool isExitingState;


    #region [ SHARED VARS ]

    public Rigidbody rb;
    public CapsuleCollider myCollider;

    public bool IsGrounded;
    public bool IsSprinting;
    public bool IsJumping;
    public bool IsCrouched;

    #endregion

    #region [ CROUCHING VARS ]

    private bool inCrouchTransition;
    private bool targetCrouchState;
    private float crouchTransitionProgress;

    #endregion
    protected Vector3 movementDirection;
    public float currentMaxSpeed
    {
        get
        {
            if (IsGrounded)
            {
                return (IsSprinting ? playerData.RunSpeed : playerData.WalkSpeed) * (IsCrouched ? playerData.CrouchingSpeedMultiplier : 1f);

            }

            return playerData.CanAirControl ? playerData.AirControlMaxSpeed : 0f;
        }
        set
        {
            throw new System.NotImplementedException("Current Speed cannot be set directly Update scriptable object data");
        }
    }

    public float CurrentHeight
    {
        get
        {
            if (inCrouchTransition)
            {
                return Mathf.Lerp(playerData.CrouchingHeight, playerData.Height, crouchTransitionProgress);
            }

            return IsCrouched ? playerData.CrouchingHeight : playerData.Height;
        }
        set
        {
            throw new System.NotImplementedException("Current height cannot be set directly Update scriptable object data");
        }
    }

    public PlayerState(Player player, CharacterControlData_SO playerData, PlayerStateMachine stateMachine, string stateName)
    {
        this.player = player;
        this.playerData = playerData;
        this.stateMachine = stateMachine;
        this.stateName = stateName;
    }

    public virtual void Enter()
    {
        DoChecks();
        startTime = Time.time;

        Debug.Log(stateName);

        isExitingState = false;

    }

    public virtual void Exit()
    {
        isExitingState = true;
    }
    public virtual void DoChecks()
    {

    }

    public virtual void BasicUpdates()
    {
        DoChecks();

        movementDirection = DaxDazCharacterControler_InputManager.Instance.movementDirInput;
    }


    public virtual void PhysicsUpdates()
    {
        IsGrounded = player.CheckIfGrounded();
    }

    public virtual void LateUpdates()
    {

    }

    public virtual RaycastHit IsGroundedCheckFixedUpdate()
    {
        IsCrouched = false;
        IsGrounded = false;

        return new RaycastHit();
    }
    public virtual void AnimTrigg() { }

}