using System;
using System.Collections.Generic;
using UnityEngine;


// More to add as necessary, this is here because the alternative is using strings which is disgusting
public enum EventType
{
    BunnyHop,
    PerfectStomp,
    Telekinesis,
    RailGrinding,
    GettingOnRails,
    RailSwitching,
    Grounded,
    VFX
}

public class EventManager : MonoBehaviour
{
    readonly Dictionary<Type, EventType> invokePermissions = new();

    readonly Dictionary<EventType, Delegate> eventDictionary = new();
    public static EventManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) { Instance = this; } else { Destroy(this); }
    }

    public static void Subscribe(EventType eventType, Action listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action == null)
            {
                Debug.LogError($"Action signature mismatch in Subscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Combine(Instance.eventDictionary[eventType], listener);
        }
        else
        {
            Instance.eventDictionary.Add(eventType, listener);
        }
    }

    public static void Subscribe<T1>(EventType eventType, Action<T1> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action<T1> == null)
            {
                Debug.LogError($"Action signature mismatch in Subscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Combine(Instance.eventDictionary[eventType], listener);
        }
        else
        {
            Instance.eventDictionary.Add(eventType, listener);
        }
    }

    public static void Subscribe<T1, T2>(EventType eventType, Action<T1,T2> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action<T1, T2> == null)
            {
                Debug.LogError($"Action signature mismatch in Subscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Combine(Instance.eventDictionary[eventType], listener);
        }
        else
        {
            Instance.eventDictionary.Add(eventType, listener);
        }
    }

    public static void Subscribe<T1, TResult>(EventType eventType, Func<T1, TResult> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            var currentDelegate = Instance.eventDictionary[eventType];
            if (currentDelegate != null && currentDelegate.GetType() != listener.GetType())
            {
                Debug.LogError($"Func signature mismatch in Subscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Combine(currentDelegate, listener);
        }
        else
        {
            Instance.eventDictionary.Add(eventType, listener);
        }
    }

    public static void Unsubscribe(EventType eventType, Action listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action == null)
            {
                Debug.LogError($"Action signature mismatch in Unsubscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Remove(Instance.eventDictionary[eventType], listener);
            if (Instance.eventDictionary[eventType] == null)
            {
                Instance.eventDictionary.Remove(eventType);
            }
        }
    }

    public static void Unsubscribe<T1>(EventType eventType, Action<T1> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action<T1> == null)
            {
                Debug.LogError($"Action signature mismatch in Unsubscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Remove(Instance.eventDictionary[eventType], listener);
            if (Instance.eventDictionary[eventType] == null)
            {
                Instance.eventDictionary.Remove(eventType);
            }
        }
    }

    public static void Unsubscribe<T1,T2>(EventType eventType, Action<T1,T2> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            if (Instance.eventDictionary[eventType] as Action<T1, T2> == null)
            {
                Debug.LogError($"Action signature mismatch in Unsubscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Remove(Instance.eventDictionary[eventType], listener);
            if (Instance.eventDictionary[eventType] == null)
            {
                Instance.eventDictionary.Remove(eventType);
            }
        }
    }

    public static void Unsubscribe<T1, TResult>(EventType eventType, Func<T1, TResult> listener)
    {
        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            var currentDelegate = Instance.eventDictionary[eventType];
            if (currentDelegate != null && currentDelegate.GetType() != listener.GetType())
            {
                Debug.LogError($"Func signature mismatch in Unsubscribe on the {eventType} event.");
                return;
            }
            Instance.eventDictionary[eventType] = Delegate.Remove(currentDelegate, listener);
            if (Instance.eventDictionary[eventType] == null)
            {
                Instance.eventDictionary.Remove(eventType);
            }
        }
    }

    public static void Invoke(EventType eventType, Type callerType)
    {
        if (Instance.invokePermissions.ContainsKey(callerType) && eventType != Instance.invokePermissions[callerType])
        {
            Debug.LogError($"Type mismatch. Why is the {callerType} class trying to invoke the {eventType} event?");
            return;
        }

        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            Action action = Instance.eventDictionary[eventType] as Action;
            if (action == null)
            {
                 Debug.LogError($"Action signature mismatch in Invoke on the {eventType} event.");
                return;
            }
            action?.Invoke();
        }
    }

    public static void Invoke<T1>(EventType eventType, Type callerType, T1 arg)
    {
        if (Instance.invokePermissions.ContainsKey(callerType) && eventType != Instance.invokePermissions[callerType])
        {
            Debug.LogError($"Type mismatch. Why is the {callerType} class trying to invoke the {eventType} event?");
            return;
        }

        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            Action<T1> action = Instance.eventDictionary[eventType] as Action<T1>;
            if (action == null)
            {
                Debug.LogError($"Action signature mismatch in Invoke on the {eventType} event.");
                return;
            }
            action?.Invoke(arg);
        }
    }

    public static void Invoke<T1, T2>(EventType eventType, Type callerType, T1 arg1, T2 arg2)
    {
        if (Instance.invokePermissions.ContainsKey(callerType) && eventType != Instance.invokePermissions[callerType])
        {
            Debug.LogError($"Type mismatch. Why is the {callerType} class trying to invoke the {eventType} event?");
            return;
        }

        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            Action<T1,T2> action = Instance.eventDictionary[eventType] as Action<T1, T2>;
            if (action == null)
            {
                Debug.LogError($"Action signature mismatch in Invoke on the {eventType} event.");
                return;
            }
            action?.Invoke(arg1, arg2);
        }
    }

    public static TResult Invoke<T1, TResult>(EventType eventType, Type callerType, T1 arg)
    {
        if (Instance.invokePermissions.ContainsKey(callerType) && eventType != Instance.invokePermissions[callerType])
        {
            Debug.LogError($"Type mismatch. Why is the {callerType} class trying to invoke the {eventType} event?");
            return default;
        }

        if (Instance.eventDictionary.ContainsKey(eventType))
        {
            var action = Instance.eventDictionary[eventType] as Func<T1, TResult>;
            if (action == null)
            {
                Debug.LogError($"Func signature mismatch in Invoke on the {eventType} event.");
                return default;
            }
            return action.Invoke(arg);
        }
        return default;
    }
}