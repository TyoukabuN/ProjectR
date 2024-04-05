using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PJR
{
    public partial class EntitySystem : MonoSingletonSystem<EntitySystem>
    {
        private static int s_guid_phys = -1;
        private static Transform s_EntityRoot;

        /// <summary>
        /// 物理实体在根节点
        /// </summary>
        public static Transform EntityRoot
        {
            get
            {
                if (s_EntityRoot == null)
                {
                    var gobj = new GameObject("EntityRoot");
                    s_EntityRoot = gobj.transform;
                    s_EntityRoot.transform.SetParent(inst.transform, false);
                }
                return s_EntityRoot;
            }
        }
        private static Dictionary<int,PhysEntity> id2PhysEntity = new Dictionary<int,PhysEntity>();

        public override void Init()
        { 
            
        }

        public static PhysEntity CreatePhysEntity()
        {
            int guid = GetGUID();
            var gobj = new GameObject($"Entity_{guid}");
            var entity = gobj.AddComponent<PhysEntity>();

            gobj.transform.SetParent(EntityRoot, false);

            id2PhysEntity[guid] = entity;
            return entity;
        }

        public static PhysEntity GetPhysEntity(int id)
        {
            id2PhysEntity.TryGetValue(id, out var entity);
            return entity;
        }
        public static int GetGUID()
        {
            return ++s_guid_phys;
        }
    }
}
