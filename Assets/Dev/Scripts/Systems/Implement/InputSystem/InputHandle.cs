using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class InputHandle
{
    public string name = string.Empty;

    InputActionMap actionMap;
    bool isInit = false;
    public InputHandle(string name)
    { 
        this.name = name;   
    }
    public void Init(InputActionMap actionMap)
    {
        if (actionMap == null)
            return;
        isInit = true;
        //
        this.actionMap = actionMap;
    }
    public abstract void OnUpdate();
}
