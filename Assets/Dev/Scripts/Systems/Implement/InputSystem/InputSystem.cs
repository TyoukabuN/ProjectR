using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PJR.Systems.Input;
using Sirenix.OdinInspector;
using static UnityEngine.InputSystem.InputAction;
using System;
using System.Collections;
using static PJR.Systems.ResourceSystem;

namespace PJR.Systems
{
    public partial class InputSystem : MonoSingletonSystem<InputSystem>
    {
        public static string assetPath = "Assets/Dev/Prefabs/SystemAssets/InputSystem/InputAction.inputactions";

        [SerializeField]
        private InputActionAsset inputActionAsset;
        public InputActionAsset @InputActionAsset { get => inputActionAsset; }        
        [SerializeField]
        private PlayerInput playerInput;
        public PlayerInput @PlayerInput { get => playerInput; }

        public InputActionMap this[string name]
        {
            get {
                if (inputActionAsset == null)
                    return null;
                return inputActionAsset.FindActionMap(name);
            }
        }

        [ShowInInspector]
        private List<InputAction> actions;

        public override IEnumerator Initialize()
        {
            actions = new List<InputAction>();
            //
            RegisterKeys.Init();
            //
            //给测试的
            if (inputActionAsset == null)
            { 
                var loader = ResourceSystem.LoadAsset(assetPath, typeof(InputActionAsset));
                yield return loader;
                if (!string.IsNullOrEmpty(loader.error))
                    yield return null;
                OnLoadAssetDone(loader);
            }
            yield return null;
        }
        void OnLoadAssetDone(ResourceLoader loader)
        {
            inputActionAsset = loader.GetRawAsset<InputActionAsset>();

            playerInput = instance.gameObject.TryGetComponent<PlayerInput>();
            //TODO:后面需要分类型 因为有Gamepad和Keyboardmouse之分
            if (!inputActionAsset.enabled)
            {
                playerInput.actions = inputActionAsset;
                inputActionAsset.Enable();

                playerInput.notificationBehavior = PlayerNotifications.InvokeCSharpEvents;
                playerInput.defaultControlScheme = "KeyboardMouse";

                var gamepad = inputActionAsset.FindControlScheme("Gamepad");
                if (gamepad != null)
                {
                }
            }
        }

        [NonSerialized]
        protected InputHandle currentHandle = null;

        public static void RegisterHandle(InputHandle handle)
        {
            if (handle == null || handle == instance.currentHandle)
                return;
            if (instance.currentHandle != null)
                instance.currentHandle.OnUnRegister();
            instance.currentHandle = handle;

            var actionMap = instance.inputActionAsset.FindActionMap(handle.inputActionMapName);
            if (actionMap == null)
                return;

            handle.OnRegister(actionMap);

            foreach (var action in actionMap.actions)
            {
                if (action == null || instance.actions.Contains(action))
                    continue;
                instance.actions.Add(action);
                action.started += instance.OnActionStarted;
                action.performed += instance.OnActionPerformed;
                action.canceled += instance.OnActionCanceled;
            }
        }
        public void OnActionStarted(CallbackContext context)
        {
            //if (currentHandle != null)
            //    if(InputKey.Cache.TryGetKey(context.action.name, out var inputKeys))
            //        currentHandle.OnActionStarted(context, inputKeys);
        }
        public void OnActionPerformed(CallbackContext context)
        {
            if (currentHandle != null)
                if (InputKey.Cache.TryGetKey(context.action.name, out var inputKeys))
                    currentHandle.OnActionPerformed(context, inputKeys);
        }
        public void OnActionCanceled(CallbackContext context)
        {
            if (currentHandle != null)
                if (InputKey.Cache.TryGetKey(context.action.name, out var inputKeys))
                    currentHandle.OnActionPerformed(context, inputKeys);
        }


        public override void OnUpdate(float f)
        {
        }
    }
    public struct InputActionCallback
    {
        public Action<CallbackContext> started;
        public Action<CallbackContext> performed;
        public Action<CallbackContext> canceled;
        public void Register(InputAction action)
        {
            action.started += started;
            action.performed += performed;
            action.canceled += canceled;
        }
        public void UnRegister(InputAction action)
        {
            action.started -= started;
            action.performed -= performed;
            action.canceled -= canceled;
        }
    }
}
