using System;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class TriggerTask : TutorialTask
{
    public Action OnTriggerEnter;

    protected override void StartTask()
    {
        base.StartTask();
        OnTriggerEnter += CompleteTask;
    }

    protected override void CompleteTask()
    {
        base.CompleteTask();
        OnTriggerEnter = null;
    }

    public override void CancelTask()
    {
        OnTriggerEnter = null;
    }
}