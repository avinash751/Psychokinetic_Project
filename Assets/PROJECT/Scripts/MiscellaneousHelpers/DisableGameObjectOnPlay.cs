using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableGameObjectOnPlay : MonoBehaviour
{
    private void OnEnable()
    {
        gameObject.SetActive(false);
        Destroy(this);
    }
}
