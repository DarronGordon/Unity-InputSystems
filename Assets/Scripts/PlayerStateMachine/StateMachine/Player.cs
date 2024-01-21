using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region [[[ STATE VARS ]]]
    public PlayerStateMachine StateMachine { get; private set; }

    public PlayerState CurrentState { get; private set; }
    public PlayerGroundedState GroundedState { get; private set; }
    public PlayerFlyState FlyState { get; private set; }
    #endregion

    #region [[[ COMPONENTS ]]]
    public Rigidbody rb;
    public CapsuleCollider myCollider;

    #endregion

    #region [ LOCKS ]
    public bool isLookingLocked;
    public bool isMovementLocked;
    #endregion


    #region [[[ PLAYER VARS ]]]

    [SerializeField] CharacterControlData_SO playerData;


    private Vector2 inputMoveDir;

    Vector3 playerVelocity;
    Vector3 currentVelocity;

    public bool inCrouchTransition;
    public float crouchTransitionProgress;

    #endregion


    private void Awake()
    {
        #region [[[ STATE INITIALIZATION ]]

        StateMachine = new PlayerStateMachine();

        GroundedState = new PlayerGroundedState(this, playerData, StateMachine, "Grounded");
        FlyState = new PlayerFlyState(this, playerData, StateMachine, "Fly");

        #endregion

        #region [[[ COMPONENTS ]]]

        rb = GetComponent<Rigidbody>();


        #endregion
    }
        private void Start()
        {
            StateMachine.InitializeState(GroundedState);

            StateMachine.CurrentState.DoChecks();

        }


    private void Update()
    {
        currentVelocity = rb.velocity;
        StateMachine.CurrentState.BasicUpdates();
    }

    private void FixedUpdate()
    {
        StateMachine.CurrentState.PhysicsUpdates();
     
    }

    private void LateUpdate()
    {
        StateMachine.CurrentState.LateUpdates(); 
    }

    #region Setters

    public void SetMovement(Vector3 movementDirection, float movementSpeed, bool isGrounded)
    {

        if (isMovementLocked) // CHECK CAN MOVE
            return;

        Vector3 movementDir = transform.forward * movementDirection.y + transform.right * movementDirection.x; //set movement input
        movementDir *= movementSpeed;

        if (isGrounded)
        {
            movementDir = Vector3.ProjectOnPlane(movementDir, CheckIfGrounded().normal); // project onto surface

            if (movementDir.y > 0 && Vector3.Angle(Vector3.up, CheckIfGrounded().normal) > playerData.SlopeLimit) // SLOPE LIMIT CHECK
            {
                movementDir = Vector3.zero;
            }

            CheckForStepUp(movementDir); // CHECK FOR STAIRS

            FootStepSounds();
        }
        else // IN AIR
        {
            movementDir += Vector3.down * playerData.FallingVelocity; // set rb Fall velocity
        }

        movementDir = UpdateJumping(movementDir);

        rb.velocity = Vector3.MoveTowards(rb.velocity, movementDir, playerData.Acceleration);

    }

    #endregion


    #region [[ GROUNDED CHECKS ]]

    public bool CheckIfGrounded()
    {
        Vector3 startPos = rb.position + Vector3.up * StateMachine.CurrentState.CurrentHeight * 0.5f;
        float groundCheckRadius = (playerData.Radius + playerData.GroundedCheckBuffer);

        float groundCheckDistance = (StateMachine.CurrentState.CurrentHeight * 0.5f) + playerData.GroundedCheckBuffer;

        return Physics.Raycast(startPos, Vector3.down, groundCheckDistance,
             playerData.GroundLayer, QueryTriggerInteraction.Ignore);
    }

   

    #endregion
}
