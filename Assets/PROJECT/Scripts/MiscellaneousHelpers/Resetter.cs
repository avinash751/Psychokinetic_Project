using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Collections;
using Dreamteck.Splines;

public class Resetter : MonoBehaviour
{
    [SerializeField] bool debugTime = true;

    Dictionary<Type, Func<ComponentState>> componentCorrelation = new()
    {
        { typeof(GameObject),() => new GameObjectState() },
        { typeof(Rigidbody), () => new RigidbodyState() },
        { typeof(Transform), () => new TransformState() },
        { typeof(SplineFollower), () => new SplineFollowerState() },
        { typeof(TimerUtilities), ()=> new ClassState<TimerUtilities>()},
        { typeof(MeshRenderer), () => new MeshRendererState() },
        //More component states to be added as needed
    };

    struct ObjectState
    {
        public Dictionary<MemberInfo, object> MemberStates;
        public List<ComponentState> ComponentStates;
        public bool Enabled;
    }

    Dictionary<IResettable, ObjectState> allInitialStates = new();
    Dictionary<IResettable, ObjectState> quickSave = new();
    public Action OnReset;

    void Start()
    {
        float startTime = Time.realtimeSinceStartup;
        var resettables = FindObjectsOfType<MonoBehaviour>().OfType<IResettable>();
        CaptureInitialStates(allInitialStates, resettables);
        float endTime = Time.realtimeSinceStartup;

        if (debugTime)
        {
            Debug.Log("Time to capture: " + ((endTime - startTime) * 1000).ToString("F2") + "ms.");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            quickSave.Clear();
            float startTime = Time.realtimeSinceStartup;
            CaptureInitialStates(quickSave, allInitialStates.Keys);
            float endTime = Time.realtimeSinceStartup;
            Debug.Log("Quicksaved!");

            if (debugTime)
            {
                Debug.Log("Time to capture: " + ((endTime - startTime) * 1000).ToString("F2") + "ms.");
            }
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            ResetAll(false);
        }
    }

    void CaptureInitialStates(Dictionary<IResettable, ObjectState> initial, IEnumerable<IResettable> resettables)
    {
        foreach (var resettable in resettables)
        {
            var initialState = new ObjectState
            {
                MemberStates = new(),
                Enabled = resettable.enabled,
                ComponentStates = new()
            };

            var type = resettable.GetType();
            var entries = GetAllEntries(type);

            foreach (var entry in entries)
            {
                object entryValue = entry.MemberType == MemberTypes.Field ? ((FieldInfo)entry).GetValue(resettable) : ((PropertyInfo)entry).GetValue(resettable);
                Type entryType = entry.MemberType == MemberTypes.Field ? ((FieldInfo)entry).FieldType : ((PropertyInfo)entry).PropertyType;

                if (entry.GetCustomAttribute<ResettableAttribute>() != null)
                {
                    if (componentCorrelation.TryGetValue(entryType, out Func<ComponentState> newComponent))
                    {
                        var componentState = newComponent();
                        componentState.CaptureState(entryValue);
                        initialState.ComponentStates.Add(componentState);
                    }
                    else
                    {
                        initialState.MemberStates[entry] = entryValue;
                    }
                }
            }

            initial[resettable] = initialState;
        }
    }

    public void ResetAll(bool resetToInitial = true)
    {
        Dictionary<IResettable, ObjectState> initial = resetToInitial ? allInitialStates : quickSave;

        float startTime = Time.realtimeSinceStartup;

        foreach (var resettable in initial.Keys)
        {
            var initialState = initial[resettable];

            foreach (var entry in initialState.MemberStates)
            {
                if (entry.Key.MemberType == MemberTypes.Field)
                {
                    (entry.Key as FieldInfo).SetValue(resettable, entry.Value);
                }
                else
                {
                    (entry.Key as PropertyInfo).SetValue(resettable, entry.Value);
                }
            }

            foreach (var componentState in initialState.ComponentStates)
            {
                componentState.ResetState();
            }

            if (resettable is MonoBehaviour monoBehaviour) { monoBehaviour.StopAllCoroutines(); }
            resettable.enabled = initialState.Enabled;
        }

        OnReset?.Invoke();

        float endTime = Time.realtimeSinceStartup;

        if (debugTime)
        {
            Debug.Log("Time to reset: " + ((endTime - startTime) * 1000).ToString("F2") + "ms.");
        }
    }

    // Recursive function that returns ALL fields & properties up to the base class, as long as it is an IResettable
    IEnumerable<MemberInfo> GetAllEntries(Type t)
    {
        if (t == null || !typeof(IResettable).IsAssignableFrom(t))
        {
            return Enumerable.Empty<MemberInfo>();
        }

        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var fields = t.GetFields(flags).Cast<MemberInfo>();
        var properties = t.GetProperties(flags).Cast<MemberInfo>();

        return fields.Concat(properties).Concat(GetAllEntries(t.BaseType));
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class ResettableAttribute : Attribute
{
}