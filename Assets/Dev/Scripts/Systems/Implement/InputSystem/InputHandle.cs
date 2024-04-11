using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using PJR.Input;
using UnityEngine;

public abstract class InputHandle
{
    public abstract KeyCategory keyCategory { get; }
    public abstract string inputActionMapName { get; }

    protected InputActionMap actionMap;
    public InputActionMap ActionMap => actionMap;

    protected Dictionary<string,InputAction> inputKey2Action = new Dictionary<string, InputAction>();
    public Dictionary<string, InputAction> InputKey2Action => inputKey2Action;

    private Flag256 _inputFlag;
    /// <summary>
    /// 记录输入的flag
    /// 用flag会快点,但是不直观,每10000次,比dic快4,5倍
    /// 但是两个都会记录
    /// </summary>
    public Flag256 InputFlag => _inputFlag;
    private Dictionary<InputKey, bool> _inputRecord = new Dictionary<InputKey, bool>();
    /// <summary>
    /// 记录输入的dic
    /// dic慢点但是最直观
    /// </summary>
    private Dictionary<InputKey, bool> InputRecord => _inputRecord;

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

                inputAction.started += context => {
                    OnActionStarted(context,inputkey.Value);
                };
                inputAction.performed += context => {
                    OnActionPerformed(context,inputkey.Value);
                };
                inputAction.canceled += context => {
                    OnActionCanceled(context,inputkey.Value);
                };
            }
        }
    }
    public virtual void OnUpdate() { }
    public virtual void OnLateUpdate() { }

    public void OnActionStarted(CallbackContext context, InputKey inputkey)
    {
    }
    public void OnActionPerformed(CallbackContext context, InputKey inputkey)
    {
        _inputFlag |= inputkey.flag;
        _inputRecord[inputkey] = true;
    }
    public void OnActionCanceled(CallbackContext context, InputKey inputkey)
    {
        _inputFlag &= ~inputkey.flag;
        _inputRecord[inputkey] = false;
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
    public virtual Vector2 ReadValueVec2(string intputKey, bool normalize = false)
    {
        if (!inputKey2Action.TryGetValue(intputKey, out var inputAction))
            return Vector2.zero;
        Vector2 value = inputAction.ReadValue<Vector2>();
        if (normalize)
            value = value.normalized * Mathf.Min(value.magnitude, 1);
        return value;
    }
}
