using System;
using System.Reflection;
using Sirenix.OdinInspector;

namespace PJR.ClassExtension
{
    public static class SirenixExtension
    {
        public static T GetEnumAttribute<T>(this Enum e) where T : Attribute
        {
            Type t = e.GetType();
            Attribute a = t.GetField(Enum.GetName(t, e)).GetCustomAttribute(typeof(T), false);
            return a as T;
        }
        public static string GetEnumNiceName(this Enum e)
        {
            var attr = e.GetEnumAttribute<LabelTextAttribute>();
            if (attr != null)
            {
                return attr.Text;
            }
            return e.ToString();
        }
    }
}