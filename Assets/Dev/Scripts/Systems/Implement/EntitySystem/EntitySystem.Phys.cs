using System;
using System.Collections.Generic;
using PJR.Core.Pooling;
using UnityEngine;

namespace PJR.Systems
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

        public static ObjectPool<PhysEntity> s_Pool;

        public EntitySystem()
        {
            s_Pool = new ObjectPool<PhysEntity>(
                createFunc: PoolActions.CreatePhysEntity,
                actionOnGet: PoolActions.OnGetPhysEntity,
                actionOnRelease: PoolActions.OnReleasePhysEntity,
                actionOnDestroy: PoolActions.OnDestroyPhysEntity
            );
        }

        static class PoolActions
        {
            public static PhysEntity CreatePhysEntity()
            {
                var gobj = new GameObject();
                var entity = gobj.AddComponent<PhysEntity>();
                gobj.transform.SetParent(EntityRoot, false);
                return entity;
            }
            public static void OnGetPhysEntity(PhysEntity entity)
            {
                int guid = GetGUID_Phys();
                entity.gameObject.name = $"Entity_{guid}";
                entity.physEntityId = guid;

                id2PhysEntity[guid] = entity;
                instanceID2PhysEntity[entity.gameObject.GetInstanceID()] = entity;
            }
            public static void OnReleasePhysEntity(PhysEntity entity)
            {
                id2PhysEntity.Remove(entity.physEntityId);
                instanceID2PhysEntity.Remove(entity.gameObject.GetInstanceID());

                entity.gameObject.SetActive(false);
            }
            public static void OnDestroyPhysEntity(PhysEntity entity)
            {
                if (entity == null)
                    return;

                id2PhysEntity.Remove(entity.physEntityId);
                instanceID2PhysEntity.Remove(entity.gameObject.GetInstanceID());

                entity.Destroy();

                DestroyImmediate(entity.gameObject);
            }
        }


        public static PhysEntity GetPhysEntityInstance()=> s_Pool.Get();
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



        public static bool ReleasePhysEntity(int id) => TryReleasePhysEntity(GetPhysEntity(id));
        public static bool ReleasePhysEntityByInstanceId(int instanceId)=> TryReleasePhysEntity(GetPhysEntity(instanceId));
        public static void ReleasePhysEntity(PhysEntity entity) => TryReleasePhysEntity(entity);
        public static bool TryReleasePhysEntity(PhysEntity entity)
        {
            if (entity == null)
                return false;

            s_Pool.Release(entity);
            return true;
        }

        protected static int GetGUID_Phys()
        {
            return ++s_guid_phys;
        }
    }
}
