using PJR.Input;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        protected void Init_Input()
        {
            inputHandle = InputSystem.GetInputHandle<PlayerInputHandle>();
            if (inputHandle == null)
            { 
                LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
                return;
            }
        }
        protected void Destroy_Input()
        { 
            inputHandle.Destroy();
        }
        protected void Update_Input()
        {
            inputHandle?.OnUpdate();
        }
        protected void LateUpdate_Input()
        {
            inputHandle?.OnLateUpdate();
        }
    }
}
