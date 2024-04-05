using Microsoft.Win32;
using PJR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputHandle
{
    public string name = string.Empty;
    public abstract InputKey.KeyCategory keyCategory { get; }

    private InputActionMap actionMap;
    public List<InputActionWrap> wraps;

    private bool isInit = false;

    public InputHandle(string name)
    { 
        this.name = name;   
    }
    public virtual void Init(InputActionMap actionMap)
    {
        if (actionMap == null)
            return;
        isInit = true;
        this.actionMap = actionMap;
        //
        wraps = new List<InputActionWrap>();
        if (InputKey.Cache.TryGetKeys((int)keyCategory, out var inputKeys))
        {
            foreach (var inputkey in inputKeys)
            {
                wraps.Add(new InputActionWrap(inputkey.Value, actionMap));
            }
        }
    }
    public struct InputActionWrap
    {
        public InputKey inputKey;
        public InputAction inputAction;
        public InputActionWrap(InputKey inputKey, InputActionMap actionMap)
        {
            this.inputKey = inputKey;
            inputAction = actionMap.FindAction(inputKey);
        }
    }

    public abstract void OnUpdate();
}
