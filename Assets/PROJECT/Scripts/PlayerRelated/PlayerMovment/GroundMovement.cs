using UnityEngine;
using CustomInspector;

public class GroundMovement : PlayerMovement
{
    [SerializeField] MoveState _moveState;
    [HorizontalLine("Ground Movement Info", 3, FixedColor.Gray)]
    public float speed;

    [HorizontalLine("Slope Movement Info", 3, FixedColor.Gray)]
    [SerializeField] float minTolerableSlopeAngle = 10;
    [SerializeField] float maxTolerableSlopeAngle = 45;
    [SerializeField] float FullDownSlopeGravity = 30;
    [SerializeField] float SpeedToReachFinalSlopeGravity;

    [SerializeField] float upSlopeSpeed = 1.25f;
    [SerializeField] AnimationCurve slopeCurve;
    [SerializeField] float slopeAlignDuration = 0.5f;
    Resetter resetter;

    [HorizontalLine("Slope Debug Info", 3, FixedColor.Gray)]
    [ReadOnly][SerializeField] float currentSlopeAngle;
    [ReadOnly][SerializeField] float finalSlopeForce;
    [ReadOnly][SerializeField] float currentSlopeForce;


    float slopeLerpFactor;
    Vector3 moveForce;
    float time;
    bool isResetting;

    bool isWalking;

    string key = "Footsteps";

    private void OnEnable()
    {
        OnMoveStateChanged += SwitchingRigidBodyInterpolation;
        OnVerticalStateChanged += DisableWalking;
    }

    protected override void  OnDisable()
    {
        base.OnDisable();
        OnMoveStateChanged -= SwitchingRigidBodyInterpolation;
        OnVerticalStateChanged -= DisableWalking;
    }

    private void FixedUpdate()
    {
        _moveState = moveState;
        if (moveState is MoveState.OnRail)
        {
            isWalking = false;
            AudioManager.Instance?.StopAudio(key);
            return;
        };
        float xInput = InputManager.GetHorizontal();
        float zInput = InputManager.GetVertical();
        PlayerMoveDirection = new Vector3(xInput, 0, zInput);

        Vector3 cameraForwardDirection = CameraTransform.forward;
        cameraForwardDirection.y = 0;
        Vector3 cameraRightDirection = CameraTransform.right;

        CamAndPlayerMoveDirection = cameraForwardDirection * PlayerMoveDirection.z + cameraRightDirection * PlayerMoveDirection.x;
        CamAndPlayerMoveDirection = CamAndPlayerMoveDirection.normalized;
        moveForce = new Vector3(CamAndPlayerMoveDirection.x * speed, 0, CamAndPlayerMoveDirection.z * speed);

        if (CamAndPlayerMoveDirection != Vector3.zero)
        {
            Quaternion forwardRotation = Quaternion.LookRotation(CamAndPlayerMoveDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, forwardRotation, Time.smoothDeltaTime * 20);
        }
        SlopePhysics();
        Rb.AddForce(moveForce, ForceMode.Acceleration);
        if(verticalState is VerticalState.InAir || verticalState is VerticalState.Falling)
        {
            AudioManager.Instance.StopAudio(key);
            return;
        }
        if (!isWalking && PlayerMoveDirection != Vector3.zero)
        {
            isWalking = true;
            AudioManager.Instance?.PlayAudio(key);
           // Debug.Log("Footstep playing");
        }

        if (PlayerMoveDirection == Vector3.zero)
        {
            isWalking = false;
            AudioManager.Instance.StopAudio(key);
          //  Debug.Log("Stopping audio");
        }

    }

    void SlopePhysics()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 20f))
        {
            if (hit.collider.TryGetComponent(out PlayerMovement player))
            {
                return;
            }
            currentSlopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }

        bool isOnSlope = currentSlopeAngle > minTolerableSlopeAngle && currentSlopeAngle < maxTolerableSlopeAngle;
        SetMoveStateTo(isOnSlope ? MoveState.onSlope : MoveState.FullControlMove);

        if (moveState != MoveState.onSlope)
        {
            time = 0;
            Rb.useGravity = true;
            currentSlopeForce = 0;
            if (currentSlopeAngle < maxTolerableSlopeAngle)
            {
                RotationSlopeAlignment(hit);
            }
            return;
        }

        RotationSlopeAlignment(hit);

        CamAndPlayerMoveDirection = Vector3.ProjectOnPlane(CamAndPlayerMoveDirection, hit.normal).normalized;
        CamAndPlayerMoveDirection.Set(CamAndPlayerMoveDirection.x, 0, CamAndPlayerMoveDirection.z);

        if (Rb.velocity.y < 0)
        {
            float slopeForcePercent = Mathf.InverseLerp(-90, 90, currentSlopeAngle);
            finalSlopeForce = FullDownSlopeGravity * slopeForcePercent;
            currentSlopeForce = Mathf.Lerp(currentSlopeForce, finalSlopeForce, Time.smoothDeltaTime * SpeedToReachFinalSlopeGravity);

            Rb.AddForce(-hit.normal * finalSlopeForce, ForceMode.Acceleration);
            moveForce = CamAndPlayerMoveDirection * speed * upSlopeSpeed * 1.5f;

            Rb.useGravity = false;
            return;
        }

        moveForce = CamAndPlayerMoveDirection * speed * upSlopeSpeed;
        currentSlopeForce = 0;
        Rb.useGravity = false;
    }

    private void RotationSlopeAlignment(RaycastHit hit)
    {
        Quaternion targetRotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        if (time < slopeAlignDuration)
        {
            time += Time.deltaTime;
            slopeLerpFactor = slopeCurve.Evaluate(time / slopeAlignDuration);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, slopeLerpFactor);
        }
        else { transform.rotation = targetRotation; }
    }

    void DisableWalking(VerticalState state)
    {
        if (state == VerticalState.InAir)
        {
            isWalking = false;
        }
    }
}
