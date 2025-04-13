using CustomInspector;
using MoreMountains.Tools;
using UnityEngine;
using System;
using System.Collections.Generic;

public class Jump : PlayerMovement
{
    [SerializeField] VerticalState _verticalState;
    [SerializeField] JumpState _jumpState;
    [SerializeField] MMGizmo jumpGizmo;
    [SerializeField] MMGizmo groundWallGizmo;
    [SerializeField] bool debugMode;

    [HorizontalLine("Jump Settings", 2, FixedColor.Gray)]
    [SerializeField] ForceMode jumpForceType;
    [SerializeField] AnimationCurve jumpCurve;
    [SerializeField] float verticalForce = 5;
    [SerializeField] float jumpForwardForce = 5;
    [SerializeField] float durationOfJump;
    [SerializeField] float coyoteTimeDuration = 0.1f;

    TimerUtilities jumpTimer = new TimerUtilities();
    TimerUtilities coyoteTimer = new TimerUtilities();

    [HorizontalLine("Momentum Settings", 2, FixedColor.Gray)]
    [SerializeField] float forwardMomentumMultiplier;
    [SerializeField][Range(0, 1)] float railJumpMomentumMultiplier;

    [HorizontalLine("Falling Settings", 2, FixedColor.Gray)]
    [SerializeField] ForceMode gravityForceType;
    [SerializeField] float gravityMultiplier = 2.5f;
    [SerializeField] float gravityAcceleration = 0.5f;

    [HorizontalLine("Ground Detection", 2, FixedColor.Gray)]
    [SerializeField]Vector3 groundDetectionOffsetVector;
    [SerializeField] float groundDetectionRadius = 2f;
    [SerializeField] float groundWallDetectionRadius = 0.5f;
    [SerializeField]Vector3 groundWallDetectionOffset;

    [HorizontalLine("Layer To Trigger Jump", 2, FixedColor.Gray)]
    [SerializeField] LayerMask groundedLayer;

    [HorizontalLine("Bunny Hopping Settings", 2, FixedColor.Gray)]
    [SerializeField] float bunnyHopForwardForce = 5;
    [SerializeField] float maxBunnyHopsAllowed;
    [SerializeField] float bunnyHopTimeWindow = 0.5f;
    [SerializeField] public float bunnyHopsDone;
    [SerializeField][ReadOnly] bool bHopTimeWindowActivated;

    public static Action onSuccessfulBunnyHop;

    float defaultGravity;
    float defaultDrag;

    string key = "JumpSound";

    private void OnValidate()
    {
        if (jumpGizmo != null)
        {
            jumpGizmo.GizmoType = MMGizmo.GizmoTypes.Position;
            jumpGizmo.PositionMode = MMGizmo.PositionModes.Sphere;
            jumpGizmo.PositionSize = groundDetectionRadius;
            jumpGizmo.GizmoOffset = groundDetectionOffsetVector;
            jumpGizmo.DisplayText = true;
            jumpGizmo.TextMode = MMGizmo.TextModes.CustomText;
            jumpGizmo.TextToDisplay = "Ground Detection";
        }

        if (groundWallGizmo != null)
        {
            groundWallGizmo.GizmoType = MMGizmo.GizmoTypes.Position;
            groundWallGizmo.PositionMode = MMGizmo.PositionModes.Sphere;
            groundWallGizmo.PositionSize = groundWallDetectionRadius;
            groundWallGizmo.GizmoOffset = groundWallDetectionOffset;
            groundWallGizmo.DisplayText = true;
            groundWallGizmo.TextMode = MMGizmo.TextModes.CustomText;
            groundWallGizmo.TextToDisplay = "Ground Wall Detection";
        }

        if (debugMode)
        {
            jumpGizmo.DisplayGizmo = true;
            groundWallGizmo.DisplayGizmo = true;
        }
        else
        {

            if (jumpGizmo != null) jumpGizmo.DisplayGizmo = false;
            if (groundWallGizmo != null) groundWallGizmo.DisplayGizmo = false;
        }
    }

    protected override void Start()
    {
        base.Start();
        jumpTimer.InitializeNormalStopWatch(durationOfJump);
        defaultGravity = gravityMultiplier;
        defaultDrag = Rb.drag;
    }

    private void Update()
    {
        _jumpState = jumpState;
        _verticalState = verticalState;
        GroundDetection();
        TriggerBunnyHopOrJump();
    }
    private void FixedUpdate()
    {
        DoAJumpOrBHop();
        FallingToGround();
        ManageBunnyHoppingState();
    }

    private void TriggerBunnyHopOrJump()
    {
        if (InputManager.GetKeyState("Jump"))
        {
            bool canJump = coyoteTimer.CountDownEnabled || verticalState is VerticalState.Grounded;
            if (!canJump) return;
            Rb.velocity = new Vector3(Rb.velocity.x, 0, Rb.velocity.z);
            if (bHopTimeWindowActivated)
            {
                EventManager.Invoke(EventType.BunnyHop, GetType());
                bHopTimeWindowActivated = false;
                SetJumpStateTo(JumpState.BunnyHopping);
                if (bunnyHopsDone < maxBunnyHopsAllowed)
                { bunnyHopsDone++; }
                ExternalMomentum += (transform.forward * (bunnyHopForwardForce * (bunnyHopsDone + 1)));
                onSuccessfulBunnyHop?.Invoke();


                CancelInvoke(nameof(ResetBunnyHoppingAndJump));
            }
            else
            {
                ManageBunnyHoppingState();
                SetJumpStateTo(JumpState.NormalJump);
            }
            AudioManager.Instance?.PlayAudio(key);
            SetVerticalStateTo(VerticalState.InAir);
        }
    }

    void DoAJumpOrBHop()
    {
        if (verticalState != VerticalState.InAir) return;

        (float elapsedTime, bool stopWatchEnabled) = jumpTimer.UpdateStopWatch(false);

        if (stopWatchEnabled)
        {
            float jumpValue = jumpCurve.Evaluate(elapsedTime / durationOfJump);
            Vector3 jumpForce = ((Vector3.up * verticalForce) + (transform.forward * forwardMomentumMultiplier) + (ExternalMomentum * railJumpMomentumMultiplier)) * jumpValue;
            jumpForce *= Time.deltaTime * 50;
            Rb.AddForceAtPosition(jumpForce, transform.position, jumpForceType);
            return;
        }
        SetVerticalStateTo(VerticalState.Falling);
        jumpTimer.InitializeNormalStopWatch(durationOfJump);
        ExternalMomentum = Vector3.zero;

    }


    void FallingToGround()
    {

        if (moveState is MoveState.OnRail || verticalState is VerticalState.Grounded)
        {
            return;
        }
        gravityMultiplier += Time.deltaTime * gravityAcceleration;
        Rb.AddForce(Vector3.down * gravityMultiplier, gravityForceType);
        Rb.drag = defaultDrag / 1.5f;
    }

    private void GroundDetection()
    {
        if (coyoteTimer.CountDownEnabled && verticalState is not VerticalState.Grounded)
        {
            coyoteTimer.UpdateCountDown();
        }
        if (verticalState is VerticalState.InAir) return;

        bool isOnGround = Physics.OverlapSphere(transform.position + groundDetectionOffsetVector, groundDetectionRadius, groundedLayer, QueryTriggerInteraction.Ignore).Length > 0;
        bool isOnGroundedWall = Physics.OverlapSphere(transform.position + groundWallDetectionOffset, groundWallDetectionRadius, groundedLayer, QueryTriggerInteraction.Ignore).Length > 0;

        if(isOnGroundedWall && !isOnGround)
        {
            if(moveState is MoveState.OnRail) return;
            SetVerticalStateTo(VerticalState.Falling);
        }
        else if (isOnGround)
        {
            SetVerticalStateTo(VerticalState.Grounded);
            ResetDragAndGravity();
            coyoteTimer.InitializeCountDownTimer(coyoteTimeDuration);
        }
        else
        {
            if (moveState is MoveState.OnRail) return;
            SetVerticalStateTo(VerticalState.Falling);
        }
      
    }
    void ManageBunnyHoppingState()
    {
        if (bHopTimeWindowActivated) return;
        if (verticalState == VerticalState.Grounded && jumpState != JumpState.None)
        {
            bHopTimeWindowActivated = true;
            Invoke(nameof(ResetBunnyHoppingAndJump), bunnyHopTimeWindow);
        }
    }

    void ResetBunnyHoppingAndJump()
    {
        bunnyHopsDone = 0;
        SetJumpStateTo(JumpState.None);
        bHopTimeWindowActivated = false;
    }

    public void ResetDragAndGravity()
    {
        gravityMultiplier = defaultGravity;
        Rb.drag = defaultDrag;
    }
}