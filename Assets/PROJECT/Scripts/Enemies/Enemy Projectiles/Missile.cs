using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : ProjectileBase
{
    [SerializeField] float homingStrength;
    [SerializeField] float stopHomingRange = 5f; // The range at which homing stops
    [SerializeField] float straightSpeed = 20f; // Speed after homing stops
    bool isHoming = true;

    void Start()
    {
        rb.velocity = transform.forward * speed;
    }

    void FixedUpdate()
    {
        if (player == null)
            return; 

        Vector3 directionToTarget = (player.transform.position - transform.position).normalized;

        if (isHoming)
        {
            if (Vector3.Distance(transform.position, player.transform.position) <= stopHomingRange)
            {
                isHoming = false;
                rb.velocity = directionToTarget * straightSpeed;
                return;
            }
            Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, homingStrength * Time.fixedDeltaTime);
            Vector3 homingVelocity = directionToTarget * speed;
            rb.velocity = homingVelocity;
        }
    }
}
