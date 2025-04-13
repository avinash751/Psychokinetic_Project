using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using CustomInspector;

public class CameraAimAndFollowAttachmentChange : MonoBehaviour
{
    [HorizontalLine("CinemachineReferences", 2, FixedColor.BabyBlue)]
    [SerializeField] CinemachineFreeLook mouseAndKeyboardCamera;
    [SerializeField] CinemachineFreeLook controllerCamera;
    [SerializeField][ReadOnly] CinemachineRecomposer controllerCinemachineRecomposer;
    [SerializeField][ReadOnly] CinemachineRecomposer mouseCinemachineRecomposer;



    [HorizontalLine("Rail Grinding Camera  Attachment Values", 2, FixedColor.BabyBlue)]
    [SerializeField][Range(0, 1)] float railAimAttachment;
    [SerializeField][Range(0, 1)] float railLookAtAttachment;
    [SerializeField] float railTransitionDuration;

    [HorizontalLine("Falling State Camera  Attachment Values", 2, FixedColor.BabyBlue)]
    [SerializeField][Range(0, 1)] float fallingAimAttachment;
    [SerializeField][Range(0, 1)] float fallingFollowAttachment;

    [HorizontalLine("Ground Movement Camera Attachment Values", 2, FixedColor.BabyBlue)]
    [SerializeField][Range(0, 1)] float GroundMoveAimAttachment;
    [SerializeField][Range(0, 1)] float GroundMoveLookAtAttachment;
    [SerializeField] float GroundMoveTransitionDuration;



    private void OnEnable()
    {
        // PlayerMovement.OnMoveStateChanged += SetCameraValuesBasedOnMoveState;
        PlayerMovement.OnGrindStateChanged += SetCameraValuesBasedOnGrindState;
        PlayerMovement.OnVerticalStateChanged += SetCameraValuesBasedOnVerticalState;
    }

    private void OnDisable()
    {
        // PlayerMovement.OnMoveStateChanged -= SetCameraValuesBasedOnMoveState;
        PlayerMovement.OnGrindStateChanged -= SetCameraValuesBasedOnGrindState;
        PlayerMovement.OnVerticalStateChanged -= SetCameraValuesBasedOnVerticalState;
    }

    private void Start()
    {
        controllerCinemachineRecomposer = mouseAndKeyboardCamera.GetComponent<CinemachineRecomposer>();
        mouseCinemachineRecomposer = controllerCamera.GetComponent<CinemachineRecomposer>();
    }

    void SetCameraValuesBasedOnGrindState(GrindState state)
    {
        if (state is GrindState.RailGrinding)
        {
            StartCoroutine(SmoothlyTransitionLookAtAndFollowValues(fallingAimAttachment, fallingFollowAttachment,
            railAimAttachment, railLookAtAttachment, railTransitionDuration));
        }
    }

    void SetCameraValuesBasedOnVerticalState(VerticalState state)
    {
        if (state is VerticalState.Falling || state is VerticalState.InAir)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyTransitionLookAtAndFollowValues(mouseCinemachineRecomposer.m_LookAtAttachment,mouseCinemachineRecomposer.m_FollowAttachment,
            fallingAimAttachment,fallingFollowAttachment,0.025f));
            return;
        }
        if (state is VerticalState.Grounded)
        {
            StartCoroutine(SmoothlyTransitionLookAtAndFollowValues(fallingAimAttachment, fallingFollowAttachment,
            GroundMoveAimAttachment, GroundMoveLookAtAttachment, GroundMoveTransitionDuration));
        }
       
    }

    void SetLookAtAndFollowValues(float aimValue, float followValue)
    {
        mouseCinemachineRecomposer.m_LookAtAttachment = aimValue;
        mouseCinemachineRecomposer.m_FollowAttachment = followValue;

        controllerCinemachineRecomposer.m_LookAtAttachment = aimValue;
        controllerCinemachineRecomposer.m_FollowAttachment = followValue;
    }

    IEnumerator SmoothlyTransitionLookAtAndFollowValues(float startAim, float startFollow, float aimValue, float followValue, float duration)
    {
        float elapsedTime = 0;
        float currentAimValue = startAim;
        float currentFollowValue = startFollow;

        while (elapsedTime < duration)
        {
            currentAimValue = Mathf.Lerp(currentAimValue, aimValue, elapsedTime / duration);
            currentFollowValue = Mathf.Lerp(currentFollowValue, followValue, elapsedTime / duration);
            SetLookAtAndFollowValues(currentAimValue, currentFollowValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

}
