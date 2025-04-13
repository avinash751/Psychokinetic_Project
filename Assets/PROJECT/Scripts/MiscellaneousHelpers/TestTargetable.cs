using UnityEngine;

public class TestTargetable : Targetable, IResettable
{
    [Resettable][SerializeField] private int exampleValue = 15;
    [Resettable][SerializeField] private Targetable exampleTarget = null;

    [Resettable] public Rigidbody rb => GetComponent<Rigidbody>();
    [Resettable] Transform t => transform;
    [Resettable] GameObject go => gameObject;

    [VInspector.Button]
    public void ResetObject()
    {
        ReferenceManager.Instance.Resetter.ResetAll();
    }

   [VInspector.Button]
   public void SetValues()
   {
        exampleTarget = FindObjectOfType<Targetable>();
        exampleValue = Random.Range(0, 100);
   }
}