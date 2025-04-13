using System;
using UnityEngine;

public interface IDamageable
{
    public GameObject gameObject {  get; }
    public Transform transform { get; }

    public static Action<int,GameObject> OnDamageTaken;

    public void TakeDamage(int damage);
}