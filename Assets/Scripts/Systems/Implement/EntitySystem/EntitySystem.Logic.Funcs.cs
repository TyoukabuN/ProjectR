using UnityEngine;

namespace PJR.Systems
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
            context = context == null ? new EntityContext() : context;
            context.entityType = EntityDefine.LogicEntityType.Player;
            context.LogicEntityID = guid;

            PlayerEntity logicEntity = new PlayerEntity();
            logicEntity.entityContext = context;
            logicEntity.OnCreate(context);

            AddLogicEntity(logicEntity);

            inst.localPlayer = logicEntity;
            return logicEntity;
        }

        /// <summary>
        /// 由Host创建怪物实体
        /// </summary>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        public static LogicEntity CreateMonster(MonsterConfigHost host)
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
            context.avatarAssetNames = host.assetName;
            //
            MonsterEntity logicEntity = new MonsterEntity();
            logicEntity.entityContext = context;
            logicEntity.configAsset = host.configAsset;
            logicEntity.OnCreate(context);

            AddLogicEntity(logicEntity);
            return logicEntity;
        }

        /// <summary>
        /// 由Host创建陷阱实体
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
            context.avatarAssetNames = host.assetName;
            //
            TrapEntity logicEntity = new TrapEntity();
            logicEntity.entityContext = context;
            logicEntity.configAsset = host.configAsset;
            logicEntity.configHost = host;
            logicEntity.OnCreate(context);
            if (host.configAsset.isPhysics)
            {
                Rigidbody rb = logicEntity.gameObject.AddComponent<Rigidbody>();
            }
            
            AddLogicEntity(logicEntity);
            return logicEntity;
        }
        /// <summary>
        /// 创建物品实体
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
            context.avatarAssetNames = host.assetName;
            //
            ItemEntity logicEntity = new ItemEntity();
            logicEntity.entityContext = context;
            logicEntity.config = host.configAsset;
            logicEntity.OnCreate(context);

            AddLogicEntity(logicEntity);
            return logicEntity;
        }
    }
}
