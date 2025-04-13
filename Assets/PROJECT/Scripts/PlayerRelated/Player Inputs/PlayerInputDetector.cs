using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;


public class PlayerInputDetector : MonoBehaviour
{
    [SerializeField] private GameObject mouseCamera;
    [SerializeField] private GameObject controllerCamera;
    private CinemachineFreeLook mouseFreeLook;
    private CinemachineFreeLook controllerFreeLook;

    private void Start()
    {
        InputSystem.onActionChange += InputActionChangeCallback;
        mouseFreeLook = mouseCamera.GetComponent<CinemachineFreeLook>();
        controllerFreeLook = controllerCamera.GetComponent<CinemachineFreeLook>();
    }

    private void OnDestroy()
    {
        InputSystem.onActionChange -= InputActionChangeCallback;
    }

    private void Update()
    {
        if (mouseCamera.activeSelf)
        {
            controllerFreeLook.m_XAxis.Value = mouseFreeLook.m_XAxis.Value;
            controllerFreeLook.m_YAxis.Value = mouseFreeLook.m_YAxis.Value;
        }
        else if (controllerCamera.activeSelf)
        {
            mouseFreeLook.m_XAxis.Value = controllerFreeLook.m_XAxis.Value;
            mouseFreeLook.m_YAxis.Value = controllerFreeLook.m_YAxis.Value;
        }
    }

    private void InputActionChangeCallback(object obj, InputActionChange change)
    {
        if (!(obj is InputAction inputAction)) return;
        if (!inputAction.controls[0].IsPressed()) return;
        InputDevice device = inputAction.controls[0].device;
        if (device is Gamepad)
        {
            controllerCamera.SetActive(true);
            mouseCamera.SetActive(false);
        }
        else if (device is Keyboard)
        {
            controllerCamera.SetActive(false);
            mouseCamera.SetActive(true);
        }
    }
}