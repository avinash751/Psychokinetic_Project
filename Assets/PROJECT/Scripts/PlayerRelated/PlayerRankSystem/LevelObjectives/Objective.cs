using UnityEngine;

public abstract class Objective : MonoBehaviour,IResettable
{
    [Resettable] [field:SerializeField] public int Current { get; protected set; }
    public int Total { get; protected set; }
    public abstract bool CheckCompletion();
    public abstract void SetTotal(bool customTarget, int targetNumber);
}