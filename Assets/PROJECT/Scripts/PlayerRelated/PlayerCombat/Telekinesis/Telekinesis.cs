using CustomInspector;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using VInspector;
using Random = UnityEngine.Random;

public class Telekinesis : MonoBehaviour, IResettable
{
    [Resettable] public TelekinesisState State { get; private set; } = TelekinesisState.Idle;
    Coroutine grabCoroutine;

    [VInspector.Foldout("Debug")]
    [SerializeField] bool showTelekinesisRange = false;
    [SerializeField] bool showOrbitDistance = false;
    [SerializeField] bool showLockOnRange = false;
    [EndFoldout]

    [HorizontalLine("Grabbing", 1, FixedColor.Gray)]
    [SerializeField] float bufferZone = 0.01f;
    [SerializeField] float telekinesisRange = 10f;
    [SerializeField, Range(0.1f, 100f)] float slowdownPercentage = 50f;
    [SerializeField] float timeToReachPlayer = 0.5f;
    [SerializeField] AnimationCurve grabAnimationCurve;
    [SerializeField] float curveModifier = 4.5f;

    [HorizontalLine("Holding", 1, FixedColor.Gray)]
    [SerializeField] Transform orbitPosition;
    [SerializeField] float orbitDistance = 1f;
    [SerializeField] float orbitSpeed = 50f;
    [SerializeField] float axisChangeSpeed = 0.5f;
    [SerializeField] float axisChangeInterval = 5f;
    [SerializeField] Image targetUI;
    Vector3 nextTargetDirection;
    float axisChangeTimer = 0f;
    Vector3 orbitAxis = Vector3.up;
    Vector3 currentSteeringDirection;

    [HorizontalLine("Throwing", 1, FixedColor.Gray)]
    [SerializeField] float throwForce = 10f;
    [SerializeField] float lockOnRange = 30f;
    [SerializeField] float screenCenterThreshold = 0.4f;
    [SerializeField] float minTimeToReachTarget = 0.2f;
    [SerializeField] float maxTimeToReachTarget = 0.8f;
    [SerializeField] AnimationCurve throwAnimationCurve;

    TelekineticObject[] telekineticObjects;
    [Resettable] TelekineticObject currentObject;
    [Resettable] (Targetable target, Vector3 viewportPoint) currentTarget;

    [SerializeField] RenderTexture texture;

    string grabSoundKey = "TeleGrab";
    string throwSoundKey = "TeleThrow";

    private void Start()
    {
        currentSteeringDirection = Random.onUnitSphere * orbitDistance;
        nextTargetDirection = Random.onUnitSphere * orbitDistance;
        telekineticObjects = FindObjectsOfType<TelekineticObject>();
        if (Targeting.Instance == null) { new GameObject("Targeting System").AddComponent<Targeting>(); }
    }

    void Update()
    {
        if (Time.timeScale == 0) { return; }

        if (State == TelekinesisState.Idle) { GetTelekineticObject(); }
        
        if (currentObject != null) { currentObject.ToggleOutline(true); }

        HandleInput();

        if (currentObject)
        {
            currentTarget = Targeting.Instance.GetClosestTargetOnScreen(Camera.main, transform.position, lockOnRange, screenCenterThreshold);
            if (State == TelekinesisState.Holding) OrbitHoverPosition();
        }
        else
        {
            State = TelekinesisState.Idle;
        }

        DisplayTargetUI();
    }

    void HandleInput()
    {
        if (InputManager.GetKeyState("Telekinesis"))
        {
            if (State == TelekinesisState.Idle)
            {
                Grab();
            }
            else
            {
                Throw();
            }
        }
    }

    void Grab()
    {
        if (currentObject != null)
        {
            currentObject.rb.useGravity = false;
            EventManager.Invoke(EventType.Telekinesis, GetType(), currentObject);
            EventManager.Invoke(EventType.VFX, GetType(), true, currentObject.gameObject);
            State = TelekinesisState.Grabbing;
            AudioManager.Instance?.PlayAudio(grabSoundKey); 
            grabCoroutine = StartCoroutine(ManipulateObject(orbitPosition, grabAnimationCurve, timeToReachPlayer, TelekinesisState.Holding));
        }
    }

    void Throw()
    {
        Targetable targetable = currentTarget.target;
        currentObject.transform.parent = null;
        AudioManager.Instance?.PlayAudio(throwSoundKey);
        if (targetable == null)
        {
            currentObject.rb.useGravity = true;
            currentObject.rb.constraints = RigidbodyConstraints.None;
            currentObject.EnableEffect(null, throwForce);
            Vector3 throwDirection = Camera.main.transform.forward;
            currentObject.rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
            StopCoroutine(grabCoroutine);
            
        }
        else
        {
            StopCoroutine(grabCoroutine);
            float time = Mathf.Lerp(minTimeToReachTarget, maxTimeToReachTarget, Vector3.Distance(currentObject.transform.position, targetable.transform.position) / lockOnRange);
            Coroutine throwRoutine = StartCoroutine(ManipulateObject(targetable.transform, throwAnimationCurve, time));
            currentObject.EnableEffect(targetable.transform, throwForce, throwRoutine);
            currentObject = null;
            State = TelekinesisState.Idle;
        }
        State = TelekinesisState.Idle;
        EventManager.Invoke(EventType.Telekinesis, GetType(), currentObject);
        currentObject = null;
       
    }

    void DisplayTargetUI()
    {
        bool shouldDisplay = (State == TelekinesisState.Holding || State == TelekinesisState.Grabbing) && currentTarget.target != null;
        targetUI.gameObject.SetActive(shouldDisplay);
        if (shouldDisplay)
        {
            Vector3 worldPos = currentTarget.target.transform.position;
            Vector2 screenPoint = Camera.main.ViewportToScreenPoint(currentTarget.viewportPoint);

            screenPoint = new Vector2((screenPoint.x), (screenPoint.y));
            
            targetUI.rectTransform.position = screenPoint;
            targetUI.rectTransform.localScale = Vector3.Lerp(Vector3.one * 2f, Vector3.one * 0.5f, Vector3.Distance(worldPos, transform.position) / lockOnRange);
        }
    }

    void GetTelekineticObject()
    {
        if (currentObject != null && Vector3.Distance(currentObject.transform.position, transform.position) > telekinesisRange) 
        {
            currentObject.ToggleOutline(false);
            currentObject = null;
        }

        TelekineticObject nearestObject = null;
        float closestToScreenCenter = float.MaxValue;

        foreach (var obj in telekineticObjects)
        {
            if (!obj.gameObject.activeSelf || Vector3.Distance(obj.transform.position, transform.position) > telekinesisRange || obj.Thrown)
            {
                continue;
            }

            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(obj.transform.position);
            if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
            {
                float distanceToScreenCenter = Vector2.Distance(new Vector2(0.5f, 0.5f), viewportPoint);
                if (closestToScreenCenter > distanceToScreenCenter)
                {
                    closestToScreenCenter = distanceToScreenCenter;
                    nearestObject = obj;
                }
            }
        }

        if (currentObject != nearestObject)
        {
            if (currentObject != null)
            {
                float currentObjectDistance = Vector2.Distance(new Vector2(0.5f, 0.5f), Camera.main.WorldToViewportPoint(currentObject.transform.position));
                if (closestToScreenCenter < currentObjectDistance - bufferZone)
                {
                    currentObject.ToggleOutline(false);
                    currentObject = nearestObject;
                }
            }
            else
            {
                currentObject = nearestObject;
            }
        }
    }

    private IEnumerator ManipulateObject(Transform endPosition, AnimationCurve animationCurve, float timeToReach, TelekinesisState state = TelekinesisState.Idle)
    {
        TelekineticObject obj = currentObject;
        if (obj != null)
        {
            Vector3 startPosition = obj.transform.position;
            Vector3 controlPoint1 = startPosition + Random.onUnitSphere * curveModifier;
            Vector3 controlPoint2 = endPosition.position - Random.onUnitSphere * curveModifier;
            float time = 0;
            while (time < timeToReach)
            {
                while (Time.timeScale == 0) { yield return null; }
                float adjustedTimeScale = Time.timeScale < 1 ? Time.timeScale * (1 / (slowdownPercentage / 100)) : Time.timeScale;
                time += Time.deltaTime / (adjustedTimeScale);
                float t = animationCurve.Evaluate(time / timeToReach);
                Vector3 targetPosition = CalculateBezierPoint(t, startPosition, controlPoint1, controlPoint2, endPosition.position);
                obj.transform.position = targetPosition;
                yield return null;
            }
            obj.rb.useGravity = true;
            if (state != TelekinesisState.Idle)
            {
                this.State = state;
                obj.rb.constraints = RigidbodyConstraints.FreezeRotation;
                obj.rb.velocity = Vector3.zero;
                obj.transform.parent = orbitPosition;
            }
            else
            {
                obj.rb.AddForce((endPosition.position - obj.transform.position).normalized * throwForce, ForceMode.VelocityChange);
            }
        }
    }

    Vector3 CalculateBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0;
        p += 3 * uu * t * p1;
        p += 3 * u * tt * p2;
        p += ttt * p3;

        return p;
    }

    void OrbitHoverPosition()
    {
        if (currentObject != null)
        {
            Vector3 toHoverPosition = orbitPosition.position - currentObject.transform.position;
            Vector3 desiredPosition = orbitPosition.position - toHoverPosition.normalized * orbitDistance;
            Vector3 orbitDirection = Vector3.Cross(toHoverPosition, orbitAxis).normalized;
            float circumference = 2 * Mathf.PI * orbitDistance;
            float orbitTime = circumference / orbitSpeed;
            Vector3 tangentialVelocity = orbitDirection * (circumference / orbitTime);
            float distanceFromDesired = (desiredPosition - currentObject.transform.position).magnitude;
            Vector3 centeringForceDirection = (desiredPosition - currentObject.transform.position).normalized;
            Vector3 centeringVelocity = distanceFromDesired * orbitSpeed * centeringForceDirection;
            currentObject.rb.velocity = centeringVelocity + tangentialVelocity;
            currentObject.transform.Rotate(orbitAxis, orbitSpeed);
            orbitAxis = Vector3.Lerp(orbitAxis, currentSteeringDirection, axisChangeSpeed * Time.fixedDeltaTime);

            if (axisChangeTimer >= axisChangeInterval)
            {
                nextTargetDirection = Random.onUnitSphere * orbitDistance;
                axisChangeTimer = 0;
            }
            currentSteeringDirection = Vector3.Lerp(currentSteeringDirection, nextTargetDirection.normalized, axisChangeSpeed * Time.fixedDeltaTime);
            axisChangeTimer += Time.fixedDeltaTime;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showTelekinesisRange)
        {
            Handles.color = new(0, 0, 1, 0.2f);
            Handles.DrawSolidDisc(transform.position, transform.up, telekinesisRange);
        }

        if (showOrbitDistance)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(orbitPosition.position, orbitDistance);
        }

        if (showLockOnRange)
        {
            Handles.color = new(0, 1, 0, 0.2f);
            Handles.DrawSolidDisc(transform.position, transform.up, lockOnRange);
        }
    }
#endif
}

public enum TelekinesisState
{
    Idle,
    Grabbing,
    Holding,
}