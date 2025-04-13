using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityRandom = UnityEngine.Random;

public class CursorSettings : MonoBehaviour
{
    [SerializeField] CursorLockMode cursorLockMode = CursorLockMode.None;
    [SerializeField] bool cursorVisible = true;

    private void Start()
    {
        Cursor.lockState = cursorLockMode;
        Cursor.visible = cursorVisible;
    }

    public void ActivateMouseCursor(bool enabled)
    {
        /*if (!enabled && cursorVisible) 
        {
            Debug.LogWarning("Cursor cannot be set to be not  visible since its visibility in " + GetType().FullName +  " class is set to true");
            return;
        }*/
        Cursor.visible = enabled;
        Cursor.lockState = enabled ? CursorLockMode.Confined : CursorLockMode.Locked;
    }
}