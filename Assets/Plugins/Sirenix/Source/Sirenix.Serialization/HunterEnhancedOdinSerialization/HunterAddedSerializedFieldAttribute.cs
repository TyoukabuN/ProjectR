using System;
using System.Diagnostics;

namespace Sirenix.Serialization
{
    /// <summary>
    /// 标记一个新添加的字段, 尽在EntityComponent和MotionEvent中使用，仅在Editor下生效
    /// 目的是解决在已进行过序列化的类中添加新的字段后初值丢失的情况，标记并编译触发反序列化，完成初值的设置，之后需要把标记清掉
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    [Conditional("UNITY_EDITOR")]
    public class HunterAddedSerializedFieldAttribute :Attribute{}
    
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    [Conditional("UNITY_EDITOR")]
    public class HandleAddedFieldAttribute :Attribute{}
}
