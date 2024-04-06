using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static PJR.InputKey;

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

        public override void Init()
        {
            base.Init();
            //
            RegisterKeys.Init();
            //
            var loader = ResourceSystem.LoadAsset(assetPath, typeof(InputActionAsset));
            loader.OnDone = OnLoadAssetDone;
        }
        void OnLoadAssetDone(ResourceLoader loader)
        {
            inputActionAsset = loader.GetAsset<InputActionAsset>();
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
            return handle;
        }
        private void Update()
        {

        }
    }
}
