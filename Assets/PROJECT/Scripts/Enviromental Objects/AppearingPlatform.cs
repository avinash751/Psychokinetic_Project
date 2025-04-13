
using UnityEngine;
using static UnityEngine.ParticleSystem;
using CustomInspector;
using UnityEngine.Events;

public class AppearingPlatform : EnviromentalAid
{
    [SerializeField] MinMaxCurve moveCurve;
    [SerializeField] Vector3 targetPosition;
    [SerializeField] Vector3 startPosition;
    [SerializeField]bool useTargetXAnsZAsStartPosition;
    [SerializeField] float moveDuration;
    [SerializeField] float distanceToActivate;
    [SerializeField][ReadOnly] float currentDistance;
    [Resettable]TimerUtilities timer;
    [SerializeField] UnityEvent OnPlatformAppear;


    private void OnValidate()
    {
        targetPosition = transform.localPosition;
        startPosition = useTargetXAnsZAsStartPosition ? new Vector3(targetPosition.x, startPosition.y, targetPosition.z) : startPosition;
    }

    private void Awake()
    {
      
        transform.localPosition= startPosition;
        timer = new TimerUtilities();
        timer.InitializeNormalStopWatch(moveDuration);
    }

    void Update()
    {
        currentDistance= Vector3.Distance(player.transform.position, transform.position);
        if (currentDistance <= distanceToActivate)
        {
            isActivated = true;
        }

        if(!isActivated) { return; }

        float elapsedTime = timer.UpdateStopWatch(false).elapsedTime;
        float t = elapsedTime / moveDuration;

        if(elapsedTime <= moveDuration)
        {
            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, moveCurve.Evaluate(t, Random.value));
            return;
        } 
        OnPlatformAppear.Invoke();
    }
}
