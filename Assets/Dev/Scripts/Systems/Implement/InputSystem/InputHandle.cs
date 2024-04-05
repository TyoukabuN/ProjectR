using Microsoft.Win32;
using PJR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

public abstract class InputHandle
{
    public abstract InputKey.KeyCategory keyCategory { get; }
    public abstract string inputActionMapName { get; }

    protected InputActionMap actionMap;
    protected Dictionary<string,InputAction> inputKey2Action = new Dictionary<string, InputAction>();

    protected bool isInit = false;

    public InputHandle()
    { 
    }
    public virtual void Init(InputActionMap actionMap)
    {
        if (actionMap == null)
            return;
        isInit = true;
        this.actionMap = actionMap;
        inputKey2Action ??= new Dictionary<string, InputAction>();
        //
        if (InputKey.Cache.TryGetKeys((int)keyCategory, out var inputKeys))
        {
            foreach (var inputkey in inputKeys)
            {
                inputKey2Action[inputkey.Value.strValue] = actionMap.FindAction(inputkey.Value);
            }
        }
    }
    public virtual void RegisterCallback(InputKey inputKey, Action<CallbackContext> performed, Action<CallbackContext> canceled)=> RegisterCallback(inputKey.strValue, null, performed, canceled);
    public virtual void RegisterCallback(InputKey inputKey, Action<CallbackContext> started, Action<CallbackContext> performed, Action<CallbackContext> canceled) => RegisterCallback(inputKey.strValue, started, performed, canceled);
    public virtual void RegisterCallback(string inputKeyStr, Action<CallbackContext> started, Action<CallbackContext> performed, Action<CallbackContext> canceled)
    {
        if (!inputKey2Action.TryGetValue(inputKeyStr, out var inputAction))
            return;
        if (started != null) inputAction.started += started;
        if (performed != null) inputAction.performed += performed;
        if (canceled != null) inputAction.canceled += canceled;
    }

    public abstract void OnUpdate();
}
