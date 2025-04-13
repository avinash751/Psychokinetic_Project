using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : ProjectileBase
{
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void FixedUpdate()
    {
        rb.velocity = transform.parent.transform.forward * speed;
    }
}
