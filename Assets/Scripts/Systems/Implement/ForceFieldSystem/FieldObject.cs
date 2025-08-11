using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations;
using static PJR.Systems.FieldSystem;

namespace PJR.Systems
{
    public abstract class FieldObject : MonoBehaviour, IDisposable
    {
        [LabelText("id"), DisableIf("@true")]
        public int id;
        [LabelText("场型状")]
        public FieldShape shape = FieldShape.Sphere;

        [LabelText("半径")]
        [OnValueChanged("OnRadiusChanged")]
        public float _radius = 1f;

        public float Radius {
            get => _radius;
            set {
                _radius = value;
                OnRadiusChanged();
            }
        }


        [LabelText("中心偏移")]
        public Vector3 translationOffset = Vector3.zero;

        [HideInInspector]
        /// <summary>
        /// 半径的平方,用Vec3.sqrMagnitude代替Vec3.Magnitude进行距离判断，因为Vec3.Magnitude有sqrt运算
        /// </summary>
        public float sqrRadius = 1f;

        /// <summary>
        /// 持续时间,-1 = 永续
        /// </summary>
        [LabelText("持续时间")]
        public float duration = -1;

        [HideInInspector]
        /// <summary>
        /// 已存在时间
        /// </summary>
        public float time = 0;
        /// <summary>
        /// 更新频率<para/>
        /// 每多少帧数更新一次
        /// </summary>
        [LabelText("更新频率")]
        public int updateFrameInterval = 2;

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        public void Dispose()
        {
#if FIELD_DEBUG
            Debug.LogError("[Field][Destroy] 场销毁");
#endif
        }

        public virtual void Tick(float deltaTime) {
            //TODO:[Field]加个更新频率判断

            if (!Update_Count(deltaTime))
                return;

            //TODO:[Field]先用Collider那一套
            //CheckIncludedEntity();

#if UNITY_EDITOR    
            Editor_OnDebug();
#endif
        }

        /// <summary>
        /// 更新计时器
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        public virtual bool Update_Count(float deltaTime)
        {
            if (!IsActive)
                return false;
            return true;
        }

        protected List<LogicEntity> entitys = new List<LogicEntity>();

        /// <summary>
        /// 检查范围内的实体
        /// </summary>
        public virtual void CheckIncludedEntity() {
            //TODO:[Field]要优化的地方
            //TODO：[Field]后面需要放到jobs里去
            entitys.Clear();
            foreach (var entity in EntitySystem.GetEntitys())
            {
                if (shape == FieldShape.Sphere)
                {
                    var dis = entity.transform.position - transform.position;
                    if (Mathf.Abs(dis.sqrMagnitude) <= sqrRadius)
                        entitys.Add(entity);
                }
            }
        }

        public const string Default_LargetLayerName = "Combat_hurtBox";
        public virtual void OnCreate()
        {
            if (shape == FieldShape.Sphere)
                AddSphereTrigger(Radius, translationOffset);
            else if (shape == FieldShape.Cube)
            { 
            }
            //contactBox.OnContactEnter += TriggerEnter;
            //contactBox.OnContactStay += TriggerStay;
            //contactBox.OnContactExit += TriggerExit;
            //
            //SetCollisionFlag(false, false, false, true);
            //gameObject.layer = 0;// LayerMask.NameToLayer("Default");
            //SetCollisionLayer(LayerMask.NameToLayer(Default_LargetLayerName));
            //
            ignoreEntities = new HashSet<LogicEntity>();
            detectedEntities = new HashSet<LogicEntity>();

            //KinematicSystem.onBeforeSimulate -= Tick;
            //KinematicSystem.onBeforeSimulate += Tick;
        }

        private ParentConstraint parentConstraint;
        public ParentConstraint ParentConstraint => parentConstraint;

        public virtual ParentConstraint SetConstraint(Transform parent) => SetConstraint(parent, Vector3.zero, Vector3.zero);
        public virtual ParentConstraint SetConstraint(Transform parent, Vector3 postionOffset) => SetConstraint(parent, postionOffset, Vector3.zero);
        /// <summary>
        /// 设置限制组件
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="postionOffset"></param>
        /// <param name="rotationOffset"></param>
        public virtual ParentConstraint SetConstraint(Transform parent, Vector3 postionOffset, Vector3 rotationOffset)
        {
            if (!gameObject.TryGetComponent<ParentConstraint>(out parentConstraint))
            {
                parentConstraint = gameObject.AddComponent<ParentConstraint>();
            }
            var source = new ConstraintSource() {
                sourceTransform = parent,
                weight = 1f,
            };
            if(parentConstraint.sourceCount <= 0)
                parentConstraint.AddSource(source);
            else
                parentConstraint.SetSource(0, source);
            parentConstraint.SetTranslationOffset(0, postionOffset);
            parentConstraint.SetRotationOffset(0, rotationOffset);

            parentConstraint.constraintActive = true;
            parentConstraint.weight = 1f;

            return parentConstraint;
        }

        #region Collision

        public LogicEntity OwnerEntity { get; set; }

        public HashSet<LogicEntity> ignoreEntities { get; protected set; } = new HashSet<LogicEntity>();

        public HashSet<LogicEntity> detectedEntities = new HashSet<LogicEntity>();

        //protected virtual void TriggerEnter(IContactBox contactBox, Collider other)
        //{
        //    var entity = other.GetComponent<LogicEntity>() ?? other.GetComponentInParent<LogicEntity>();
        //    if (ShouldDectect(entity))
        //    {
        //        AddDetectedEntity(entity);
        //    }
        //}

        //protected virtual void TriggerExit(IContactBox contactBox, Collider other)
        //{
        //    var entity = other.GetComponent<LogicEntity>() ?? other.GetComponentInParent<LogicEntity>();
        //    RemoveDetectedEntity(entity);
        //}

        //protected virtual void TriggerStay(IContactBox contactBox, Collider other)
        //{
        //    var entity = other.GetComponent<LogicEntity>() ?? other.GetComponentInParent<LogicEntity>();
        //    if (ShouldDectect(entity))
        //    {
        //        AddDetectedEntity(entity);
        //    }
        //    else
        //    {
        //        RemoveDetectedEntity(entity);
        //    }
        //}
        public bool ShouldDectect(LogicEntity entity)
        {
            if (entity == null) return false;
            if (ignoreEntities.Contains(entity)) return false;
            if (detectedEntities.Contains(entity)) return false;
            return true;
        }

        private void AddDetectedEntity(LogicEntity entity)
        {
            if (detectedEntities.Add(entity))
            {
#if FIELD_DEBUG
                Debug.LogError($"[Field][Destroy] 碰撞检测到目标: {entity.name}");
#endif
            }
        }

        private void RemoveDetectedEntity(LogicEntity entity)
        {
            if (detectedEntities.Remove(entity))
            {
#if FIELD_DEBUG
                Debug.LogError($"[Field][Destroy] 碰撞检测丢失目标: {entity.name}");
#endif
            }
        }

        public new virtual Collider collider { 
            get { 
                return shape == FieldShape.Sphere ? sphereCollider : boxCollider; 
            } 
        }
        public SphereCollider sphereCollider;
        public BoxCollider boxCollider;
        public virtual void ClearCollider()
        {
            if (sphereCollider != null) Destroy(sphereCollider);
            if (boxCollider != null) Destroy(boxCollider);
        }

        public virtual void AddSphereTrigger(float radius) => AddSphereTrigger(radius, Vector3.zero);
        public virtual void AddSphereTrigger(float radius,Vector3 translationOffset)
        {
            ClearCollider();
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.center = translationOffset;
            sphereCollider.radius = radius;
            sphereCollider.isTrigger = true;
        }
        public virtual void AddCubeTrigger(Vector3 size) => AddCubeTrigger(size, Vector3.zero);
        public virtual void AddCubeTrigger(Vector3 size, Vector3 translationOffset)
        {
            ClearCollider();
            boxCollider = gameObject.AddComponent<BoxCollider>();
            boxCollider.size = size;
            boxCollider.center = translationOffset;
            boxCollider.isTrigger = true;
        }

        #endregion


        #region IGameCollisionUnit Implement
        public string DisplayName => "FieldObject";

        public bool IsActive
        {
            get {
                if (duration < 0)
                    return true;
                return time < duration;
            }
        }
        #endregion

        public virtual void OnRadiusChanged()
        {
            sqrRadius = _radius * _radius;
            if (sphereCollider != null)
            {
                sphereCollider.radius = _radius;
            }
        }

        #region Editor
#if UNITY_EDITOR

        public virtual void Editor_OnDebug() {
            //Drawing.Draw.gameAndEditor.WireSphere(transform.position, _radius);
        }
#endif

        #endregion
    }
}


