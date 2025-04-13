using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXGraphController : MonoBehaviour
{
    [SerializeField] VisualEffect vfx;
    [SerializeField] List<string> vfxProperties = new List<string>();
    Dictionary<string, float> initialValues = new();

    private void Start()
    {
        EventManager.Subscribe<bool, GameObject>(EventType.VFX, StartVFX);
        vfx.Stop();
        foreach (var property in vfxProperties)
        {
            initialValues[property] = vfx.GetFloat(property);
        }
        ReferenceManager.Instance.Resetter.OnReset += () => vfx.Stop();    
    }

    private void OnDisable()
    {
        EventManager.Unsubscribe<bool, GameObject>(EventType.VFX, StartVFX);
    }

    public void StartVFX(bool enable, GameObject obj)
    {
        if (vfx != null && gameObject == obj)
        {
            if (enable)
            {
                vfx.Play();
                foreach (var property in initialValues)
                {
                    vfx.SetFloat(property.Key, property.Value);
                }
            }
            else
            {
                foreach (var property in initialValues)
                {
                    vfx.SetFloat(property.Key, 0);
                }
            }
        }
    }
}