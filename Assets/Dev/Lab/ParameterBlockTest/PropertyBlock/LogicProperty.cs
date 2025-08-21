using System;
using Sirenix.OdinInspector;

namespace LS.Game.DataContext
{
    public abstract class LogicProperty : ILogicProperty
    {
        public abstract bool IsTemp { get; }
        public abstract PropertyBlock PropertyBlock { get; }

        public bool IsPropertyBlockNull => PropertyBlock == null;

        protected bool AllowToModify => _allowToModify;
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

        void IDisposable.Dispose() => Release();

        public virtual void Release()
        {
        }

        public static implicit operator PropertyBlock(LogicProperty logicProperty)
            => logicProperty.PropertyBlock;
    }
}