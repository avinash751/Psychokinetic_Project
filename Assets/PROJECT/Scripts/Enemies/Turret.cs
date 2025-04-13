using CustomInspector;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Turret : EnemyBase
{
    [SerializeField] ParticleSystem muzzleFlashVfx;
    [SerializeField] Animator animController;
    [SerializeField] AnimationClip idleClip;
    [SerializeField] AnimationClip shootClip;

    public override void Rotate()
    {
        Transform closestPlayer = player.transform;
        Vector3 targetDir = closestPlayer.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(targetDir);
        Vector3 rotation = Quaternion.Lerp(aimHolder.localRotation, lookRotation, Time.deltaTime * rotationSpeed).eulerAngles;
        if (aimHolder != null) aimHolder.localRotation = Quaternion.Euler(-rotation.y, 0f, -rotation.x);
        if(!isPlaying(animController,shootClip.name))
        {
            animController.Play(shootClip.name,0,-1);
        }
    }

    void Update()
    {
        if (isStunned && allowStunOnDeath)
        {
            animController.enabled = false;
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
        else if (!isPlaying(animController, idleClip.name))
        {
            animController.Play(idleClip.name, 0, -1);

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
        GameObject _bullet = ObjectPool.GetObject(bulletPrefab, firePoint.position,firePoint.transform.rotation);
        _bullet.transform.GetChild(0).GetComponent<ProjectileBase>().InitializeStats(projectileDamage, projectileSpeed, projectileLifetime);

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