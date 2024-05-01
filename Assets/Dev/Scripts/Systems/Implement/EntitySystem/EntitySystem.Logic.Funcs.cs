using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class EntitySystem : MonoSingletonSystem<EntitySystem>
    {
        public PlayerEntity localPlayer = null;
        public static PlayerEntity LocalPlayer => inst.localPlayer;
        /// <summary>
        /// 创建Player实体
        /// </summary>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        public static LogicEntity CreatePlayer(EntityContext context = null)
        {
            if (!Valid())
                return null;
            var guid = GetGUID();
            context = new EntityContext();
            context.entityType = EntityDefine.LogicEntityType.Player;
            context.LogicEntityID = guid;

            PlayerEntity logicEntity = new PlayerEntity();
            logicEntity.entityContext = context;
            logicEntity.OnCreate(context);

            id2LogicEntity[context.LogicEntityID] = logicEntity;

            inst.localPlayer = logicEntity;
            return logicEntity;
        }

        /// <summary>
        /// 由Host创建
        /// </summary>
        /// <param name="host"></param>
        public static LogicEntity CreateTrapEntity(TrapConfigHost host)
        {
            if (!Valid())
                return null;
            var guid = GetGUID();
            EntityContext context = new EntityContext();
            context.entityType = EntityDefine.LogicEntityType.Trap;
            context.LogicEntityID = guid;
            context.originPosition = host.transform.position;
            context.originRotation = host.transform.eulerAngles;
            context.originScale = host.transform.lossyScale;
            context.avatarAssetNames = new AvatarAssetNames() {
                modelName = host.AssetName,
            };
            //
            TrapEntity logicEntity = new TrapEntity();
            logicEntity.entityContext = context;
            logicEntity.configAsset = host.configAsset;
            logicEntity.OnCreate(context);

            id2LogicEntity[context.LogicEntityID] = logicEntity;
            return logicEntity;
        }
        /// <summary>
        /// 由Host创建
        /// </summary>
        /// <param name="host"></param>
        public static LogicEntity CreateItemEntity(ItemConfigHost host)
        {
            if (!Valid())
                return null;
            var guid = GetGUID();
            EntityContext context = new EntityContext();
            context.entityType = EntityDefine.LogicEntityType.Item;
            context.LogicEntityID = guid;
            context.originPosition = host.transform.position;
            context.originRotation = host.transform.eulerAngles;
            context.originScale = host.transform.lossyScale;
            context.avatarAssetNames = new AvatarAssetNames()
            {
                modelName = host.AssetName,
            };
            //
            ItemEntity logicEntity = new ItemEntity();
            logicEntity.entityContext = context;
            logicEntity.config = host.config;
            logicEntity.OnCreate(context);
            //UnityEngine.Object component = host.config.script;
            if (host.config.itemType == 0)
            {
                logicEntity.itembase = logicEntity.gameObject.AddComponent<Item_Buff>() ;
            }
            logicEntity.itembase.CanRegenerateTimes = host.config.CanRegenerateTimes;
            logicEntity.itembase.interval = host.config.interval;
            logicEntity.itembase.itemconfig = host.config;
            id2LogicEntity[context.LogicEntityID] = logicEntity;
            return logicEntity;
        }
    }
}
