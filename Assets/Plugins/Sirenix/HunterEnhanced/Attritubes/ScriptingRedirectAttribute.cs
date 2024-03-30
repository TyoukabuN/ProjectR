using System;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum), System.Diagnostics.Conditional("UNITY_EDITOR")]
    public class ScriptingRedirectAttribute : Attribute
    {
        public readonly Type scriptType;

        public ScriptingRedirectAttribute(Type ScriptType)
        {
            scriptType = ScriptType;
        }
    }
}
