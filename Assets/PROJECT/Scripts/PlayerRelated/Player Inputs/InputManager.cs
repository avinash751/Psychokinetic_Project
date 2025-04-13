using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    private static InputActionAsset inputActionAsset;
    private static Dictionary<string, InputAction> keysActionMap;
    private static bool isCached = false;
    //All inputs
    private static InputAction horizontalAction;
    private static InputAction verticalAction;
    private static InputAction combatAction;
    private static InputAction jumpAction;
    private static InputAction leftRailSwitchAction;
    private static InputAction rightRailSwitchAction;
    private static InputAction railSprintAction;
    private static InputAction pauseAction;
    private static InputAction slowDownAction;
    private static InputAction stompAction;
    private static InputAction resetAction;
    private static InputAction railForwardsAction;
    private static InputAction railBackwardsAction;

    static InputManager()
    {
        LoadInputActions();
        Debug.LogWarning("input manager initalized");
    }

    private static void LoadInputActions()
    {
        inputActionAsset = Resources.Load<InputActionAsset>("PlayerInput");
        if (inputActionAsset == null)
        {
            Debug.LogError("InputActionAsset not found");
            return;
        }
        FindActionMaps();
    }

    private static void FindActionMaps()
    {
        horizontalAction = inputActionAsset.FindAction("Move Horizontal");
        verticalAction = inputActionAsset.FindAction("Move vertical");
        combatAction = inputActionAsset.FindAction("Telekinesis");
        jumpAction = inputActionAsset.FindAction("Jump");
        pauseAction = inputActionAsset.FindAction("Pause");
        slowDownAction = inputActionAsset.FindAction("Slowdown");
        stompAction = inputActionAsset.FindAction("Stomp");
        resetAction = inputActionAsset.FindAction("Reset");
        leftRailSwitchAction = inputActionAsset.FindAction("Left Rail Switch");
        rightRailSwitchAction = inputActionAsset.FindAction("Right Rail Switch");
        railSprintAction = inputActionAsset.FindAction("Rail Sprint");
        railForwardsAction = inputActionAsset.FindAction("Grind Up");
        railBackwardsAction = inputActionAsset.FindAction("Grind Down");

        keysActionMap = new Dictionary<string, InputAction>()
        {
            {"Move Horizontal", horizontalAction},
            {"Move vertical",verticalAction},
            {"Telekinesis", combatAction},
            {"Jump", jumpAction},
            {"Pause", pauseAction},
            {"Slow Down", slowDownAction},
            {"Stomp", stompAction},
            {"Reset", resetAction},
            {"Left Rail Switch", leftRailSwitchAction},
            {"Right Rail Switch", rightRailSwitchAction},
            {"Rail Sprint", railSprintAction},
            {"Grind Up", railForwardsAction},
            {"Grind Down", railBackwardsAction }
        };
        EnableActions();
    }

    private static void EnableActions()
    {
        if (!isCached)
        {
            foreach (var kvp in keysActionMap)
            {
                kvp.Value.Enable();
            }
        }
        isCached = true;
        
    }

    public static float GetHorizontal()
    {
        return horizontalAction.ReadValue<float>();
    }

    public static float GetVertical()
    {
        return verticalAction.ReadValue<float>();
    }

    public static bool GetKeyState(string actionName, bool isCountinousKeyPress = false)
    {
        if (keysActionMap.ContainsKey(actionName))
        {
            if (!isCountinousKeyPress)
            {
                return GetKeyState(keysActionMap[actionName]);
            }
            else
            {
                return GetContinuousKeyState(keysActionMap[actionName]);
            }
        }
        else
        {
            Debug.LogError(actionName + "InputAction not found.");
            return false;
        }
    }

    private static bool GetKeyState(InputAction action)
    {
        return action.triggered;
    }
    private static bool GetContinuousKeyState(InputAction action)
    {
        return !Mathf.Approximately(action.ReadValue<float>(), 0f);
    }
}