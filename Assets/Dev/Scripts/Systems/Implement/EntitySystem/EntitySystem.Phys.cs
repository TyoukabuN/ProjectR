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
        private static Dictionary<int,PhysEntity> instanceID2PhysEntity = new Dictionary<int,PhysEntity>();


        public static PhysEntity CreatePhysEntity()
        {
            int guid = GetGUID_Phys();
            var gobj = new GameObject($"Entity_{guid}");
            var entity = gobj.AddComponent<PhysEntity>();

            gobj.transform.SetParent(EntityRoot, false);

            entity.physEntityId = guid;
            //
            id2PhysEntity[guid] = entity;
            instanceID2PhysEntity[gobj.gameObject.GetInstanceID()] = entity;
            return entity;
        }

        public static PhysEntity GetPhysEntity(int id)
        {
            id2PhysEntity.TryGetValue(id, out var entity);
            return entity;
        }
        public static PhysEntity GetPhysEntityByGobjInstanceId(int instanceId)
        {
            instanceID2PhysEntity.TryGetValue(instanceId, out var entity);
            return entity;
        }
        public static bool DestroyPhysEntity(int id)
        {
            return DestroyPhysEntity(GetPhysEntity(id));
        }
        public static bool DestroyPhysEntityByInstanceId(int instanceId)
        {
            return DestroyPhysEntity(GetPhysEntity(instanceId));
        }
        public static bool DestroyPhysEntity(PhysEntity entity)
        {
            if (entity == null)
                return false;

            id2PhysEntity.Remove(entity.physEntityId);
            instanceID2PhysEntity.Remove(entity.gameObject.GetInstanceID());

            entity.Destroy();

            GameObject.DestroyImmediate(entity.gameObject);

            return true;
        }

        protected static int GetGUID_Phys()
        {
            return ++s_guid_phys;
        }
    }
}
