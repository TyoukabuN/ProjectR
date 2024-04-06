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

        public static string Asset_Default_Player = "Assets/Art/Character/GlazyRunner/Prefabs/DefaultPlayer.prefab";

        /// <summary>
        /// 创建Player实体
        /// </summary>
        /// <param name="assetFullName"></param>
        /// <returns></returns>
        public static LogicEntity CreatePlayer(string assetFullName = "")
        {
            if(!Valid())
                return null;
            var context = new EntityContext();
            context.logicEntityID = GetGUID();
            context.assetFullName = !string.IsNullOrEmpty(assetFullName) ? assetFullName : Asset_Default_Player;
            //
            PlayerEntity player = new PlayerEntity();
            player.OnCreate(context);

            id2LogicEntity[context.logicEntityID] = player;
            return player;
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

        protected static int GetGUID()
        {
            return ++s_guid_logic;
        }
    }
}
