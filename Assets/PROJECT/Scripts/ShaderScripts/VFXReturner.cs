using UnityEngine;

public class VFXReturner : MonoBehaviour, IResettable
{
    [SerializeField] float lifetime = 0.5f;
    [Resettable] GameObject _ => gameObject;
    [Resettable] float time = 0;

    private void Start()
    {
        ReferenceManager.Instance.Resetter.OnReset += Return;
    }

    void Update()
    {
        if (!gameObject.activeSelf) {  return; }

        time += Time.deltaTime;
        
        if (time > lifetime)
        {
            Return();
        }
    }

    private void Return()
    {
        time = 0;
        ObjectPool.ReturnObject(gameObject);
    }

    public void SetLifetime(float lifetime)
    {
        this.lifetime = lifetime;
        time = 0;
    }
}