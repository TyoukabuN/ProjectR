using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace LS.Game.DataContext
{
    /// <summary>
    /// 实现了如果从<see cref="PropertyBlock"/>获取数据
    /// </summary>
    [Serializable]
    public abstract partial class EffectProperty : IEffectProperty
    {
        public virtual bool IsTemp => PropertyBlock?.IsTemp ?? true;
        public virtual PropertyBlock PropertyBlock
        {
            get => _propertyBlock;
            set => _propertyBlock = value;
        }

        public bool IsPropertyBlockNull => PropertyBlock == null;

        [SerializeField, ShowIf("@IsPropertyBlockNull")]
        protected PropertyBlock _propertyBlock;

        //因为不想Persistent的数据在runtime被修改,但是又想它能在Editor上被修改所以增加了一个手动修改的按钮
        protected bool AllowToModify => _allowToModify;
        [NonSerialized]
        protected bool _allowToModify = false;

#if UNITY_EDITOR
        public string Editor_AllowToModify_ButtonName
            => _allowToModify ? "<锁定属性>" : "<修改属性>";

        [Button("@Editor_AllowToModify_ButtonName")]
        public void Editor_AllowToModify()
            => _allowToModify = !_allowToModify;
#endif
        
        protected bool Set<T>(int index, T value, bool force = false)
        {
            force = force || _allowToModify;
            if (!force && !IsTemp)
                return false;
            return PropertyBlock?.Set(index, value, force) ?? false;
        }
        protected T Get<T>(int index)
        {
            if (PropertyBlock != null) 
                return PropertyBlock.Get<T>(index);
            return default;
        }
        public float GetFloat(int index) => PropertyBlock?.FloatBlock.Get(index) ?? 0f;
        public int GetInt(int index) => PropertyBlock?.IntBlock.Get(index) ?? 0;
        public bool GetBool(int index) => PropertyBlock?.BoolBlock.Get(index) ?? false;
        public string GetString(int index) => PropertyBlock?.StringBlock.Get(index) ?? string.Empty;
        
        void IDisposable.Dispose() => Release();

        public abstract void Release();

        public static implicit operator PropertyBlock(EffectProperty effectProperty)
            => effectProperty.PropertyBlock;
        
        /// <summary>
        /// 配置用
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetPersistent<T>() where T : EffectProperty, new()
        {
            var instance = new T();
            instance.PropertyBlock = new PropertyBlock.Persistent();
            return instance;
        }
        
        /// <summary>
        /// runtime用
        /// </summary>
        /// <param name="propertyBlock"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetTemp<T>(PropertyBlock propertyBlock = null) where T : EffectProperty, new()
        {
            var instance = new T();
            instance.PropertyBlock = PropertyBlock.Temp.Get(propertyBlock);
            return instance;
        }
    }

    public abstract class EffectProperty<T> : EffectProperty where T : EffectProperty, new()
    {
        public override void Release()
        {
            if (!IsTemp)
                return;
            if (_propertyBlock?.IsTemp ?? false)
            {
                _propertyBlock?.Release();
                _propertyBlock = null;
            }
            //Pool<T>.Release(this as T);
        }
        public static T GetPersistent() => EffectProperty.GetPersistent<T>();
        public static T GetTemp(PropertyBlock propertyBlock = null) => EffectProperty.GetTemp<T>(propertyBlock);
    }
}