using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class TelekineticObject : MonoBehaviour, IResettable
{
    [SerializeField] protected int damage = 1;
    [SerializeField] GameObject vfx;
    [SerializeField] GameObject outline;
    [Resettable] bool applied = false;
    [Resettable] bool thrown = false;
    Transform target;
    [Resettable] public Rigidbody rb { get; private set; }
    [Resettable] Transform t => transform;
    [Resettable] GameObject go => gameObject;
    public bool Thrown => thrown;
    float throwForce;
    Coroutine throwRoutine;


    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody>();
        ObjectPool.InitializePool(vfx);
        thrown = false;
    }

    public void EnableEffect(Transform target = null, float throwForce = 0, Coroutine throwRoutine = null)
    {
        thrown = true;
        this.target = target;
        this.throwForce = throwForce;
        this.throwRoutine = throwRoutine;
    }

    protected virtual void Effect(Targetable targetable = null, float throwForce = 0)
    {
        if (targetable != null)
        {
            rb.AddForce((targetable.transform.position - transform.position).normalized * throwForce, ForceMode.VelocityChange);
        }
        if (gameObject.TryGetComponent(out Dissolver dissolver))
        {
            dissolver.Dissolve();
        }
    }

    public void ToggleOutline(bool value)
    {
        if (outline != null)
        {
            outline.SetActive(value);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (thrown && (target == null || target == collision.transform) && !applied)
        {
            if (collision.transform.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(damage);
                damageable.gameObject.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            }
            if (!applied)
            {
                if (throwRoutine != null) { ReferenceManager.Instance.telekinesis.StopCoroutine(throwRoutine); }
                Effect(collision.transform.GetComponent<Targetable>(), throwForce);
                if (vfx != null) { ObjectPool.GetObject(vfx, collision.GetContact(0).point, Quaternion.identity); }
                EventManager.Invoke(EventType.VFX, GetType(), false, gameObject);
                applied = true;
            }
        }
    }
}