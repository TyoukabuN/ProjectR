using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using PJR.Input;
using Sirenix.OdinInspector;

namespace PJR
{
    public partial class InputSystem : MonoSingletonSystem<InputSystem>
    {
        public static string assetPath = "Assets/Dev/Prefabs/SystemAssets/InputSystem/InputAction.inputactions";

        [SerializeField]
        private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset { get => inputActionAsset; }

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

        public override void Init()
        {
            base.Init();
            actions = new List<InputAction>();
            //
            RegisterKeys.Init();
            //
            //给测试的
            if (inputActionAsset == null)
            { 
                var loader = ResourceSystem.LoadAsset(assetPath, typeof(InputActionAsset));
                loader.OnDone = OnLoadAssetDone;
            }
        }
        void OnLoadAssetDone(ResourceLoader loader)
        {
            inputActionAsset = loader.GetRawAsset<InputActionAsset>();

            //TODO:后面需要分类型 因为有Gamepad和Keyboardmouse之分
            if (!inputActionAsset.enabled)
                inputActionAsset.Enable();
        }

        public static InputHandle GetInputHandle<T>() where T : InputHandle,new()
        {
            if (instance.inputActionAsset == null)
                return null;
            var handle = new T();
            var actionMap = instance.inputActionAsset.FindActionMap(handle.inputActionMapName);
            if (actionMap == null)
            {
                handle = null;
                return null;
            }
            handle.Init(actionMap);

            foreach ( var action in handle.InputKey2Action) {
                if (action.Value == null || instance.actions.Contains(action.Value))
                    continue;
                instance.actions.Add(action.Value);
            }

            return handle;
        }
        public override void Update()
        {
            base.Update();
        }
    }
}
