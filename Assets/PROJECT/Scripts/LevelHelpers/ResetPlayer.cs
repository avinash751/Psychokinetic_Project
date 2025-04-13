using UnityEngine;

public class ResetPlayer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out PlayerMovement player))
        {
            ReferenceManager.Instance.GameManager.Restart();
        }
    }
}