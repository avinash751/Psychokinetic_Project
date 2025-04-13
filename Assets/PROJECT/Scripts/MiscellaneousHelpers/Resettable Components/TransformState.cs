using UnityEngine;

public class TransformState : ComponentState
{
    Transform transform;
    Vector3 position;
    Quaternion rotation;
    Vector3 scale;
    Transform parent;
    
    public override void CaptureState(object component)
    {
        transform = component as Transform;
        if (transform == null) { return; }
        parent = transform.parent;
        position = transform.localPosition;
        rotation = transform.localRotation;
        scale = transform.localScale;
    }

    public override void ResetState()
    {
        if (transform == null) { return; }
        transform.parent = parent;
        transform.localPosition = position;
        transform.localRotation = rotation;
        transform.localScale = scale;
    }
}