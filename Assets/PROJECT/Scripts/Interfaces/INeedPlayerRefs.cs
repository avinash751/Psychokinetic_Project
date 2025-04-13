using Dreamteck.Splines;
using UnityEngine;

public interface INeedPlayerRefs
{
    virtual void FetchPlayerGrindControllerAndRb(Rigidbody rb, GrindController _gc = null) { }
    virtual void FetchPlayerMovement(PlayerMovement _playerMovement) { }
    virtual void FetchPlayerRigidbody(Rigidbody _rb) { }
    virtual void FetchPlayerGrindController(GrindController _gc) { }
    virtual void FetchPlayerSplineFollower(SplineFollower _splineFollower) { }
    virtual void FetchPlayerTransform(Transform _playerTransform) { }
}
