using System.Collections;
using UnityEngine;

public class MissileMan : EnemyBase
{
    [SerializeField] ParticleSystem muzzleFlashVfx;

    void Update()
    {
        if(isStunned && allowStunOnDeath)
        {
            return;
        }

        if (PlayerInRange())
        {
            Rotate();
            if (!isFiring)
            {
                StartCoroutine(FireRoutine());
            }
        }
    }

    IEnumerator FireRoutine()
    {
        isFiring = true;

        while (PlayerInRange())
        {
            if (isStunned && allowStunOnDeath)
            {
                break;
            }
            Fire();
            yield return new WaitForSeconds(1f / fireRate);
        }

        isFiring = false;
    }

    void Fire()
    {
        GameObject _bullet = ObjectPool.GetObject(bulletPrefab, firePoint.position, firePoint.rotation);
        _bullet.GetComponent<ProjectileBase>().InitializeStats(projectileDamage, projectileSpeed, projectileLifetime);

        if (muzzleFlashVfx != null)
        {
            muzzleFlashVfx.Play();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, playerRange);
    }

}
