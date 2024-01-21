using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CharacterMoveData_SO", menuName = "CharacterCtrl/New Move Data")]

//
public class CharacterControlData_SO : ScriptableObject
{
    #region [[[[[ VARS ]]]]]

    [Header("BASIC_VARS")]
    [SerializeField] private string _name;
    [TextArea(5, 5)]
    [SerializeField] private string description;

    #region [[[ CHARACTER VARS ]]]
    [Header("CHARACTER VARS")]
    [SerializeField] private float height = 1.8f;
    [SerializeField] private float radius = 0.3f;

    #endregion

    #region [[[ CAMERA VARS ]]]

    [Header("CAMERA")]
    [SerializeField] private bool camInvertY = false;
    [SerializeField] private float verticalOffset = -0.1f;

    [Header("view speed")]
    [SerializeField] private float cameraHorizontalSensitivity = 10f;
    [SerializeField] private float cameraVerticalSensitivity = 10f;

    [Header("view angle limits")]
    [SerializeField] private float camera_MinPitch = -75f;
    [SerializeField] private float camera_MaxPitch = 75f;

    #region [ VCAM_EFFECTS_VARS ]
    [Header("FOV CAMERA VARS")]
    [SerializeField] private float defaultFov = 40f;
    [SerializeField] private float walkingFOV = 40f;
    [SerializeField] private float runningFov = 50f;
    [SerializeField] private float slewRate = 50f;

    #endregion

    #endregion

    #region [[[ MOVEMENT VARS ]]]

    [Header("MOVEMENT")]
    [SerializeField] private bool canRun = true;
    [SerializeField] private float walkSpeed = 10f;
    [SerializeField] private float runSpeed = 15f;
    [SerializeField] private float acceleration = 1f;

    [SerializeField] private float slopeLimit = 60f;

    [Header("CROUCHING")]
    [SerializeField] private bool canCrouch = true;
    [SerializeField] private bool crouchingToggle = true;
    [SerializeField] private float crouchingHeight = 0.9f;
    [SerializeField] private float crouchingSpeedMultiplier = 0.5f;
    [SerializeField] private float crouchingTransitionTime = 0.25f;

    [Header("GROUNDED CHECKS")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundedCheckBuffer = 0.4f;
    [SerializeField] private float groundedCheckColliderRadiusBuffer = 0.05f;

    [Header("JUMPING")]
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canDoubleJump = true;
    [SerializeField] private float jumpVelocity = 15;
    [SerializeField] private float jumpTime = .1f;

    [Header("STAIRS CHECKS")]
    [SerializeField] private float stairCheck_LookAheadRange = 0.1f;
    [SerializeField] private float stairHeightCheck_MaxHeight = 0.2f;
    [SerializeField] private float stepClimbSpeed = 0.5f;

    [Header("HEAD BOB")]
    [SerializeField] private bool headBobEnable = true;
    [SerializeField] private float headBobActivation_MinSpeed = 0.1f;
    [SerializeField] private float headBobLerpSpeed = 1f;
    [SerializeField] private AnimationCurve headBobSpeed_VertAnimCurve;
    [SerializeField] private AnimationCurve headBobSpeed_HoriAnimCurve;
    [SerializeField] private AnimationCurve headBob_interval;

    #endregion

    #region [[[ CURSOR VARS ]]]
    [Header("CURSOR")]
    [SerializeField] private bool hideCursor = true;
    #endregion

    #region [[[ INAIR ]]]

    [Header("AIR CONTROL")]
    [SerializeField] private bool canAirControl = true;
    [SerializeField] private float airControlMaxSpeed = 10f;

    #endregion

    #region [[[ FALLING VARS ]]]

    [Header("FALLING")]
    [SerializeField] private float fallingVelocity = 12.5f;


    #endregion

    #region [[[ PHYSICS ]]]

    [Header("PHYSICS MATERIALS")]
    [SerializeField] private PhysicMaterial materialDefault;
    [SerializeField] private PhysicMaterial materialJumping;

    #endregion

    #region [[[ AUDIO ]]]

    [Header("Audio")]

    [SerializeField] private List<AudioClip> footStepSounds;
    [SerializeField]private List<AudioClip> landOnGroundSounds;
    [SerializeField] private List<AudioClip> jumpingSounds;

    [SerializeField] private float footStepSpeed_Walking = 0.4f;
    [SerializeField] private float footStepSpeed_sprinting = 0.2f;

    [SerializeField] private float minAirTimeForLandSound = 0.2f;
    #endregion

    #endregion

    #region [[[[[ GETTERS & SETTERS ]]]]]

    #region [[[ CHARACTER GETTERS & SETTERS ]]]
    public float Height { get => height; set => height = value; }
    public float Radius { get => radius; set => radius = value; }

    #endregion

    #region [[[ CAMERA GETTERS & SETTERS ]]]

    #region [ LOOK VALUES ]
    public bool CamInvertY { get => camInvertY; set => camInvertY = value; }
    public float CameraHorizontalSensitivity { get => cameraHorizontalSensitivity; set => cameraHorizontalSensitivity = value; }
    public float CameraVerticalSensitivity { get => cameraVerticalSensitivity; set => cameraVerticalSensitivity = value; }
    public float Camera_MinPitch { get => camera_MinPitch; set => camera_MinPitch = value; }
    public float Camera_MaxPitch { get => camera_MaxPitch; set => camera_MaxPitch = value; }
    public float VerticalOffset { get => verticalOffset; set => verticalOffset = value; }

    #endregion

    #region [ VCAM ]
    public float DefaultFov { get => defaultFov; set => defaultFov = value; }
    public float WalkingFOV { get => walkingFOV; set => walkingFOV = value; }
    public float RunningFov { get => runningFov; set => runningFov = value; }
    public float SlewRate { get => slewRate; set => slewRate = value; }

    #endregion

    #endregion

    #region [[[ MOVEMENT GETTERS & SETTERS ]]]

    public float WalkSpeed { get => walkSpeed; set => walkSpeed = value; }
    public float RunSpeed { get => runSpeed; set => runSpeed = value; }
    public bool CanRun { get => canRun; set => canRun = value; }
    public float Acceleration { get => acceleration; set => acceleration = value; }

    #region [[ GROUND CHECKS ]]
    public LayerMask GroundLayer { get => groundLayer; set => groundLayer = value; }
    public bool IsGrounded { get => IsGrounded; set => IsGrounded = value; }
    public float GroundedCheckBuffer { get => groundedCheckBuffer; set => groundedCheckBuffer = value; }
    public float GroundedCheckColliderRadiusBuffer { get => groundedCheckColliderRadiusBuffer; set => groundedCheckColliderRadiusBuffer = value; }
    public float FallingVelocity { get => fallingVelocity; set => fallingVelocity = value; }

    #endregion

    #region [[ CROUCHING ]]

    public bool CanCrouch { get => canCrouch; set => canCrouch = value; }
    public bool CrouchingToggle { get => crouchingToggle; set => crouchingToggle = value; }
    public float CrouchingHeight { get => crouchingHeight; set => crouchingHeight = value; }
    public float CrouchingSpeedMultiplier { get => crouchingSpeedMultiplier; set => crouchingSpeedMultiplier = value; }
    public float CrouchingTransitionTime { get => crouchingTransitionTime; set => crouchingTransitionTime = value; }

    #endregion

    #region [[[ JUMPING ]]]

    public bool CanDoubleJump { get => canDoubleJump; set => canDoubleJump = value; }
    public bool CanJump { get => canJump; set => canJump = value; }
    public float JumpVelocity { get => jumpVelocity; set => jumpVelocity = value; }
    public float JumpTime { get => jumpTime; set => jumpTime = value; }

    #endregion

    #region [[ AIR_CONTROL ]]

    public bool CanAirControl { get => canAirControl; set => canAirControl = value; }
    public float AirControlMaxSpeed { get => airControlMaxSpeed; set => airControlMaxSpeed = value; }

    #endregion

    #region [[ STAIRS ]]
    public float SlopeLimit { get => slopeLimit; set => slopeLimit = value; }
    public float StairCheck_LookAheadRange { get => stairCheck_LookAheadRange; set => stairCheck_LookAheadRange = value; }
    public float StairHeightCheck_MaxHeight { get => stairHeightCheck_MaxHeight; set => stairHeightCheck_MaxHeight = value; }
    #endregion

    #region [[ HEAD_BOB ]]
    public bool HeadBobEnable { get => headBobEnable; set => headBobEnable = value; }
    public float HeadBobActivation_MinSpeed { get => headBobActivation_MinSpeed; set => headBobActivation_MinSpeed = value; }
    public AnimationCurve HeadBobSpeed_VertAnimCurve { get => headBobSpeed_VertAnimCurve; set => headBobSpeed_VertAnimCurve = value; }
    public AnimationCurve HeadBobSpeed_HoriAnimCurve { get => headBobSpeed_HoriAnimCurve; set => headBobSpeed_HoriAnimCurve = value; }
    public AnimationCurve HeadBob_interval { get => headBob_interval; set => headBob_interval = value; }
    public float HeadBobLerpSpeed { get => headBobLerpSpeed; set => headBobLerpSpeed = value; }

    #endregion

    #endregion

    #region [[[ AUDIO ]]]

    public List<AudioClip> FootStepSounds { get => footStepSounds; set => footStepSounds = value; }
    public List<AudioClip> LandOnGroundSounds { get => landOnGroundSounds; set => landOnGroundSounds = value; }
    public List<AudioClip> JumpingSounds { get => jumpingSounds; set => jumpingSounds = value; }

    public float FootStepSpeed_sprinting { get => footStepSpeed_sprinting; set => footStepSpeed_sprinting = value; }
    public float FootStepSpeed_Walking { get => footStepSpeed_Walking; set => footStepSpeed_Walking = value; }

    public float MinAirTimeForLandSound { get => minAirTimeForLandSound; set => minAirTimeForLandSound = value; }

    #endregion

    #region [[[ MISC ]]]
    public bool HideCursor { get => hideCursor; set => hideCursor = value; }
    public PhysicMaterial MaterialDefault { get => materialDefault; set => materialDefault = value; }
    public PhysicMaterial MaterialJumping { get => materialJumping; set => materialJumping = value; }
    public float StepClimbSpeed { get => stepClimbSpeed; set => stepClimbSpeed = value; }


    #endregion

    #endregion
}