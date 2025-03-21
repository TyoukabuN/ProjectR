using System;
using Sirenix.OdinInspector;

namespace LS.Game
{
    public abstract class OrdinalConfigItemAsset<TItem> : OrdinalConfigItemAsset
        where TItem : OrdinalConfigItem
    {
        public TItem item;

        public override int ID { get => item?.ID ?? -1; set => item.ID = value; }
        public override string Name { get => item?.Name; set => item.Name = value; }
        public override bool Valid => item?.Valid ?? false;
#if UNITY_EDITOR
        public override string Editor_LabelName => item?.Name;
        public override bool Editable => item?.Editable ?? false;
#endif
    }

    public abstract class OrdinalConfigItemAsset : SerializedScriptableObject, IOrdinalConfigItem
    {
        public abstract int ID { get; set; }
        public abstract string Name { get; set; }
        public virtual bool Valid => ID > 0;
        public int CompareTo(IOrdinalConfigItem other)
        {
            return ID.CompareTo(other.ID);
        }
#if UNITY_EDITOR
        public virtual string Editor_LabelName => Name;
        public virtual bool Editable => false;
#endif

    }

    public interface IOrdinalConfigItem: IComparable<IOrdinalConfigItem>
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Valid { get; }

#if UNITY_EDITOR
        public string Editor_LabelName { get; }
        public bool Editable { get; }
#endif
    }
}
