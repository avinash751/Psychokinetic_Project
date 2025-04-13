using CustomInspector;
using System.Collections.Generic;
using UnityEngine;

public class InputTask : TutorialTask
{
    [SerializeField] KeyCode[] keysToPress;
    [SerializeField] bool requireAllKeys;
    [SerializeField][ShowIf(nameof(requireAllKeys))] bool simultaneousInput = false;
    [Resettable] bool cancelled = false;
    List<KeyCode> pressedKeys = new();

    protected override void Start()
    {
        base.Start();
        ReferenceManager.Instance.Resetter.OnReset += () => pressedKeys.Clear();
    }

    private void Update()
    {
        if (cancelled || !started) { return; }

        if (!simultaneousInput)
        {
            TrackKeyPresses();
        }

        if (CheckInputs() && !complete)
        {
            CompleteTask();
            cancelled = true;
        }
    }

    private void TrackKeyPresses()
    {
        foreach (KeyCode key in keysToPress)
        {
            if (Input.GetKey(key) && !pressedKeys.Contains(key))
            {
                pressedKeys.Add(key);
            }
        }
    }

    private bool CheckInputs()
    {
        if (requireAllKeys)
        {
            if (!simultaneousInput)
            {
                return pressedKeys.Count == keysToPress.Length;
            }
            else
            {
                foreach (KeyCode key in keysToPress)
                {
                    if (!Input.GetKey(key))
                    {
                        return false;
                    }
                }
                return true;
            }
        }
        else
        {
            foreach (KeyCode key in keysToPress)
            {
                if (Input.GetKey(key))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public override void CancelTask()
    {
        cancelled = true;
    }
}