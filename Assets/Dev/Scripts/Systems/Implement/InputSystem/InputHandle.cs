using PJR;
using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using PJR.Input;

public abstract class InputHandle
{
    public abstract KeyCategory keyCategory { get; }
    public abstract string inputActionMapName { get; }

    protected InputActionMap actionMap;
    public InputActionMap ActionMap => actionMap;

    protected Dictionary<string,InputAction> inputKey2Action = new Dictionary<string, InputAction>();
    public Dictionary<string, InputAction> InputKey2Action => inputKey2Action;

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
                var inputAction = actionMap.FindAction(inputkey.Value);
                if (inputAction == null)
                    continue;
                inputKey2Action[inputkey.Value.strValue] = inputAction;
            }
        }
    }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }

    //TODO:后面所有的callback都走这里，找个地方存起来
    public void OnActionStarted(CallbackContext context)
    { 
    }
    public void OnActionPerformed(CallbackContext context)
    { 
    }
    public void OnActionCanceled(CallbackContext context)
    { 
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

    public virtual InputAction FindAction(InputKey intputKey, bool throwIfNotFound = false) => FindAction(intputKey.strValue, throwIfNotFound);
    public virtual InputAction FindAction(string intputKey, bool throwIfNotFound = false)
    {
        return actionMap.FindAction(intputKey, throwIfNotFound);
    }
    public virtual bool GetKeyDown(string intputKey)
    {
        if (!inputKey2Action.TryGetValue(intputKey, out var inputAction))
            return false;
        return inputAction.WasPressedThisFrame();
    }
    public virtual bool GetKey(string intputKey)
    {
        if (!inputKey2Action.TryGetValue(intputKey, out var inputAction))
            return false;
        return inputAction.IsPressed();
    }
    public virtual TValue ReadValue<TValue>(string intputKey) where TValue : struct
    {
        if (!inputKey2Action.TryGetValue(intputKey, out var inputAction))
            return default(TValue);
        return inputAction.ReadValue<TValue>();
    }

}
