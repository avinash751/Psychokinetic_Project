using UnityEngine;

public class MeshRendererState : ComponentState
{
    private MeshRenderer meshRenderer;
    private Material[] startMaterials;
    private Color _startColor;
    public override void CaptureState(object component)
    {
        meshRenderer = component as MeshRenderer;
        if (meshRenderer == null) { return; }
        startMaterials = meshRenderer.materials;
        if(meshRenderer.material.HasProperty("_BaseColor"))
        {
           _startColor = meshRenderer.material.GetColor("_BaseColor");
        }
        else if(meshRenderer.material.HasProperty("_Color"))
        {
            _startColor = meshRenderer.material.GetColor("_Color");
        }
    }

    public override void ResetState()
    {
        meshRenderer.materials = startMaterials;
        meshRenderer.material.color = _startColor;
    }
}
