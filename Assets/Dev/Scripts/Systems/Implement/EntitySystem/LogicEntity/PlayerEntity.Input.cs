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
            if (inputHandle == null) LogSystem.LogError("[PlayerEntity.Init_Input]�Ҳ�����Ӧ��InputAssetMap");
        }
    }
}
