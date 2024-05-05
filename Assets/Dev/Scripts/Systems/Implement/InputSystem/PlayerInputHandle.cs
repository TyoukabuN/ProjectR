using UnityEngine.InputSystem;
using PJR.Input;

namespace PJR
{
    public class PlayerInputHandle : InputHandle
    {
        public override KeyCategory keyCategory => KeyCategory.PlayerInput;
        public override string inputActionMapName => "Player";
        public override void OnRegister(InputActionMap actionMap)
        {
            base.OnRegister(actionMap);
        }
        public override void OnUpdate()
        {
            base.OnUpdate();
        }
    }
}
