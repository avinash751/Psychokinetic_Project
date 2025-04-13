using UnityEngine;

public class GameObjectState : ComponentState
{
    bool active;
    int layer;
    GameObject gameObject;

    public override void CaptureState(object component)
    {
        gameObject = component as GameObject;
        if (gameObject == null) { return; }
        layer = gameObject.layer;
        active = gameObject.activeSelf;
    }

    public override void ResetState()
    {
        if (gameObject == null) { return; }
        gameObject.layer = layer;
        gameObject.SetActive(active);
    }
}