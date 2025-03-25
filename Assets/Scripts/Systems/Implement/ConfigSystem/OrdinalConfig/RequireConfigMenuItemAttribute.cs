using System;
using System.Diagnostics;

namespace PJR.Config
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireConfigMenuItemAttribute : Attribute
    {
        public string MenuName { get; }

        public RequireConfigMenuItemAttribute(string menuName)
        {
            MenuName = menuName;
        }
    }
}
