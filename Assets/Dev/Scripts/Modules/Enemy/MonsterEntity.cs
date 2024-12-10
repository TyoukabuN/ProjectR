using PJR;
using PJR.ScriptStates.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MonsterEntity : StateMachineEntity
{
    public override string entityName => "MonsterEntity";
    public MonsterConfigAsset configAsset;
    protected override void Init_State()
    {
        //scriptStateMachine = new PlayerScriptStateMachine(this);
        //scriptStateMachine.Init();
        physEntity.onUpdateVelocity += OnUpdateVelocity;
        physEntity.onUpdateRotation += OnUpdateRotation;
    }
    protected override void Init_Input()
    {
        inputHandle = new PlayerInputHandle();
        //InputSystem.RegisterHandle(inputHandle);
        if (inputHandle == null)
        {
            LogSystem.LogError("[PlayerEntity.Init_Input]找不到对应的InputAssetMap");
            return;
        }
        Update_InputKCContent();
    }
    public override bool OnCreate(EntityContext context)
    {
        if (ResourceSystem.TryGetAsset("EntityPhysicsConfig.asset", out var loader))
            physicsConfig = loader.GetRawAsset<EntityPhysicsConfigAsset>();
        else
            LogSystem.LogError("PlayerEntity.OnCreate 加载 EntityPhysicsConfig 失败");

        physRequire = PhysEntityComponentRequire.All;
        return base.OnCreate(context);
    }
    protected override void OnAvatarLoadDone(PhysEntity physEntity)
    {
        if (physEntity != this.physEntity)
            return;

        Init_Input();
        Init_State();
    }
    public override void Update()
    {
        base.Update();

        Update_Input();
        Update_State();
    }
    protected override void OnUpdateRotation(KCContext context)
    {
        //if (scriptStateMachine == null)
        //    return;

        FillKCContext(context);

        //scriptStateMachine.CurrentState?.OnUpdateRotation(context);
        PlayerKCCFunc.CommonRotation(context);
    }
    protected override void OnUpdateVelocity(KCContext context)
    {
        //if (scriptStateMachine == null)
        //    return;

        FillKCContext(context);

        //更新额外值
        this.UpdateExtraValue(context.deltaTime);

        //状态的速度控制
        //scriptStateMachine.CurrentState?.OnUpdateVelocity(context);
        PlayerKCCFunc.CommonMove(context);

        //额外速度控制
        this.UpdateExtraVelocity(context.deltaTime, out var extraVec3);
        if (extraVec3.y > 0)
            context.motor.ForceUnground();
        context.currentVelocity += extraVec3;
    }
}
