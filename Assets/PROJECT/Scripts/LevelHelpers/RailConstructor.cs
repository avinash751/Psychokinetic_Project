using Dreamteck.Splines;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailConstructor: MonoBehaviour
{
    [SerializeField] SplineComputer spline;

    private void OnValidate()
    {
        if (spline == null) { spline = gameObject.GetComponent<SplineComputer>(); }
        TryGetComponent(out MeshCollider splineCollider);
        if (splineCollider == null) { splineCollider = gameObject.AddComponent<MeshCollider>(); }
        splineCollider.sharedMesh = spline.GetComponent<MeshFilter>().sharedMesh;
    }

}
