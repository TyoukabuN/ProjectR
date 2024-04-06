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
        /// ����Playerʵ��
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
            player.entityContext = context; 
            player.OnCreate(context);

            id2LogicEntity[context.logicEntityID] = player;
            return player;
        }

        /// <summary>
        /// ��id��ȡ�����ɵ�ʵ��
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

        protected static int GetGUID()
        {
            return ++s_guid_logic;
        }
    }
}
