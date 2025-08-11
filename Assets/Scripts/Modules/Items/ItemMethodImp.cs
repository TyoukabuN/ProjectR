using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR
{
    public class ItemMethod_Star : ItemApproach
    {
        public override EActionType ActionType => EActionType.InvincibleStar;

        [LabelText("速度")] public float speed = 10f;
        [LabelText("持续时间")] public float duration = 3f;
        [LabelText("转向系数")] public float orientationSharpness = 5f;

        public override void ExecuteActionEvent(EActionEvent evt, LogicEntity entity, LogicEntity targetEntity)
        {
            if(!(entity is ItemEntity))
                return;
            if (!entity.physEntity.avatar.activeSelf)
                return;
            ExcuteEntrance((ItemEntity)entity, targetEntity);

            targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.Invincible, this, duration);
            targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.SpeedModify, new SpeedModifyParam(0, 0), duration);

            targetEntity.physEntity.Animancer_Play_Shot(EntityAnimationDefine.AnimationName.Invincible_Start, 0, () =>
            {
                targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.Invincible, this, duration);
                targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.SpeedModify, new SpeedModifyParam(speed, orientationSharpness), duration);
                targetEntity.AddExtraValue(EntityDefine.ExtraValueKey.LastNonZeroInput, targetEntity.physEntity.motor.CharacterForward, duration);
            });
        }

        public void ExcuteEntrance(ItemEntity itemEntity, LogicEntity targetEntity)
        {
            Debug.Log("无敌了");
            if (!itemEntity.itembase.isMugen)
            {
                if (itemEntity.config.CanRegenerateTimes - 1 <= 0)
                {
                    itemEntity.Destroy();
                }
                else
                {
                    CountDownToDo(itemEntity);
                }
            }
            else
            {
                CountDownToDo(itemEntity);
            }
        }
        private static void CountDownToDo(ItemEntity itemEntity)
        {
            itemEntity.itembase.CanRegenerateTimes--;
            itemEntity.physEntity.avatar.SetActive(false);
            itemEntity.itembase.StartCoroutine(itemEntity.itembase.CountDown(() => { itemEntity.physEntity.avatar.SetActive(true); }));
        }
    }
}