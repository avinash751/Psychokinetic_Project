using UnityEditor;
using UnityEngine;
using VInspector;

public class KnockbackThrowable : TelekineticObject
{
    [SerializeField] float forceStrength = 5f;
    [SerializeField] float knockbackRadius = 10f;
    [SerializeField] float dragOffset = 0.5f;
    [SerializeField] LayerMask layerMask;
    
    [Foldout("Debug")]
    [SerializeField] bool showRadius = false;
    [EndFoldout]

    protected override void Effect(Targetable targetable = null, float throwForce = 0)
    {
        base.Effect(targetable, throwForce);
        Collider[] objects = Physics.OverlapSphere(transform.position, knockbackRadius, layerMask);
        PhysicsUtilities.KnockbackObjects(transform, knockbackRadius, forceStrength, dragOffset, objects);
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i].TryGetComponent(out IDamageable damageable)) { damageable.TakeDamage(damage); }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, knockbackRadius);
        }
    }
#endif
}