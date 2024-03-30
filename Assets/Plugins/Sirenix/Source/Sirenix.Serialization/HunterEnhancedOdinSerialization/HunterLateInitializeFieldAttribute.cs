using System;
using System.Diagnostics;

namespace Sirenix.Serialization
{
    /// <summary>
    /// 标记一个字段需要被额外初始复制，目的是解决odin序列化的constructor问题，
    /// 在赋值之前会额外创建一个持有该字段的实例，现在仅在NonMonoComponent中使用这个机制，
    /// 且尽在Editor下生效，只能用于非序列化字段，谨慎使用
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class HunterLateInitializeFieldAttribute :Attribute
    {
    }
}
