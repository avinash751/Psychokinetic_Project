using Dreamteck.Splines;
using UnityEngine;
using CustomInspector;
using System.Collections.Generic;

public class GrindController : PlayerMovement
{
    [SerializeField] GrindState _grindState;
    [HorizontalLine("Grind Controller Settings", 2, FixedColor.BabyBlue)]
    [SerializeField] public float normalGrindSpeed;
    [SerializeField] float layerToIgnore;
    [SerializeField][Resettable] public float currentGrindSpeed;
    [SerializeField] float sprintSpeedMultiplier;
    [SerializeField] float SprintingTransitionSpeed;

    [HorizontalLine("Momentum Settings", 2, FixedColor.BabyBlue)]
    [SerializeField][Range(0,1)] float dynamicJumpOffThreshold;
    [SerializeField] float sprintJumpMomentumMultiplier;
    [SerializeField] float normalGrindJumpMomentumMultiplier;
    [SerializeField] float upwardForce;

    [HorizontalLine("Rail Switching  Settings", 2, FixedColor.BabyBlue)]
    [SerializeField] float sideForce;
    [SerializeField] float switchFallForce;

    [HorizontalLine("Invoke settings", 2, FixedColor.BabyBlue)]
    [SerializeField] float durationToResetSwitching;
    [SerializeField] float OnExitBoxColliderDisableDuration;

    SplineComputer previousSpline;

    string key = "RailGrind";



    private void OnEnable()
    {
        OnVerticalStateChanged += WhenJumpingExitRails;
        ReferenceManager.Instance.Resetter.OnReset += OnReset;
    }

    private void OnDisable()
    {
        OnVerticalStateChanged -= WhenJumpingExitRails;
        ReferenceManager.Instance.Resetter.OnReset -= OnReset;
    }
    protected override void Start()
    {
        currentGrindSpeed = normalGrindSpeed;
        splineFollower.followSpeed = currentGrindSpeed;
    }
    private void Update()
    {
        _grindState = grindState;
        if (splineFollower.spline == null && moveState != MoveState.OnRail) return;
        transform.gameObject.transform.rotation = transform.rotation;

        SwitchingSplineForwardDirection();
        SideToSideRailSwitching("Left Rail Switch", -transform.right);
        SideToSideRailSwitching("Right Rail Switch", transform.right);

        GrindRailSprinting();
        AutoJumpOffDetection();
    }

    private void SideToSideRailSwitching(string controllerState, Vector3 _direction)
    {

        if (InputManager.GetKeyState(controllerState))
        {
            ExitRails();
            SetGrindStateTo(GrindState.RailSwitching);
            Invoke(nameof(ResetRailSwitchState), durationToResetSwitching);
            GetComponent<BoxCollider>().enabled = false;
            Rb.AddForce(_direction * sideForce, ForceMode.Impulse);
            Rb.AddForce(-transform.up * switchFallForce, ForceMode.VelocityChange);
           EventManager.Invoke(EventType.RailSwitching, GetType());
        }
    }

    private void SwitchingSplineForwardDirection()
    {
        if (InputManager.GetKeyState("Grind Up"))
        {
            ChangeRailForwardDirection(Mathf.Abs(splineFollower.followSpeed), -Mathf.Abs(splineFollower.followSpeed)
            , transform.forward, CameraTransform.forward);
        }

        if (InputManager.GetKeyState("Grind Down"))
        {
            ChangeRailForwardDirection(-Mathf.Abs(splineFollower.followSpeed), Mathf.Abs(splineFollower.followSpeed)
            , transform.forward, CameraTransform.forward);
        }
    }

    private void GrindRailSprinting()
    {
        bool isCountinousKeyPress = true;
        bool isSpeedingUp = InputManager.GetKeyState("Rail Sprint", isCountinousKeyPress);
        if (grindState is GrindState.RailSwitching) return;

        ExternalMomentum = isSpeedingUp ? transform.forward * sprintJumpMomentumMultiplier : transform.forward * normalGrindJumpMomentumMultiplier;
        SetGrindStateTo(isSpeedingUp ? GrindState.RailBreaking : grindState);

        float targetSpeed = isSpeedingUp ? currentGrindSpeed * sprintSpeedMultiplier : currentGrindSpeed;
        ExternalMomentum *= targetSpeed* dynamicJumpOffThreshold;
        targetSpeed = splineFollower.direction == Spline.Direction.Forward ? targetSpeed : -targetSpeed;
        splineFollower.followSpeed = Mathf.Lerp(splineFollower.followSpeed, targetSpeed, SprintingTransitionSpeed * Time.deltaTime);
    }

    public void GoGrindOnThoseRails(SplineComputer splineToGrindOn)
    {
        if (splineFollower.spline != null) return;
        if (grindState is GrindState.RailGrinding || grindState is GrindState.RailBreaking) return;
        if (grindState is GrindState.RailSwitching && previousSpline == splineToGrindOn) return;
        if (verticalState is VerticalState.InAir) return;

        Vector3 previousForward = transform.forward;
        splineFollower.spline = splineToGrindOn;
        splineFollower.RebuildImmediate();
        SplineSample sample = splineFollower.spline.Project(transform.position, 0, 1);
        double percent = sample.percent;
        splineFollower.SetPercent(percent);
        splineFollower.follow = true;
        splineFollower.wrapMode = SplineFollower.Wrap.Default;

        SetMoveStateTo(MoveState.OnRail);
        Invoke(nameof(TransitionFromGettingOnRailsToGrindState), 0.1f);

        ChangeRailForwardDirection(Mathf.Abs(normalGrindSpeed), -Mathf.Abs(normalGrindSpeed),
        previousForward, transform.forward);
        EventManager.Invoke(EventType.RailGrinding, GetType());

        AudioManager.Instance?.PlayAudio(key);

    }

    public void AutoJumpOffDetection()
    {
        if (_grindState is GrindState.GettingOnRail) return;

        bool atEndOfRail = splineFollower.result.percent > 0.995f || splineFollower.result.percent < 0.005f;
        if (!atEndOfRail) return;

        ExitRails();
        Rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
        Rb.AddForce(ExternalMomentum, ForceMode.VelocityChange);
        ExternalMomentum = Vector3.zero;
    }

    public void ExitRails()
    {
        if (moveState != MoveState.OnRail) return;
        previousSpline = splineFollower.spline;
        splineFollower.follow = false;
        splineFollower.spline = null;
        GetComponent<BoxCollider>().enabled = false;
        Invoke(nameof(EnableBoxCollidor), OnExitBoxColliderDisableDuration);
        AudioManager.Instance?.StopAudio(key);
        SetMoveStateTo(MoveState.FullControlMove);
    }

    void WhenJumpingExitRails(VerticalState state)
    {
        if (state == VerticalState.InAir)
        { ExitRails(); }
    }

    private void TransitionFromGettingOnRailsToGrindState()
    {
        if (_grindState is GrindState.GettingOnRail)
        {
            SetGrindStateTo(GrindState.RailGrinding);
            splineFollower.wrapMode = SplineFollower.Wrap.Default;
        }
    }

    private void ChangeRailForwardDirection(float forwardSpeed, float backwardSpeed, Vector3 lhs, Vector3 rhs)
    {
        float dot = Vector3.Dot(lhs, rhs);

        if (dot > 0)
        {
            if (splineFollower.direction == Spline.Direction.Forward)
            {
                splineFollower.followSpeed = forwardSpeed;
            }
            else
            {
                splineFollower.followSpeed = backwardSpeed;
            }
        }
        else
        {
            if (splineFollower.direction == Spline.Direction.Forward)
            {
                splineFollower.followSpeed = backwardSpeed;
            }
            else
            {
                splineFollower.followSpeed = forwardSpeed;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent(out SplineComputer spline))
        {
            if (other.gameObject.layer == layerToIgnore) return;
            if(grindState is GrindState.RailSwitching)
            {
                if (previousSpline == spline) return;
            
            }
            if ( moveState is not MoveState.OnRail)
            {
                GoGrindOnThoseRails(spline);
            }
              
        }
    }

    void EnableBoxCollidor()
    {
        GetComponent<BoxCollider>().enabled = true;
    }

    void ResetRailSwitchState()
    {
        if (grindState is GrindState.RailSwitching && moveState is MoveState.FullControlMove && verticalState is VerticalState.Grounded)
        {
            SetGrindStateTo(GrindState.None);
        }
    }

    private void OnReset()
    {
        AudioManager.Instance?.StopAudio(key);
    }

}