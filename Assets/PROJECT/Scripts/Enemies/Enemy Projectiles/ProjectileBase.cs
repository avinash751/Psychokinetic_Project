using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    public EnemyTag player;
    public Rigidbody rb;
    public int speed = 5;
    public float lifetime = 5f;
    public int damage = 1;
    [SerializeField] ParticleSystem impactVfx;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        player = FindObjectOfType<EnemyTag>();
    }

    public void InitializeStats(int _damage, int _speed, float _lifeTime)
    {
        speed = _speed;
        lifetime = _lifeTime;
        damage = _damage;
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            return;
        }

        if (collision.transform.TryGetComponent(out IDamageable damageable) && collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            damageable.TakeDamage(damage);
            ObjectPool.ReturnObject(gameObject);

        }
        else 
        {
            ObjectPool.ReturnObject(gameObject);
        }
    }

}
