using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.InputSystem.InputAction;

namespace PJR
{
    public class PlayerInputHandle : InputHandle
    {
        public override InputKey.KeyCategory keyCategory => InputKey.KeyCategory.PlayerInput;
        public override string inputActionMapName => "Player";
        public override void Init(InputActionMap actionMap)
        {
            base.Init(actionMap);
        }
        public override void OnUpdate()
        {
        }
    }
}
