
using UnityEngine;
using CustomInspector;

public class ObjectHealth : MonoBehaviour, IDamageable,IResettable
{
    [SerializeField] protected  int MaxHealth;
    [Resettable][ReadOnly][SerializeField] protected int currentHealth;
    public bool IsInvincible;

    protected virtual void OnEnable()
    {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(int Amount)
    {
        if(IsInvincible) return;
        currentHealth -= Amount;
        OnTakingDamage();
        IDamageable.OnDamageTaken?.Invoke(Amount, gameObject);

        if (currentHealth <= 0)
        {
            ObjectDeath();
        }
    }

    protected virtual void OnTakingDamage()
    {

    }
    protected virtual void ObjectDeath() => Debug.Log("object is dead");
}
