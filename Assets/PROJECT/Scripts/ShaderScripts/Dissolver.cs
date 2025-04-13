using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.VFX;
using System.Linq;
using System.Collections.Generic;

public class Dissolver : MonoBehaviour, IResettable
{
    [SerializeField] float dissolveTime = 1f;
    [SerializeField] float disableDelay = 0.5f;
    [SerializeField] AnimationCurve curve;
    [SerializeField] VisualEffect vfx;
    [SerializeField] UnityEvent OnDissolve;
    [SerializeField] UnityEvent OnReset;
    [Resettable] GameObject _ => gameObject;
    List<Material> materials;

    private void Start()
    {
        materials = GetComponent<MeshRenderer>().materials.ToList();
        vfx.Stop();
        
        ReferenceManager.Instance.Resetter.OnReset += () => materials.ForEach(m => m.SetFloat("_Dissolve", 0));
        ReferenceManager.Instance.Resetter.OnReset += () => vfx.Stop();
        ReferenceManager.Instance.Resetter.OnReset += () => OnReset.Invoke();
    }

    IEnumerator DissolveObject()
    {
        float time = 0;
        gameObject.layer = 1;
        vfx.Play();
        OnDissolve.Invoke();
        while (time < dissolveTime)
        {
            materials.ForEach(m => m.SetFloat("_Dissolve", curve.Evaluate(time / dissolveTime)));
            time += Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(disableDelay);
        gameObject.SetActive(false);
    }

    public void Dissolve() => StartCoroutine(DissolveObject());
}