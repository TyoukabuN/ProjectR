using System;
using System.Diagnostics;
using Sirenix.OdinInspector;

namespace Sirenix.Utilities
{
    [ShowInInspector][HideReferenceObjectPicker]
    public sealed class PromoteProperties
    {
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PromotePropertyDrawingSettingsAttribute : Attribute
    {
        /// <summary>
        /// 绘制折叠
        /// </summary>
        public bool FoldOut;
        /// <summary>
        /// 绘制为TabGroup
        /// </summary>
        public bool DrawAsTabGroup;
        /// <summary>
        /// 缓存路径至library 遍历property会比较卡, 真的卡了再实现
        /// </summary>
        public bool CachePropertyPaths;
     
    }

    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Field| AttributeTargets.Method|AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PromotePropertyAttribute :Attribute
    {
        public readonly string group;
        public readonly Type matchType;
        /// <summary>
        /// 将字段提前绘制
        /// </summary>
        /// <param name="groupName">组名</param>
        /// <param name="promoteToType">匹配类型serializeRoot类型</param>
        public PromotePropertyAttribute(string groupName = null, Type promoteToType = null)
        {
            this.@group = groupName;
            this.matchType = promoteToType;
        }
    }
}
