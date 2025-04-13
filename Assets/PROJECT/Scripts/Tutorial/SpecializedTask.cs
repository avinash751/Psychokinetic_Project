using UnityEngine;

public class SpecializedTask : TutorialTask
{
    [SerializeField] EventType eventType;

    protected override void Start()
    {
        base.Start();
        ReferenceManager.Instance.Resetter.OnReset += () => EventManager.Unsubscribe(eventType, CompleteTask);
    }

    protected override void StartTask()
    {
        base.StartTask();
        EventManager.Subscribe(eventType, CompleteTask);
    }

    protected override void CompleteTask()
    {
        if (complete) { return; }
        EventManager.Unsubscribe(eventType, CompleteTask);
        base.CompleteTask();
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe(eventType, CompleteTask);
    }

    public override void CancelTask()
    {
        EventManager.Unsubscribe(eventType, CompleteTask);
    }
}