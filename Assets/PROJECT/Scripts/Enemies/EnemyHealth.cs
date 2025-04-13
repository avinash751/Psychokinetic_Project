using UnityEngine;
using System;
using System.Collections.Generic;

public class EnemyHealth : ObjectHealth
{
    public static Action<GameObject> OnEnemyDeath;
    [SerializeField]float  deathStunDuration;
    bool allowStunOnDeath = false;
    EnemyBase enemyBase;
    Dissolver dissolver;
    public Rigidbody rb;

    string key = "EnemyDeath";



    protected override void OnEnable()
    {
        base.OnEnable();
        if(TryGetComponent(out enemyBase))
        {
            enemyBase.OnAllowEnemyStunning += EnemyStunMode;
        }      
    }

    protected  void OnDisable()
    {
        if (enemyBase == null) return;
        enemyBase.OnAllowEnemyStunning -= EnemyStunMode;
    }

    private void Start()
    {
        dissolver = GetComponent<Dissolver>();
        TryGetComponent(out rb);
        if(rb != null) rb.constraints = RigidbodyConstraints.FreezeAll;
    }


    protected override void ObjectDeath()
    {
        if (dissolver != null)
        {
            dissolver.Dissolve();
        }

        OnEnemyDeath?.Invoke(gameObject);
        AudioManager.Instance?.PlayAudio(key);

        if (allowStunOnDeath)
        {
            Invoke(nameof(StunnedDeath), deathStunDuration);
            return;
        }      
    }

    void StunnedDeath()
    {
        gameObject.SetActive(false);
    }

    void EnemyStunMode(bool allowStun)
    {
        allowStunOnDeath = allowStun;
    } 
}
