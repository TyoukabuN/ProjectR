using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.Assertions;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        protected void Init_Input()
        {
            inputHandle = InputSystem.GetInputHandle<PlayerInputHandle>();
            if (inputHandle == null) LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
        }
        protected void Update_Input()
        {
            inputHandle?.OnUpdate();
        }
    }
}
