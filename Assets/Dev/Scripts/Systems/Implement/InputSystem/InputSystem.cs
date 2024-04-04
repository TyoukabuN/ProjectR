using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJR
{
    public partial class InputSystem : MonoSingletonSystem<InputSystem>
    {
        public static string assetPath = "Assets/Dev/Prefabs/SystemAssets/InputSystem/InputAction.inputactions";

        [SerializeField]
        private InputActionAsset inputActionAsset;
        public InputActionAsset InputActionAsset { get => inputActionAsset; }

        private Dictionary<string, InputActionMap> name2InputActionMap;
        
        public InputActionMap this[string key]
        {
            get {
                if (!name2InputActionMap.TryGetValue(key, out var inputActionMap))
                    return null;
                return inputActionMap;
            }
        }

        public override void Init()
        {
            base.Init();
            var loader = ResourceSystem.instance.LoadAsset(assetPath, typeof(InputActionAsset));
            loader.OnDone = OnLoadAssetDone;
        }
        void OnLoadAssetDone(ResourceLoader loader)
        {
            inputActionAsset = loader.GetAsset<InputActionAsset>();

            name2InputActionMap = new Dictionary<string, InputActionMap>();

            for(int i=0;i<name2InputHandle.Count;i++)
            {
                var pair = name2InputHandle.ElementAt(i);
                var handle = pair.Value;
                var name = pair.Value.name;
                //
                var actionMap = inputActionAsset.FindActionMap(name);
                if (actionMap != null)
                    handle.Init(actionMap);
                name2InputActionMap[name] = actionMap;
            }
        }
        private void Update()
        {
            for (int i = 0; i < name2InputHandle.Count; i++)
            {
                var pair = name2InputHandle.ElementAt(i);
                var handle = pair.Value;
                handle.OnUpdate();
            }
        }
    }
}
