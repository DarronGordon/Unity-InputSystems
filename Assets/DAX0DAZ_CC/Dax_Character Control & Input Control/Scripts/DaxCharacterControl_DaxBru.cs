
using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

    public class DaxCharacterControl_DaxBru : MonoBehaviour
    {

        #region [[[[[ VARS ]]]]]

        [SerializeField] CharacterControlData_SO characterCtrlData;

        #region [ COMPONENTS ]

        private Rigidbody rb;
        private CapsuleCollider myCollider;

        #endregion

        #region [[[ MOVEMENT VARS ]]]

        private Vector2 inputMoveDir;
        [SerializeField] public bool IsGrounded { get; protected set; } = true;

        #region [ LOCKS ]
        public bool isLookingLocked { get; protected set; }
        public bool isMovementLocked { get; protected set; }
        #endregion

        #region [ CAMERA ]

        [SerializeField] private Transform myCamera;
        private float currentCamPitch = 0f;
        #endregion

        #region [ CROUCHING ]
        public bool isCrouching { get; protected set; } = false;
        public bool inCrouchTransition { get; protected set; } = false;
        public bool targetCrouchState { get; protected set; } = false;
        public float crouchTransitionProgress { get; protected set; } = 1f;

        #endregion

        #region [ SPRINTING ]

        public bool IsSprinting { get; protected set; } = false;
        #endregion

        #region [ JUMPING VARS ]

        float jumpTimeRemaining;
        public bool IsJumping { get; private set; } = false;
        public int JumpCount { get; private set; } = 0;

        private float timeInAir = 0f;

        #endregion

        #region [ HEAD_BOB ]

        private float headBobProgress = 0f;
        #endregion

        float originalDrag;

        #endregion

        #region [ Audio ]

        private float timeSinceLastStepAudio = 0f;

        #endregion

        #region [[[ EXPOSED EVENTS ]]]

        public UnityEvent <Vector3>OnLand = new UnityEvent<Vector3>();
        public UnityEvent <Vector3>OnStep = new UnityEvent<Vector3>();
        public UnityEvent <bool> OnSprintChanged = new UnityEvent<bool> ();
        public UnityEvent <Vector3>OnJump = new UnityEvent<Vector3>();

    #endregion

    #endregion
    public float currentMaxSpeed
        {
            get
            {
                if(IsGrounded) 
                {
                    return (IsSprinting? characterCtrlData.RunSpeed: characterCtrlData.WalkSpeed) * (isCrouching ? characterCtrlData.CrouchingSpeedMultiplier : 1f);

                }

                return characterCtrlData.CanAirControl ? characterCtrlData.AirControlMaxSpeed : 0f;
            }
        }

        public float CurrentHeight
        {
            get
            {
                if(inCrouchTransition)
                {
                    return Mathf.Lerp(characterCtrlData.CrouchingHeight, characterCtrlData.Height, crouchTransitionProgress);
                }

                return isCrouching ? characterCtrlData.CrouchingHeight : characterCtrlData.Height;
            }
        }


        void Awake()
        {
            rb = GetComponent<Rigidbody>();
            myCollider = GetComponent<CapsuleCollider>();

        }

        private void Start()
        {
            SetCursorLock(characterCtrlData.HideCursor);

            originalDrag = rb.drag;

            myCollider.material = characterCtrlData.MaterialDefault;

            myCollider.height = CurrentHeight;

            myCollider.center = Vector3.up * CurrentHeight * 0.5f;
            
        }
    private void Update()
    {

    }
    void FixedUpdate()
        {
            bool wasGrounded = IsGrounded;
            bool wasSprinting = IsSprinting;

            RaycastHit groundCheckResult = UpdateIsGrounded();

            UpdateSprinting(groundCheckResult);


            if(wasSprinting != IsSprinting) 
            {
                OnSprintChanged.Invoke(IsSprinting);
            }

            if(wasGrounded != IsGrounded && IsGrounded) 
            {
                myCollider.material = characterCtrlData.MaterialDefault;
                timeSinceLastStepAudio = 0f;

                rb.drag = originalDrag;

                if(timeInAir >= characterCtrlData.MinAirTimeForLandSound)
                {
                    OnLand.Invoke(rb.position);
                }

            }

            timeInAir = IsGrounded ? 0f : (Time.deltaTime + timeInAir);// tracks how long in air

            UpdateMovement(groundCheckResult);

        }


        protected void LateUpdate()
        {
            CamLookUpdate();

            UpdateCrouch();
        }

        #region [[[[[ GROUND CHECK ]]]]]
        RaycastHit UpdateIsGrounded()
        {
            RaycastHit hitResult;

            if(jumpTimeRemaining > 0f) 
            { 
                IsGrounded = false;
                return new RaycastHit();
            }

            Vector3 startPos = rb.position + Vector3.up * CurrentHeight * 0.5f;
            float groundCheckRadius = (characterCtrlData.Radius + characterCtrlData.GroundedCheckBuffer);
            float groundCheckDistance = (CurrentHeight * 0.5f) + characterCtrlData.GroundedCheckBuffer;

             if (Physics.Raycast(startPos, Vector3.down, out hitResult, groundCheckDistance,
                  characterCtrlData.GroundLayer, QueryTriggerInteraction.Ignore))
             {

                 IsGrounded = true;
                 JumpCount = 0;
                 jumpTimeRemaining = 0f;

                 //AUTO PARENTING FOR MOVING-PLATFORMS LIKE ELEVATORS ECT...

             }

            else

                IsGrounded = false;

               return hitResult;
           }

        #endregion

        #region [[[[[ ALL MOVEMENT ]]]]]

        #region [[ MOVEMENT ]]
        public void SetMovementLock()
        {
            isMovementLocked = true;
        }

         private void UpdateMovement(RaycastHit groundCheck)
        {

            if (isMovementLocked) // CHECK CAN MOVE
                return;

            Vector3 movementDir = transform.forward * DaxDazCharacterControler_InputManager.Instance.movementDirInput.z + transform.right * DaxDazCharacterControler_InputManager.Instance.movementDirInput.x; //set movement input
            movementDir *= currentMaxSpeed;

            if (IsGrounded)
            {
                movementDir = Vector3.ProjectOnPlane(movementDir, groundCheck.normal); // project onto surface

                if (movementDir.y > 0 && Vector3.Angle(Vector3.up, groundCheck.normal) > characterCtrlData.SlopeLimit) // SLOPE LIMIT CHECK
                {
                    movementDir = Vector3.zero;
                }

                CheckForStepUp(movementDir); // CHECK FOR STAIRS

                FootStepSounds();
            }
            else // IN AIR
            {
                movementDir += Vector3.down * characterCtrlData.FallingVelocity; // set rb Fall velocity
            }

            movementDir = UpdateJumping(movementDir);

            rb.velocity = Vector3.MoveTowards(rb.velocity, movementDir, characterCtrlData.Acceleration);

        }

        #endregion

        #region [[ CROUCHING ]]

        private void UpdateCrouch()
        {
            if(isMovementLocked || isLookingLocked || !characterCtrlData.CanCrouch)
            {
                return;
            }

            if(IsJumping || !IsGrounded)
            {
                if(isCrouching || targetCrouchState)
                {
                    targetCrouchState = false;
                    inCrouchTransition = true;
                }
            }
            else if(characterCtrlData.CrouchingToggle) 
            {
                if (DaxDazCharacterControler_InputManager.Instance.isCrouching) // Toggle crouch Setting
                {

                    DaxDazCharacterControler_InputManager.Instance.isCrouching = false;

                    targetCrouchState = !targetCrouchState;
                    inCrouchTransition = true;
                }
            }
            else
            {
                if(DaxDazCharacterControler_InputManager.Instance.isCrouching != targetCrouchState) // crouch state change
                {
                    targetCrouchState = DaxDazCharacterControler_InputManager.Instance.isCrouching;
                    inCrouchTransition = true;
                }
            }

            if(inCrouchTransition)
            {
                crouchTransitionProgress = Mathf.MoveTowards(crouchTransitionProgress, targetCrouchState ? 0 : 1f, Time.deltaTime / characterCtrlData.CrouchingTransitionTime);


                myCollider.height = CurrentHeight; // CAMERA AND COLLIDER UPDATE
                myCollider.center = Vector3.up * CurrentHeight * 0.5f;
                myCamera.transform.localPosition = Vector3.up * (CurrentHeight + characterCtrlData.VerticalOffset);

                if(Mathf.Approximately(crouchTransitionProgress, targetCrouchState ? 0f : 1f))// is crouch finished
                {
                    isCrouching = targetCrouchState;
                    inCrouchTransition = false;
                }
            }

            Debug.Log(targetCrouchState);        }

        #endregion

        #region [[ SPRINTING ]]
        private void UpdateSprinting(RaycastHit groundCheck)
        {

            if (DaxDazCharacterControler_InputManager.Instance.movementDirInput.magnitude < float.Epsilon) // Stop running if no input
            {
                IsSprinting = false;
            }

            if (!IsGrounded) // can only sprint on the ground
            {
                IsSprinting = false;
                return;
            }

            if (!characterCtrlData.CanRun) // check so if we can sprint
            {
                IsSprinting = false;
                return;
            }

            IsSprinting = DaxDazCharacterControler_InputManager.Instance.isSprinting;
        }

        #endregion

        #region [[ JUMPING ]]
        private Vector3 UpdateJumping(Vector3 movementDir)
        {
            bool triggeredJumpThisFrame = false;

            if (DaxDazCharacterControler_InputManager.Instance.isJumping)
            {
                DaxDazCharacterControler_InputManager.Instance.isJumping = false;
                // JUMP CHECKS
                bool triggerJump = true;
                int numberJumpsPermitted = characterCtrlData.CanDoubleJump ? 2 : 1;

                if (JumpCount >= numberJumpsPermitted)
                {
                    triggerJump = false;
                }

                if (!IsGrounded && !IsJumping)
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

                    rb.drag = 0f;
                    jumpTimeRemaining += characterCtrlData.JumpTime;
                    IsJumping = true;
                    myCollider.material = characterCtrlData.MaterialJumping;
                    ++JumpCount;

                    //DaxCharacterControlerEventHandler.Instance.CallOnJumpEvent();
                    OnJump.Invoke(rb.position);
                }

            }

            if (IsJumping)
            {
                // upadate remaining jump time if not jumping this frame
                if (!triggeredJumpThisFrame)
                {
                    jumpTimeRemaining -= Time.deltaTime;
                }

                //Jumping finished
                if (jumpTimeRemaining <= 0)
                {
                    IsJumping = false;
                }
                else
                {
                    movementDir.y = characterCtrlData.JumpVelocity;
                }

            }

            return movementDir;
        }
        #endregion

        #region [[ STAIRS ]]
        private void CheckForStepUp(Vector3 movementDir)
        {
            Vector3 lookAheadSratPoint = transform.position + Vector3.up * (characterCtrlData.StairHeightCheck_MaxHeight * 0.5f);

            Vector3 lookAheadDir = movementDir.normalized;

            float lookAheadDistance = characterCtrlData.Radius + characterCtrlData.StairCheck_LookAheadRange;
            Debug.DrawRay(lookAheadSratPoint, lookAheadDir, Color.green, lookAheadDistance);
            if (Physics.Raycast(lookAheadSratPoint, lookAheadDir, lookAheadDistance, characterCtrlData.GroundLayer, QueryTriggerInteraction.Ignore)) // Check for step ahead of character
            {
                Debug.Log("Ray one hit");
                Debug.DrawRay(lookAheadSratPoint, lookAheadDir, Color.blue, lookAheadDistance);
                lookAheadSratPoint = transform.position + Vector3.up * characterCtrlData.StairHeightCheck_MaxHeight;

                if (!Physics.Raycast(lookAheadSratPoint, lookAheadDir, lookAheadDistance, characterCtrlData.GroundLayer, QueryTriggerInteraction.Ignore)) // Check is clear above step
                {

                    Debug.Log("Ray two clear");
                    Vector3 canPoint = lookAheadSratPoint + lookAheadDir * lookAheadDistance;

                    RaycastHit hitResult;
                    Debug.DrawRay(canPoint, Vector3.down, Color.cyan, characterCtrlData.StairHeightCheck_MaxHeight * 2f);
                    if (Physics.Raycast(canPoint, Vector3.down, out  hitResult, characterCtrlData.StairHeightCheck_MaxHeight * 2f,
                        characterCtrlData.GroundLayer, QueryTriggerInteraction.Ignore))
                    {

                        Debug.Log("Ray three hit");
                        if (Vector3.Angle(Vector3.up, hitResult.normal) <= characterCtrlData.SlopeLimit)
                        {

                            Debug.Log("MOVE UP STAIRS");
                            var stepUp = Vector3.Lerp(rb.transform.position, hitResult.point,characterCtrlData.StepClimbSpeed);

                            rb.position = stepUp;          
                        }
                    }
                }
            }
        }
        #endregion

        #region [[ AUDIO ]]
        private void FootStepSounds()
        {
            if (DaxDazCharacterControler_InputManager.Instance.movementDirInput.magnitude > float.Epsilon)
            {
                timeSinceLastStepAudio += Time.deltaTime;

                float footStepInterval = IsSprinting ? characterCtrlData.FootStepSpeed_sprinting : characterCtrlData.FootStepSpeed_Walking;
                if (timeSinceLastStepAudio > footStepInterval)
                {
                    //DaxCharacterControlerEventHandler.Instance.CallOnStepEvent();
                    OnStep.Invoke(rb.position);

                    timeSinceLastStepAudio -= footStepInterval;
                }
            }

        }
        #endregion

        #endregion

        #region [[[[[ LOOKING ]]]]]

        private void CamLookUpdate()
        {
            if(isLookingLocked) return;

            // Calc Cam Inputs
            var lookInput = DaxDazCharacterControler_InputManager.Instance.lookDirInputRaw;

             float camRotDelta = lookInput.x * characterCtrlData.CameraHorizontalSensitivity * Time.deltaTime;
             float camPitchDelta = lookInput.y * characterCtrlData.CameraVerticalSensitivity * Time.deltaTime *
                (characterCtrlData.CamInvertY ? 1f : -1f);//Turn to -1 to invert the camera control style could add bool and ter exp ?1 : -1; ect...

        // Rotate Character
            transform.localRotation = transform.localRotation * Quaternion.Euler(0f, camRotDelta, 0f);
        // rb.MoveRotation(transform.localRotation * Quaternion.Euler(0f, camYawDelta, 0f));


        // Tilt Cam
        currentCamPitch = Mathf.Clamp(currentCamPitch + camPitchDelta, characterCtrlData.Camera_MinPitch, characterCtrlData.Camera_MaxPitch);

        myCamera.transform.localRotation = Quaternion.Euler(currentCamPitch, 0f, 0f);

        if (characterCtrlData.HeadBobEnable && IsGrounded) 
            {
                float currentSpeed = rb.velocity.magnitude;
                Vector3 defaultCamOffset = Vector3.up * (CurrentHeight + characterCtrlData.VerticalOffset);

                if (currentSpeed >= characterCtrlData.HeadBobActivation_MinSpeed) 
                {
                    float speedFactor = currentSpeed / (characterCtrlData.CanRun ? characterCtrlData.RunSpeed -2f : characterCtrlData.WalkSpeed - 2f);

                    headBobProgress += Time.deltaTime / characterCtrlData.HeadBob_interval.Evaluate(speedFactor);
                    headBobProgress %= 1f;

                    float maxVerticalTranslation = characterCtrlData.HeadBobSpeed_VertAnimCurve.Evaluate(speedFactor);
                    float maxHorizontalTranslation = characterCtrlData.HeadBobSpeed_HoriAnimCurve.Evaluate(speedFactor);
                    float sinPro = Mathf.Sin(headBobProgress * Mathf.PI * 2f);

                    defaultCamOffset += Vector3.up * sinPro * maxVerticalTranslation; 
                    defaultCamOffset += Vector3.right * sinPro * maxHorizontalTranslation;

                }
                else
                
                    headBobProgress = 0f;
                myCamera.transform.transform.localPosition = Vector3.MoveTowards(myCamera.transform.localPosition, defaultCamOffset, characterCtrlData.HeadBobLerpSpeed * Time.deltaTime);
            }

        }

        public void SetCursorLock(bool locked)
        {
            UnityEngine.Cursor.visible = !locked;
            UnityEngine.Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
        }

        public void SetLookLock(bool locked)
        {
            isLookingLocked = locked;
        }

        #endregion

    }



