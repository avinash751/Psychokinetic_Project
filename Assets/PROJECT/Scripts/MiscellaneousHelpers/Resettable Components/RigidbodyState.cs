using UnityEngine;

public class RigidbodyState : ComponentState
{
    Rigidbody rb;
    Vector3 velocity;
    Vector3 angularVelocity;
    bool kinematic;
    bool gravity;
    RigidbodyConstraints rigidbodyConstraints;

    public override void CaptureState(object component)
    {
        rb = component as Rigidbody;
        if (rb == null) { return; }
        velocity = rb.velocity;
        angularVelocity = rb.angularVelocity;
        kinematic = rb.isKinematic;
        rigidbodyConstraints = rb.constraints;
        gravity = rb.useGravity;
    }

    public override void ResetState()
    {
        if (rb == null) { return; }
        rb.velocity = velocity;
        rb.angularVelocity = angularVelocity;
        rb.isKinematic = kinematic;
        rb.useGravity = gravity;
        rb.constraints = rigidbodyConstraints;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.ResetInertiaTensor();
        rb.ResetCenterOfMass();
    }
}