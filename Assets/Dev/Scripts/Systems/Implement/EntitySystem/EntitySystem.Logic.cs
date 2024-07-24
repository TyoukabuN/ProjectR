using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Animancer.Editor.SerializableEventSequenceDrawer;

namespace PJR
{
    public partial class EntitySystem : MonoSingletonSystem<EntitySystem>
    {
        private static int s_guid_logic = -1;
        private static Dictionary<int, LogicEntity> id2LogicEntity = new Dictionary<int, LogicEntity>();

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
            id2LogicEntity.Remove(logicEntity.entityContext.LogicEntityID);
            return true;
        }

        static bool IsLogicEntityValid(LogicEntity logicEntity)
        {
            if (logicEntity == null)
                return false;
            if (logicEntity.entityContext == null || logicEntity.entityContext.LogicEntityID < 0)
                return false;
            return true;
        }
        public static bool AddLogicEntity(int logicEntityID, LogicEntity logicEntity, bool CheckValid = true)
        {
            if(CheckValid && !IsLogicEntityValid(logicEntity))
                return false;
            if (id2LogicEntity.ContainsKey(logicEntityID))
                return false;
            id2LogicEntity[logicEntityID] = logicEntity;
            return true;
        }
        public static bool AddLogicEntity(LogicEntity logicEntity)
        {
            if (!IsLogicEntityValid(logicEntity))
                return false;
            return AddLogicEntity(logicEntity.entityContext.LogicEntityID, logicEntity, false);
        }

        protected static int GetGUID()
        {
            return ++s_guid_logic;
        }
    }
}
