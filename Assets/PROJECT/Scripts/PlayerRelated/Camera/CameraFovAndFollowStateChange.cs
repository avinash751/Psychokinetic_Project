using CustomInspector;
using UnityEngine;
using Cinemachine;
using VInspector;
using UnityEngine.UIElements;
using System.Collections;
using System;




public class CameraFovAndFollowStateChange : MonoBehaviour
{
    [HorizontalLine("References", 2, FixedColor.BabyBlue)]
    [SerializeField][ReadOnly] Rigidbody playerRb;
    [SerializeField][ReadOnly] CinemachineFreeLook keyboardAnsMouseCamera;
    [SerializeField][ReadOnly] CinemachineFreeLook controllerCamera;
    [SerializeField][ReadOnly] Transform camFollowTarget;

    [HorizontalLine("Camera State Settings", 2, FixedColor.BabyBlue)]
    [VInspector.Foldout("Idle Camera settings")]
    [SerializeField] Vector3 idleTargetOffset;
    [SerializeField] float idleTargetFov;
    [SerializeField] AnimationCurve idleTransitionCurve;
    [SerializeField] float idleTransitionDuration;

    /*[VInspector.Foldout("Run Camera settings")]
    [SerializeField] Vector3 runTargetOffset;
    [SerializeField] float runTargetFov;
    [SerializeField] AnimationCurve runTransitionCurve;
    [SerializeField] float runTransitionDuration; */

    /*[VInspector.Foldout("Jump Camera settings")]
    [SerializeField] Vector3 jumpTargetOffset;
    [SerializeField] float jumpTargetFov;
    [SerializeField] AnimationCurve jumpTransitionCurve;
    [SerializeField] float jumpTransitionDuration;*/

    [VInspector.Foldout("Grind Camera settings")]
    [SerializeField] Vector3 grindTargetOffset;
    [SerializeField] float grindTargetFov;
    [SerializeField] AnimationCurve grindTransitionCurve;
    [SerializeField] float grindTransitionDuration;

    [HorizontalLine("Debug", 2, FixedColor.BabyBlue)]
    bool idlePlayedOnce;

    private void Start()
    {
        playerRb = ReferenceManager.Instance.PlayerRb;
        keyboardAnsMouseCamera = ReferenceManager.Instance.keyboardAnsMouseCamera;
        controllerCamera = ReferenceManager.Instance.controllerCamera;
        camFollowTarget = keyboardAnsMouseCamera.Follow;
    }

    private void OnEnable()
    {

        //PlayerMovement.OnVerticalStateChanged += JumpCameraTransition;
        PlayerMovement.OnMoveStateChanged += GrindStateCameraTransition;

    }

    private void OnDisable()
    {
        //PlayerMovement.OnVerticalStateChanged -= JumpCameraTransition;
        PlayerMovement.OnMoveStateChanged -= GrindStateCameraTransition;
    }

    private void Update()
    {
        IdleAndRunCameraTransition();
    }

    void IdleAndRunCameraTransition()
    {
        if (PlayerMovement.moveState != MoveState.FullControlMove) return;
        if (idlePlayedOnce) return;
        StopAllCoroutines();
        StartCoroutine(LerpCameraValuesBasedOnState(idleTargetOffset, idleTargetFov, idleTransitionCurve, idleTransitionDuration));
        idlePlayedOnce = true;
    }

   /* void JumpCameraTransition(VerticalState state)
    {
        if (state != VerticalState.InAir) return;
        StopAllCoroutines();
        StartCoroutine(LerpCameraValuesBasedOnState(jumpTargetOffset, jumpTargetFov, jumpTransitionCurve, jumpTransitionDuration));
        idlePlayedOnce = false;
        Debug.Log("Jump");
    }*/

    void GrindStateCameraTransition(MoveState state)
    {
        if (state != MoveState.OnRail) return;
        StopAllCoroutines();
        StartCoroutine(LerpCameraValuesBasedOnState(grindTargetOffset, grindTargetFov, grindTransitionCurve, grindTransitionDuration));
        idlePlayedOnce = false;
        Debug.Log("grind");

    }

    IEnumerator LerpCameraValuesBasedOnState(Vector3 targetOffset, float targetFov, AnimationCurve transitionCurve, float transitionDuration)
    {
        float time = 0;
        Vector3 startPosition = camFollowTarget.localPosition;
        float startFov = keyboardAnsMouseCamera.m_Lens.FieldOfView;

        while (time < transitionDuration)
        {
            time += Time.deltaTime;
            float t = transitionCurve.Evaluate(time / transitionDuration);
            camFollowTarget.localPosition = Vector3.Lerp(startPosition, targetOffset, t);
            keyboardAnsMouseCamera.m_Lens.FieldOfView = Mathf.Lerp(startFov, targetFov, t);
            controllerCamera.m_Lens.FieldOfView = Mathf.Lerp(startFov, targetFov, t);
            yield return null;
        }
    }
}
