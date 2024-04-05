using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PJR
{
    public class PlayerInputHandle : InputHandle
    {
        public override InputKey.KeyCategory keyCategory => InputKey.KeyCategory.PlayerInput;
        public PlayerInputHandle(string name):base(name) {  }
        public override void Init(InputActionMap actionMap)
        {
            base.Init(actionMap);
        }
        public override void OnUpdate()
        {

        }
    }
}
