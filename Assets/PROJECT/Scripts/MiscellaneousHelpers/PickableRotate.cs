using UnityEngine;

public class PickableRotate : MonoBehaviour, IResettable
{
    [SerializeField] float hoverHeight = 0.5f;
    [SerializeField] float hoverSpeed = 1f;
    [SerializeField] float rotationSpeed = 30f;
    [SerializeField] float axisRotationSpeed = 5f;
    private Vector3 initialPosition;
    private Vector3 rotationAxis;
    [Resettable] bool hover = true;
    [Resettable] Rigidbody rb;

    public void  Start()
    {
        initialPosition = transform.position;
        rotationAxis = Random.onUnitSphere;
        if (TryGetComponent(out Rigidbody rigidbody))
        {
            rb = rigidbody;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        EventManager.Subscribe<TelekineticObject>(EventType.Telekinesis, ToggleHover);
    }

    void Update()
    {   
        if (!hover) { return; }
        
        float hoverOffset = 1 + Mathf.Sin(Time.time * hoverSpeed);
        Vector3 targetPosition = initialPosition + Vector3.up * hoverHeight * hoverOffset;
        transform.position = targetPosition;
        
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime);

        rotationAxis = Quaternion.Euler(axisRotationSpeed * Time.deltaTime, axisRotationSpeed * Time.deltaTime, axisRotationSpeed * Time.deltaTime) * rotationAxis;
        rotationAxis.Normalize();
    }

    private void ToggleHover(TelekineticObject telekineticObject)
    {
        if (telekineticObject == null || telekineticObject.gameObject != gameObject) { return; }

        if (hover)
        {
            hover = false;
            rb.constraints = RigidbodyConstraints.None;
        }
        else if (!hover && !telekineticObject.Thrown)
        {
            hover = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }

    float Decay(float a, float b, float decay)
    {
        return b + (a - b) * Mathf.Exp(-decay * Time.deltaTime);
    }
}