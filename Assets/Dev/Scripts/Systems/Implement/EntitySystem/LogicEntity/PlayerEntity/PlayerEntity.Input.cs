using PJR.Input;
using UnityEngine;

namespace PJR
{
    public partial class PlayerEntity : LogicEntity
    {
        public KCContext _inputKCContent = null;
        public KCContext InputKCContent { 
            get {
                return _inputKCContent ??= new KCContext();
            }
        }
        protected void Init_Input()
        {
            inputHandle = InputSystem.GetInputHandle<PlayerInputHandle>();
            if (inputHandle == null)
            { 
                LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
                return;
            }
            Update_InputKCContent();
        }
        protected void Destroy_Input()
        { 
            inputHandle.Destroy();
        }

        protected void Update_InputKCContent()
        {
            if (inputHandle == null)
                return;

            var context = InputKCContent;
            var inputAxi = inputHandle.ReadValueVec2(RegisterKeys.Move);
            context.motor ??= this.physEntity.motor;
            context.inputAxi = inputAxi;
            context.rawMoveInputVector = Vector3.ClampMagnitude(new Vector3(inputAxi.x, 0f, inputAxi.y), 1f);

            context.moveInputVector = context.rawMoveInputVector;
            context.lookInputVector = context.rawMoveInputVector;
            if (CameraSystem.inst.mainCamera)
            {
                Quaternion CameraRotation = CameraSystem.inst.mainCamera.transform.rotation;
                Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.forward, context.motor.CharacterUp).normalized;
                if (cameraPlanarDirection.sqrMagnitude == 0f)
                {
                    cameraPlanarDirection = Vector3.ProjectOnPlane(CameraRotation * Vector3.up, context.motor.CharacterUp).normalized;
                }
                context.lookInputVector = cameraPlanarDirection;

                Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, context.motor.CharacterUp);

                context.moveInputVector = cameraPlanarRotation * context.rawMoveInputVector;
            }

            context.direction = context.moveInputVector;
            context.inputHandle = inputHandle;
        }

        public void CopyInputKCContent(KCContext context)
        {
            context.inputAxi = InputKCContent.inputAxi;
            context.rawMoveInputVector = InputKCContent.rawMoveInputVector;
            context.moveInputVector = InputKCContent.moveInputVector;
            context.lookInputVector = InputKCContent.lookInputVector;
            context.direction = InputKCContent.direction;
            context.inputHandle = InputKCContent.inputHandle;
        }

        protected void Update_Input()
        {
            inputHandle?.OnUpdate();

            Update_InputKCContent();
        }
        protected void LateUpdate_Input()
        {
            inputHandle?.OnLateUpdate();
        }
    }
}
