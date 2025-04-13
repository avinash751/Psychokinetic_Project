using CustomInspector;
using VInspector;
using UnityEngine.UI;
using UnityEngine;
using System.Collections;

public class TimeSlowdown : MonoBehaviour, IResettable
{
    [VInspector.Foldout("Debug")]
    [SerializeField, ReadOnly, Resettable] float slowTimeRemaining;
    [SerializeField, ReadOnly, Resettable] bool onCooldown = false;
    [SerializeField, ReadOnly, Resettable] bool canActivate = false;
    [SerializeField, ReadOnly, Resettable] bool slowing = false;
    [EndFoldout]

    [HorizontalLine("Settings", 1, FixedColor.Gray)]
    [SerializeField, Range(1, 10)] int segments = 2;
    [SerializeField, Range(0, 5f)] float timePerSegment = 3f;
    [SerializeField, Range(0, 100f)] float slowdownPercentage = 50f;
    [SerializeField, Range(1, 10)] int rechargableSegments = 1;
    [SerializeField, Range(0, 1f)] float activationCooldown = 0.5f;
    [SerializeField, Range(0, 1f)] float timeScaleChangeDuration = 0.5f;

    [HorizontalLine("UI", 1, FixedColor.Gray)]
    [SerializeField] Transform slowdownUIParent;
    [SerializeField] GameObject segmentPrefab;

    Coroutine timeScaleCoroutine;
    SlowdownEffects effects;
    Image[] spawnedSegments;
    float fixedDeltaTime;
    float activationTime = 0f;
    [Resettable] bool paused = true;

    private void Start()
    {
        fixedDeltaTime = Time.fixedDeltaTime;
        slowTimeRemaining = segments * timePerSegment;
        rechargableSegments = Mathf.Clamp(rechargableSegments, 0, segments);
        effects = FindObjectOfType<SlowdownEffects>();
        GameManager.Instance.onPause.AddListener(() => GamePaused(true));
        GameManager.Instance.OnLevelCompleted.AddListener(() => GamePaused(true));
        GameManager.Instance.onLose.AddListener(() => GamePaused(true));
        GameManager.Instance.onReset.AddListener(() => GamePaused(true));
        GameManager.Instance.onStart.AddListener(() => GamePaused(false));
        GameManager.Instance.onResume.AddListener(() => GamePaused(false));
        SpawnUI();
    }

    private void SpawnUI()
    {
        spawnedSegments = new Image[segments];
        for (int i = 0; i < segments; i++)
        {
            GameObject newSegment = Instantiate(segmentPrefab, slowdownUIParent);
            spawnedSegments[i] = newSegment.transform.GetChild(0).GetComponent<Image>();
            spawnedSegments[i].color = (i < rechargableSegments) ? Color.red : Color.cyan;
        }
    }

    private void Update()
    {
        if (paused) { return; }
        
        HandleActivationCooldown();

        if (InputManager.GetKeyState("Slow Down" , true) && slowTimeRemaining > 0 && !onCooldown && canActivate)
        {
            HandleSlowdown();
        }
        else if (slowing)
        {
            ResetTimeScale();
        }

        RechargeSlowdown();
        UpdateSlowdownUI();
    }

    private void HandleActivationCooldown()
    {
        if (!canActivate)
        {
            activationTime -= Time.deltaTime;
            canActivate = activationTime <= 0;
        }
    }

    private void HandleSlowdown()
    {
        if (!slowing)
        {
            slowing = true;
            if (timeScaleCoroutine != null)
            {
                StopCoroutine(timeScaleCoroutine);
            }
            timeScaleCoroutine = StartCoroutine(ChangeTimeScale(slowdownPercentage / 100, timeScaleChangeDuration));
        }
        
        slowTimeRemaining -= Time.unscaledDeltaTime;
        slowTimeRemaining = Mathf.Max(0, slowTimeRemaining);

        onCooldown = slowTimeRemaining == 0;
    }

    private void ResetTimeScale()
    {
        slowing = false;
        if (timeScaleCoroutine != null)
        {
            StopCoroutine(timeScaleCoroutine);
        }
        timeScaleCoroutine = StartCoroutine(ChangeTimeScale(1, timeScaleChangeDuration));
        canActivate = false;
        activationTime = activationCooldown;
    }

    IEnumerator ChangeTimeScale(float targetScale, float duration)
    {
        float startScale = Time.timeScale;
        float timer = 0;

        while (timer < duration)
        {
            if (paused) 
            {
                Time.timeScale = 0;
                yield return null; 
            }
            Time.timeScale = Mathf.Lerp(startScale, targetScale, timer / duration);
            Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
            if (effects != null) { effects.UpdateEffects(slowdownPercentage / 100); }
            timer += Time.unscaledDeltaTime;
            yield return null;
        }

        Time.timeScale = targetScale;
        Time.fixedDeltaTime = fixedDeltaTime * Time.timeScale;
        if (effects != null) { effects.UpdateEffects(slowdownPercentage / 100); }
    }

    private void RechargeSlowdown()
    {
        if (!slowing && slowTimeRemaining < timePerSegment * rechargableSegments)
        {
            slowTimeRemaining += Time.deltaTime;
            slowTimeRemaining = Mathf.Min(slowTimeRemaining, timePerSegment * rechargableSegments);
            onCooldown = slowTimeRemaining < timePerSegment;
        }
    }

    void UpdateSlowdownUI()
    {
        float filledSegmentValue = slowTimeRemaining / timePerSegment;

        for (int i = 0; i < segments; i++)
        {
            if (i < filledSegmentValue)
            {
                spawnedSegments[i].fillAmount = filledSegmentValue - i;
            }
            else
            {
                spawnedSegments[i].fillAmount = 0;
            }
        }
    }

    void GamePaused(bool paused)
    {
        this.paused = paused;
        if (paused)
        {
            if (timeScaleCoroutine != null) { StopCoroutine(timeScaleCoroutine); }
        }
        else if (Time.timeScale < 1)
        {
            slowing = Input.GetKey(KeyCode.Mouse1) && slowTimeRemaining > 0 && !onCooldown && canActivate;
            float targetScale = slowing ? slowdownPercentage / 100 : 1;
            float t = Mathf.Abs((Time.timeScale - targetScale) / (1 - slowdownPercentage / 100));
            float timeToScale = Mathf.Lerp(0, timeScaleChangeDuration, Mathf.Clamp01(t));
            StartCoroutine(ChangeTimeScale(targetScale, timeToScale));
        }
    }

    public static float FixedTimeModifier() => 0.02f / Time.fixedDeltaTime;

    public static float TimeScaleModifier() => 1 / Time.timeScale;
}