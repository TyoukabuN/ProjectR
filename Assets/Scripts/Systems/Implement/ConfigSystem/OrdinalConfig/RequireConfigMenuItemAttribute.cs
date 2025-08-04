using System;
using System.Diagnostics;

namespace PJR.Config
{
    [Conditional("UNITY_EDITOR")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class RequireConfigMenuItemAttribute : Attribute
    {
        public string MenuName { get; }
        public int Order { get; }

        public RequireConfigMenuItemAttribute(string menuName, int order = 0)
        {
            MenuName = menuName;
            Order = order;
        }
    }
}
