using Dreamteck.Splines;
using UnityEngine;
using System;

public enum MoveState
{
    FullControlMove,
    onSlope,
    OnRail,
}

public enum GrindState
{
    None,
    GettingOnRail,
    RailGrinding,
    RailBreaking,
    RailSwitching,
}

public enum VerticalState
{
    Grounded,
    InAir,
    Falling,
}

public enum JumpState
{
    None,
    NormalJump,
    BunnyHopping,
    Stomping,
    PerfectStomp,
}

public abstract class PlayerMovement : MonoBehaviour, INeedPlayerRefs, IResettable
{

    protected static Rigidbody Rb;
    protected static SplineFollower splineFollower;
    protected static Transform CameraTransform;
    public static Vector3 PlayerMoveDirection { get; protected set; }

    public static Vector3 CamAndPlayerMoveDirection { get; protected set; }
    public static Vector3 ExternalMomentum;

    [Resettable] public static MoveState moveState { get; private set; }
    [Resettable] public static GrindState grindState { get; private set; }
    [Resettable] public static VerticalState verticalState { get; private set; }
    [Resettable] public static JumpState jumpState { get; private set; }

    public static Action<MoveState> OnMoveStateChanged;
    public static Action<GrindState> OnGrindStateChanged;
    public static Action<VerticalState> OnVerticalStateChanged;
    public static Action<JumpState> OnJumpStateChanged;

    bool isResetting;

    Resetter resetter;

    protected virtual void Start()
    {
        moveState = MoveState.FullControlMove;
        grindState = GrindState.None;
        verticalState = VerticalState.Grounded;
        jumpState = JumpState.None;
        ExternalMomentum = Vector3.zero;

        Rb = ReferenceManager.Instance.PlayerRb;
        splineFollower = ReferenceManager.Instance.PlayerSF;
        CameraTransform = ReferenceManager.Instance.MainCamTransform;
        resetter = ReferenceManager.Instance.Resetter;
        resetter.OnReset += ResetStats;
    }

    protected virtual void OnDisable()
    {
        resetter.OnReset -= ResetStats;
    }

    protected void SetMoveStateTo(MoveState _moveState)
    {
        moveState = _moveState;
        OnMoveStateChanged?.Invoke(_moveState);

        if (moveState is MoveState.OnRail)
        SetGrindStateTo(GrindState.GettingOnRail);
        else if (moveState is MoveState.FullControlMove && grindState != GrindState.RailSwitching)
        SetGrindStateTo(GrindState.None);
    }

    protected void SetGrindStateTo(GrindState _grindState)
    {
        grindState = _grindState;
        OnGrindStateChanged?.Invoke(_grindState);
        if (grindState is GrindState.None && moveState!= MoveState.FullControlMove)
        SetMoveStateTo(MoveState.FullControlMove);
    }

    protected void SetVerticalStateTo(VerticalState _verticalState)
    {
        verticalState = _verticalState;
        OnVerticalStateChanged?.Invoke(_verticalState);
    }

    protected void SetJumpStateTo(JumpState _jumpState)
    {
        jumpState = _jumpState;
        OnJumpStateChanged?.Invoke(_jumpState);
    }

    void ResetStats()
    {
        isResetting = true;
        Invoke(nameof(setResetToFalse), 0.1f);
        ExternalMomentum = Vector3.zero;
        moveState = MoveState.FullControlMove;
        grindState = GrindState.None;
        verticalState = VerticalState.Grounded;
        jumpState = JumpState.None;
    }

    void setResetToFalse() => isResetting = false;

 
    protected void SwitchingRigidBodyInterpolation(MoveState state)
    {
        if (isResetting) return;

        if (state is MoveState.OnRail)
        {
            Rb.interpolation = RigidbodyInterpolation.None;
        }
        else { Rb.interpolation = RigidbodyInterpolation.Extrapolate; }
    }

}