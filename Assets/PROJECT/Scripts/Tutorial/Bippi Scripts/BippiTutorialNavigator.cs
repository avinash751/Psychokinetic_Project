using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Dreamteck.Splines;
using VInspector;
using CustomInspector;
using System;
using System.Collections;

public class BippiTutorialNavigator : MonoBehaviour,IResettable
{
    [HorizontalLine("References", 2, FixedColor.BabyBlue)]
    [SerializeField][SelfFill] SplineFollower splineFollower;
    [SerializeField][ReadOnly] List<TutorialTask> tutorialTasks;

    [HorizontalLine("Settings", 2, FixedColor.BabyBlue)]
    [SerializeField] float moveSpeed;
    [SerializeField] float distanceToTriggerDamping;
    [SerializeField] float dampingDuration;
    double startPercentOnSpline;

    [HorizontalLine("Debug", 2, FixedColor.BabyBlue)]
    [SerializeField][ReadOnly] bool isArriving;

    public Action BippiIsOnTheMove;
    public Action OnBippiDestinationArrived;
    public Action OnBippiIdle;

    [Resettable] Transform bippiTransform => transform;

    private void OnEnable()
    {
        FindAllTutorialTaskTransforms();
        tutorialTasks.ForEach(x => x.TaskComplete += TriggerBippi);
        startPercentOnSpline = splineFollower._startPosition;
    }

    private void OnDisable()
    {
        tutorialTasks.ForEach(x => x.TaskComplete -= TriggerBippi);
        ReferenceManager.Instance.Resetter.OnReset -= OnBippiReset;
    }

    private void Start()
    {
        ReferenceManager.Instance.Resetter.OnReset += OnBippiReset;
    }

    public void TriggerBippi()
    {
        splineFollower.followSpeed = moveSpeed;
        isArriving = false;
        BippiIsOnTheMove?.Invoke();;
        Debug.Log("Bippi is moving");
    }
    public void FindAllTutorialTaskTransforms()
    {
        List<TutorialTask> tasks = FindObjectsOfType<TutorialTask>().ToList();
        tutorialTasks = tasks;
    }

    public void TriggerBippiToStopMoving()
    {
        if (isArriving) return;
        OnBippiDestinationArrived?.Invoke();
        Debug.Log("Bippi is stopping");
        isArriving = true;
        StartCoroutine(DampBippiToFullStop());
        
    }

    private IEnumerator DampBippiToFullStop()
    {
        float elapsedTime = 0;
        float currentSpeed = splineFollower.followSpeed;
        while (elapsedTime < dampingDuration)
        {
            elapsedTime += Time.deltaTime;
            float lerpedSpeed = Mathf.Lerp(currentSpeed, 0, elapsedTime / dampingDuration);
            splineFollower.followSpeed = lerpedSpeed;
            yield return null;
        }

        OnBippiIdle?.Invoke();
    }

    void OnBippiReset()
    {
        splineFollower.SetPercent(startPercentOnSpline);
        splineFollower.followSpeed = 0;
    }
}