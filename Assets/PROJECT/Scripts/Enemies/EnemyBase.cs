using CustomInspector;
using UnityEngine;
using System;

public class EnemyBase : Targetable, IResettable
{
    [HideInInspector] protected EnemyTag player;
    [Resettable] public bool isFiring = false;
    [Resettable][SerializeField] protected bool allowStunOnDeath = false;
    [Resettable][HideInInspector] protected bool isStunned = false;

    [HorizontalLine("Projectile Settings ", 2, FixedColor.CherryRed)]
    public int projectileSpeed = 5;
    public float projectileLifetime = 5f;
    public int projectileDamage = 1;

    [HorizontalLine("Projectile Activation Settings ", 2, FixedColor.CherryRed)]
    public Color gizmoColor = Color.red;
    public float rotationSpeed = 5f;
    public float fireRate = 1f;
    public float playerRange = 10f; // Range within which players are detected

    [HorizontalLine("References ", 2, FixedColor.CherryRed)]
    public GameObject bulletPrefab;
    public Transform aimHolder;
    public Transform firePoint;

    [Resettable] public Rigidbody rb => GetComponent<Rigidbody>();
    [Resettable] Transform t => transform;
    [Resettable] GameObject go => gameObject;

    public Action<bool> OnAllowEnemyStunning;

    private void OnEnable()
    {
        EnemyHealth.OnEnemyDeath += StunOnDeath;
    }
    private void OnDisable()
    {
        EnemyHealth.OnEnemyDeath -= StunOnDeath;
    }

    void Start()
    {
        player = FindObjectOfType<EnemyTag>();
        isFiring = false;
        OnAllowEnemyStunning?.Invoke(allowStunOnDeath);
        int poolsize = 50;
        ObjectPool.InitializePool(bulletPrefab, poolsize);
    }

    public virtual void  Rotate()
    {
        Transform closestPlayer = player.transform;
        if (aimHolder != null) { aimHolder.LookAt(closestPlayer); }
        Vector3 targetDir = closestPlayer.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        transform.rotation = Quaternion.Euler(0f, rotation.y, 0f);
    }

    public bool PlayerInRange()
    {
        if (Vector3.Distance(transform.position, player.transform.position) < playerRange)
        {
            return true;
        }
        return false;
    }

    protected void StunOnDeath(GameObject enemy)
    {
        if (enemy == gameObject)
        {
            isStunned = true;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    public bool isPlaying(Animator anim, string stateName)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
                anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            return true;
        else
            return false;
    }
}