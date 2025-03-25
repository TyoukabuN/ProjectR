using UnityEngine.InputSystem;
using PJR.Systems.Input;

namespace PJR.Systems
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
