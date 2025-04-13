using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

public class ClassState<T> : ComponentState where T : class
{
    protected Dictionary<MemberInfo, object> initialFieldValues = new Dictionary<MemberInfo, object>();
    protected T capturedClass;
    public override void CaptureState(object component)
    {
        capturedClass = component as T;
        if (capturedClass == null) { return; }
        Type classType = typeof(T);
        BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        var fields = classType.GetFields(flags).Cast<MemberInfo>();
        var properties = classType.GetProperties(flags).Cast<MemberInfo>();

        foreach (var memberInfo in fields.Concat(properties))
        {
            if (memberInfo.GetCustomAttribute<ResettableAttribute>() != null)
            {
                if (memberInfo.MemberType == MemberTypes.Field)
                {
                    object initialValue = (memberInfo as FieldInfo).GetValue(capturedClass);
                    initialFieldValues[memberInfo] = initialValue;
                }
                else
                {
                    object initialValue = (memberInfo as PropertyInfo).GetValue(capturedClass);
                    initialFieldValues[memberInfo] = initialValue;
                }
            }
        }      
    }

    public override void ResetState()
    {
        foreach (var keyField in initialFieldValues)
        {
            if (keyField.Value != null)
            {
                if (keyField.Key.MemberType == MemberTypes.Field)
                {
                    (keyField.Key as FieldInfo).SetValue(capturedClass, keyField.Value);
                }
                else
                {
                    (keyField.Key as PropertyInfo).SetValue(capturedClass, keyField.Value);
                }
            }
        }   
    }
}
