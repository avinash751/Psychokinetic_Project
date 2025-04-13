using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlopePhysics : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    private void Start()
    {
        rb = transform.parent.GetComponent<Rigidbody>();
    }

    private void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.5f))
        {
           transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        }
    }
}
