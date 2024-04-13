using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class EntitySystem : MonoSingletonSystem<EntitySystem>
    {
        private static int s_guid_logic = -1;
        private static Dictionary<int, LogicEntity> id2LogicEntity = new Dictionary<int, LogicEntity>();

        /// <summary>
        /// 创建Player实体
        /// </summary>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        public static LogicEntity CreatePlayer(EntityContext context = null)
        {
            if(!Valid())
                return null;
            context ??=new EntityContext();
            context.logicEntityID = GetGUID();
            //
            PlayerEntity player = new PlayerEntity();
            player.entityData = context; 
            player.OnCreate(context);

            id2LogicEntity[context.logicEntityID] = player;
            return player;
        }


        public static List<LogicEntity> GetEntitys()
        {
            return null;
        }

        /// <summary>
        /// 用id获取已生成的实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static LogicEntity GetEntity(int id)
        {
            if (!Valid())
                return null;
            id2LogicEntity.TryGetValue(id, out var entity);
            return entity;
        }

        public static bool DestroyEntity(int id)
        {
            return DestroyEntity(GetEntity(id));
        }
        public static bool DestroyEntity(LogicEntity logicEntity) 
        { 
            if(logicEntity == null)
                return false;
            logicEntity.Destroy();
            id2LogicEntity.Remove(logicEntity.entityData.logicEntityID);
            return true;
        }

        protected static int GetGUID()
        {
            return ++s_guid_logic;
        }
    }
}
