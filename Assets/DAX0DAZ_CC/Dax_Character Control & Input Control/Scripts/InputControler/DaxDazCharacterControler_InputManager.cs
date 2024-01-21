
using System;
using UnityEngine;
using UnityEngine.InputSystem;


    public class DaxDazCharacterControler_InputManager : MonoBehaviour
    {

        public static DaxDazCharacterControler_InputManager Instance;
        #region [[[[[ VARS ]]]]]

        #region  [[[ INPUT CONTROL VARS ]]]

        [Header("KEYBOARD & MOUSE INPUT CONTROL")]

        [SerializeField] private InputAction mouseLookDirection;
        public Vector3 lookDirInputRaw;
        public Vector3 lookDirInput;

        [SerializeField] private InputAction mousePosition;
        public Vector3 mousePositionInput;

        [SerializeField] private InputAction keyboardMoveDirection;

        public Vector3 movementDirInput;

        [SerializeField] private InputAction keyboardCrouch;

        public bool isCrouching;

        [SerializeField] private InputAction keyboardRun;

        public bool isSprinting;

        [SerializeField] private InputAction keyboardInteract;

        public bool isInteracting;

        [SerializeField] private InputAction keyboardJump;

        public bool isJumping;


        #endregion

        #endregion

        private void OnEnable()
        {
            #region [[[[[ INPUT INITILIZATION ]]]]]

            keyboardMoveDirection.Enable();
            mouseLookDirection.Enable();
            mousePosition.Enable();

            keyboardRun.Enable();
            keyboardRun.performed += RunInput;
            keyboardRun.canceled += CancelRunInput;

            keyboardCrouch.Enable();
            keyboardCrouch.performed += CrouchInput;
            keyboardCrouch.canceled += CancelCrouchInput;

            keyboardJump.Enable();
            keyboardJump.performed += JumpInput;
            keyboardJump.canceled += CancelJumpInput;

            keyboardInteract.Enable();
            keyboardInteract.performed += Interact;
            keyboardInteract.canceled += CancelInteract;

            #endregion
        }

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        #region [[[[[ MOUSE ]]]]]
        lookDirInputRaw = mouseLookDirection.ReadValue<Vector2>();
        lookDirInput = mouseLookDirection.ReadValue<Vector2>().normalized;

        mousePositionInput = mousePosition.ReadValue<Vector2>();
        #endregion
    }
    private void FixedUpdate()
        {
            #region [[[[[ KEYBOARD ]]]]]

            var moveInput = keyboardMoveDirection.ReadValue<Vector2>().normalized;
            movementDirInput = new Vector3(moveInput.x, 0f, moveInput.y);

            #endregion


        }

        #region [[[[[ MOVEMENT ]]]]]

        #region [[[[[ RUN ]]]]]
        public void RunInput(InputAction.CallbackContext context)
        {
            isSprinting = true;
        }

        void CancelRunInput(InputAction.CallbackContext context)
        {
            ResetRun();
        }

        void ResetRun()
        {
            isSprinting = false;
        }

        #endregion

        #region [[[[[ CROUCH ]]]]]
        public void CrouchInput(InputAction.CallbackContext context)
        {
            isCrouching = true;
        }

        void CancelCrouchInput(InputAction.CallbackContext context)
        {
            ResetCrouch();
        }

        void ResetCrouch()
        {
            isCrouching = false;
        }

        #endregion

        #region [[[[[ JUMP ]]]]]
        public void JumpInput(InputAction.CallbackContext context)
        {

            isJumping = true;

           // ResetJump();
        }

        void CancelJumpInput(InputAction.CallbackContext context)
        {
            ResetJump();
        }

        void ResetJump()
        {
            isJumping = false;
        }

        #endregion


        #endregion

        #region [[[[[ INTERACTION ]]]]]
        public void Interact(InputAction.CallbackContext context)
        {

            isInteracting = true;

            DaxCharacterControler_EventHandler.Instance.CallOnObjectInteractEvent();// calls an interaction event to all interactable objects, in which they will check if the player in the interactable space for the object has the interactable interface ie... will check if the player is in the objects radius to trigger the interact event

        }

        void CancelInteract(InputAction.CallbackContext context)
        {
            isInteracting = false;
        }

        #endregion
    }

