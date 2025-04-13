using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using VInspector;
using System;
using Unity.VisualScripting;

public class ChainThrowable : TelekineticObject
{
    [SerializeField][Range(0.01f, 0.99f)] float forceDamping = 0.3f;
    [SerializeField] float chainRadius = 10f;
    [SerializeField] LayerMask targetLayer;
    [SerializeField][Range(0, 1f)] float timeBetweenChains = 0.5f;
    [SerializeField] float fadeDuration = 0.1f;
    [SerializeField] float shrinkTimePercentage = 0.3f;
    [SerializeField] GameObject linePrefab;
    [SerializeField] float lineWidth = 0.1f;

    [Foldout("Debug")]
    [SerializeField] bool showRadius = false;
    [EndFoldout]

    List<LineRenderer> spawnedLines = new();

    protected override void Start()
    {
        base.Start();
        ObjectPool.InitializePool(linePrefab);
        ReferenceManager.Instance.Resetter.OnReset += ClearLines;
    }

    protected override void Effect(Targetable targetable = null, float throwForce = 0)
    {
        base.Effect(targetable, throwForce);
        CoroutineWorkHorse.StartWork(ChainBranch(transform, new(), throwForce * forceDamping));
    }

    private IEnumerator ChainBranch(Transform currentOrigin, List<Targetable> chainedTargets, float throwForce)
    {
        var targetsToChain = Targeting.Instance.GetTargetablesInRadius(currentOrigin.position, chainRadius, targetLayer);
        var chainActions = new List<Action>();

        foreach (var target in targetsToChain)
        {
            if (!chainedTargets.Contains(target))
            {
                chainedTargets.Add(target);

                if (target.TryGetComponent(out IDamageable damageable))
                {
                    damageable.TakeDamage(damage);
                }

                if (target.TryGetComponent(out Rigidbody rb))
                {
                    rb.AddForce((target.transform.position - currentOrigin.position).normalized * throwForce, ForceMode.VelocityChange);
                }

                CreateLine(currentOrigin, target.transform);
                chainActions.Add(() => CoroutineWorkHorse.StartWork(ChainBranch(target.transform, chainedTargets, throwForce)));
            }
        }

        yield return new WaitForSeconds(timeBetweenChains);
        foreach (var chainAction in chainActions)
        {
            chainAction();
        }
    }

    private void CreateLine(Transform currentOrigin, Transform target)
    {
        LineRenderer chainLine = ObjectPool.GetObject(linePrefab, target.position, Quaternion.identity).GetComponent<LineRenderer>();
        SetLifetime(chainLine, fadeDuration);
        chainLine.startWidth = lineWidth;
        chainLine.endWidth = lineWidth;
        chainLine.positionCount = 2;
        chainLine.SetPosition(0, currentOrigin.position);
        chainLine.SetPosition(1, target.position);
        spawnedLines.Add(chainLine);
        CoroutineWorkHorse.StartWork(FadeLine(chainLine, currentOrigin, target));
    }

    private void SetLifetime(LineRenderer chainLine, float lifetime)
    {
        if (chainLine.TryGetComponent(out VFXReturner returner))
        {
            returner.SetLifetime(lifetime);
        }
        else
        {
            chainLine.AddComponent<VFXReturner>().SetLifetime(lifetime);
        }
    }

    void ClearLines()
    {
        foreach (var line in spawnedLines)
        {
            if (line != null) { ObjectPool.ReturnObject(line.gameObject); }
        }
        spawnedLines.Clear();
    }

    private IEnumerator FadeLine(LineRenderer line, Transform currentOrigin, Transform target)
    {
        float time = 0;
        float startWidth = line.startWidth;

        while (time < fadeDuration)
        {
            float shrinkStart = fadeDuration * (1 - shrinkTimePercentage);

            if (time >= shrinkStart)
            {
                float moveT = (time - shrinkStart) / (fadeDuration - shrinkStart);
                line.startWidth = Mathf.Lerp(startWidth, 0, moveT);
                line.endWidth = Mathf.Lerp(startWidth, 0, moveT);
            }

            line.SetPosition(0, currentOrigin.position);
            line.SetPosition(1, target.position);
            time += Time.deltaTime;
            yield return null;
        }

        spawnedLines.Remove(line);
        ObjectPool.ReturnObject(line.gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (showRadius)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, chainRadius);
        }
    }
#endif
}