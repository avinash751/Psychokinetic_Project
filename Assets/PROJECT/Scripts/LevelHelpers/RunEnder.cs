using UnityEngine;

public class RunEnder : MonoBehaviour, IResettable
{
    [Resettable] bool open = true;

    private void Start()
    {
        ObjectiveManager objective = FindObjectOfType<ObjectiveManager>();
        if (objective != null && objective.objectiveType != ObjectiveManager.ObjectiveType.None)
        {
            objective.ObjectiveComplete += () => open = true;
            open = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (open && other.TryGetComponent(out GroundMovement player))
        {
            GameManager.Instance.Win();
        }
    }
}