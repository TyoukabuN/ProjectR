using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class InputActionWrap : MonoBehaviour, IInputActionCollection2
{
    public IEnumerable<InputBinding> bindings => asset?.bindings;

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public InputActionAsset asset = null;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public InputAction FindAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return asset.FindAction(actionNameOrId, throwIfNotFound);
    }

    public int FindBinding(InputBinding mask, out InputAction action)
    {
        return asset.FindBinding(mask, out action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Disable()
    {
        asset.Disable();
    }

    public void Enable()
    {
        asset.Enable();
    }

    void OnEnable()
    {
        Enable();
    }
    void OnDisable()
    {
        Disable();
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
