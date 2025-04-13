using System.Collections;
using UnityEngine;

public static class PhysicsUtilities
{
    public static void KnockbackObjects(Transform origin, float radius, float knockbackStrength, float dragOffset, Collider[] objects)
    {
        foreach (Collider obj in objects)
        {
            if (obj.transform == origin) {  continue; }
            if (obj.TryGetComponent(out Rigidbody rb))
            {
                //ApplyForces(rb, origin.position, radius, knockbackStrength, iterations);
                float adjustedForce = knockbackStrength * Mathf.Max(rb.drag * dragOffset, 1);
                rb.AddExplosionForce(adjustedForce, origin.position, radius, 0, ForceMode.VelocityChange);
            }
        }
    }

    public static void KnockbackWithinRadius(Transform origin, float radius, float knockbackStrength, float dragOffset, LayerMask mask)
    {
        Collider[] objects = Physics.OverlapSphere(origin.position, radius, mask);
        KnockbackObjects(origin, radius, knockbackStrength, dragOffset, objects);
    }
}