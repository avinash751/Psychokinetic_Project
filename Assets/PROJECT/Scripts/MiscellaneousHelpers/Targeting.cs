using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Targeting : MonoBehaviour
{
    Targetable[] allTargetables;
    public float screenCenterThreshold = 0.25f;
    public static Targeting Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) 
        { 
            Instance = this;
            allTargetables = FindObjectsOfType<Targetable>();
        }
        else
        {
            Destroy(this);
        }
    }

    public Targetable GetClosestTarget(List<Targetable> listOfAllObjects, Vector3 referencePosition, float maxDistance)
    {
        Targetable nearestObject = default;
        float closestDistance = maxDistance;
        foreach (Targetable obj in listOfAllObjects)
        {
            if (!obj.gameObject.activeSelf) { continue; }
            float distance = Vector3.Distance(obj.transform.position, referencePosition);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestObject = obj;
            }
        }
        return nearestObject;
    }

    public List<Targetable> GetTargetablesInRadius(Vector3 referencePosition, float maxDistance, LayerMask layer)
    {
        List<Targetable> targetables = new();
        Collider[] colliders = Physics.OverlapSphere(referencePosition, maxDistance, layer);
        foreach (var hitCollider in colliders)
        {
            if (hitCollider.TryGetComponent(out Targetable target))
            {
                targetables.Add(target);
            }
        }
        return targetables;
    }

    public List<(Targetable target, Vector3 viewportPoint)> FindTargetsOnScreen(Camera camera, Vector3 position, float maxDistance)
    {
        List<(Targetable target, Vector3 viewportPoint)> targetsOnScreen = new();
        foreach (var target in allTargetables)
        {
            if (!target.gameObject.activeSelf || Vector3.SqrMagnitude(position - target.transform.position) > (maxDistance * maxDistance)) { continue; }
            Vector3 viewportPoint = camera.WorldToViewportPoint(target.transform.position);
            if (viewportPoint.z > 0 && viewportPoint.x > 0 && viewportPoint.x < 1 && viewportPoint.y > 0 && viewportPoint.y < 1)
            {
                targetsOnScreen.Add((target, viewportPoint));
            }
        }
        return targetsOnScreen;
    }


    // This is likely to end up being expensive to run every frame with large numbers of targets (100+)
    public (Targetable target, Vector3 viewportPoint) GetClosestTargetOnScreen(Camera camera, Vector3 position, float maxDistance, float screenCenterThreshold)
    {
        this.screenCenterThreshold = screenCenterThreshold;
        List<(Targetable target, Vector3 viewportPoint)> targetsOnScreen = FindTargetsOnScreen(camera, position, maxDistance);
        (Targetable target, Vector3 viewportPoint) closestTarget = (null, Vector3.zero);
        float closestScreenDistance = screenCenterThreshold;
        float aspectRatio = Screen.width / (float)Screen.height;
        foreach (var (target, viewportPoint) in targetsOnScreen)
        {
            float screenDistance = Vector2.Distance(new Vector2(viewportPoint.x * aspectRatio, viewportPoint.y), new Vector2(0.5f * aspectRatio, 0.5f));
            if (closestScreenDistance > screenDistance)
            {
                closestTarget = (target, viewportPoint);
                closestScreenDistance = screenDistance;
            }
        }
        return closestTarget;
    }

 /*   void OnDrawGizmos()
    {
        float adjustedScale = screenCenterThreshold / 0.5f;
        CanvasScaler canvas = GetComponentInParent<CanvasScaler>();
        float screenHeight = canvas.referenceResolution.y * canvas.scaleFactor;
        if (TryGetComponent<Image>(out var circularImage))
        {
            circularImage.rectTransform.sizeDelta = new Vector2(screenHeight * adjustedScale, screenHeight * adjustedScale);
        }
    }*/
}