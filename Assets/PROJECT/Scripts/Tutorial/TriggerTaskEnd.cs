using UnityEngine;

public class TriggerTaskEnd : MonoBehaviour
{
    [SerializeField] TriggerTask triggerTask;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == ReferenceManager.Instance.PlayerMovement.gameObject)
        {
            triggerTask.OnTriggerEnter?.Invoke();
        }
    }
}