using UnityEngine;
using CustomInspector;
using MoreMountains.Feedbacks;

public class ElCockoracha : EnviromentalAid, IResettable
{
    [HorizontalLine("Ticking Settings", 3, FixedColor.IceWhite)]
    [SerializeField] Color startColor;
    [SerializeField] Color endColor;
    [SerializeField] float startFrequency;
    [SerializeField] float endFrequency;
    [SerializeField] float shakeAmplitude;
    [Range(0, 1)][SerializeField] float percentageToTriggerScaling;
    [SerializeField] MMF_Player scalingFeedback;
    [SerializeField] MMF_Player breathingFeedback;

    [HorizontalLine("Explosion Settings", 3, FixedColor.IceWhite)]
    [SerializeField] float countdownTime = 10f;
    [SerializeField] int damage = 1;
    [SerializeField] float knockbackStrength = 100f;
    [SerializeField] float dragOffset = 5f;
    [SerializeField] float playerRange = 10f;
    [SerializeField] LayerMask playerLayer;
    [SerializeField] LayerMask objectLayer;

    [HorizontalLine("Jump Off Settings", 3, FixedColor.IceWhite)]
    [SerializeField] float bounceSpeed = 140f;
    [SerializeField] float upwardForce = 60f;

    [Resettable] Transform t => transform;
    [Resettable] GameObject go => gameObject;
    [SerializeField][Resettable] float currentTime = 0f;


    [HorizontalLine("Debug Settings", 3, FixedColor.IceWhite)]
    [SerializeField] Color gizmoColor = Color.yellow;
    [SerializeField] bool debugMode = false;
    [SerializeField][ReadOnly] float currentDistance;
    [SerializeField][ReadOnly] Vector3 initialPosition;


    protected override void Start()
    {
        base.Start();
        initialPosition = transform.position;
    }
    void Update()
    {
        if (!PlayerInRange())
        {
            currentTime -= Time.deltaTime;
            currentTime = Mathf.Max(currentTime, 0f);
            return;
        }

        currentTime += Time.deltaTime;
        if (padRenderer == null) return;

        if (breathingFeedback.IsPlaying)
        {
            breathingFeedback.StopFeedbacks();
        }

        VisualFeedbacksWhenPlayerInRange();

        if (currentTime >= countdownTime)
        {
            Explode(playerLayer);
            currentTime = 0f;
        }
    }

    private void VisualFeedbacksWhenPlayerInRange()
    {
        float t = currentTime / countdownTime;
        float frequency = Mathf.Lerp(startFrequency, endFrequency, t);

        float percentage = Mathf.InverseLerp(0, countdownTime, currentTime);
        if (percentage >= percentageToTriggerScaling)
        {
            padRenderer.material.color = endColor;
            if (!scalingFeedback.IsPlaying)
            scalingFeedback.PlayFeedbacks();
            return;
        }
        Color color = Color.Lerp(startColor, endColor, (Mathf.Sin(frequency * currentTime)));
        padRenderer.material.color = color;
        transform.position = initialPosition + ((new Vector3(1, 0, 1) * Mathf.Sin(frequency * currentTime)) * shakeAmplitude);
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (playerRB != null)
        {
            if (PlayerMovement.jumpState is JumpState.Stomping || PlayerMovement.jumpState is JumpState.PerfectStomp)
            {
                float force = Mathf.Sqrt(bounceSpeed * Mathf.Abs(Physics.gravity.y) * playerRB.mass);
                Vector3 Direction = (playerRB.transform.up * force) + (Vector3.up * upwardForce);
                playerRB.GetComponent<Jump>().ResetDragAndGravity();
                playerRB.GetComponent<Stomp>().StopAllCoroutines();
                playerRB.AddForce(Direction, ForceMode.Impulse);
                Explode(objectLayer);
            }
            else if (collision.gameObject.TryGetComponent(out IDamageable damageable))
            {
                Vector3 randomDirection = Random.insideUnitSphere;
                playerRB.AddForce(randomDirection * knockbackStrength, ForceMode.Impulse);
                damageable.TakeDamage(damage);
                Explode(objectLayer);
            }

        }
    }

    void Explode(LayerMask layer)
    {
        PhysicsUtilities.KnockbackWithinRadius(transform, playerRange, knockbackStrength, dragOffset, layer);
        Collider[] objects = Physics.OverlapSphere(transform.position, playerRange, layer);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].TryGetComponent(out IDamageable damageable)) { damageable.TakeDamage(damage); }
        }
        gameObject.SetActive(false);
    }

    public bool PlayerInRange()
    {
        currentDistance = Vector3.Distance(transform.position, grindController.transform.position);
        if (currentDistance < playerRange)
        {
            return true;
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!debugMode) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }
}