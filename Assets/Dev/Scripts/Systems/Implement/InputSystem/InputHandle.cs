using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;
using PJR.Input;
using UnityEngine;
using PJR;
using Unity.VisualScripting;

namespace PJR
{

    public abstract class InputHandle
    {
        public abstract KeyCategory keyCategory { get; }
        public abstract string inputActionMapName { get; }

        protected InputActionMap actionMap;
        public InputActionMap ActionMap => actionMap;

        protected Dictionary<string, InputAction> inputKey2Action = new Dictionary<string, InputAction>();
        protected Dictionary<string, InputActionCallback> inputKey2ActionCallback = new Dictionary<string, InputActionCallback>();
        public Dictionary<string, InputAction> InputKey2Action => inputKey2Action;

        private Flag256 _inputFlag;
        /// <summary>
        /// 记录输入的flag
        /// 用flag会快点,但是不直观,每10000次,比dic快4,5倍
        /// 但是两个都会记录
        /// 这里的记录相当于GetKey，但是GetKeyDown之类的只关注这一帧是否按下某个key的需求，还需要别的处理
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

        public virtual bool HasAnyFlag(InputKey inputKey)
        {
            return InputFlag.HasAny(inputKey);
        }
        public virtual bool HasAllFlag(InputKey inputKey)
        {
            return InputFlag.HasAll(inputKey);
        }
        public virtual bool HasKey(InputKey inputKey)
        {
            return _inputRecord.TryGetValue(inputKey, out var boolValue) && boolValue;
        }
        public virtual void OnRegister(InputActionMap actionMap)
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
                    inputKey2Action[inputkey.Value] = inputAction;
                    //
                    //var callback = new InputActionCallback
                    //{
                    //    started     = context => { OnActionStarted(context, inputkey.Value); },
                    //    performed   = context => { OnActionPerformed(context, inputkey.Value); },
                    //    canceled    = context => { OnActionCanceled(context, inputkey.Value); },
                    //};

                    //callback.Register(inputAction);
                    //inputKey2ActionCallback[inputkey.Value] = callback;
                }
            }
        }

        public virtual void OnUnRegister()
        {
            inputKey2Action.Clear();
            _inputFlag = Flag256.Empty;
        }

        public void Destroy()
        {
            foreach (var pair in inputKey2Action)
                inputKey2ActionCallback[pair.Key].UnRegister(pair.Value);
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
        public virtual void RegisterCallback(InputKey inputKey, Action<CallbackContext> performed, Action<CallbackContext> canceled) => RegisterCallback(inputKey.strValue, null, performed, canceled);
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
                value = Vector2.ClampMagnitude(value, 1);
            //value = value.normalized * Mathf.Min(value.magnitude, 1);
            return value;
        }
    }
}