using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetableGameObject : MonoBehaviour,IResettable
{
   [Resettable]GameObject go => gameObject;
    bool previousActiveState;

    private void OnEnable()
    {
        previousActiveState = !go.activeSelf;
        ReferenceManager.Instance.Resetter.OnReset += OnReset;

    }

    void OnReset()
    {
        go.SetActive(previousActiveState);
        ReferenceManager.Instance.Resetter.OnReset -= OnReset;
    }

}
