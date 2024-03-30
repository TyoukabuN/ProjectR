using System;
using Sirenix.Serialization.Utilities;
using UnityEditor;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    
    public class HunterSelectionPathAttribute :Attribute
    {
        public readonly string path;
        public readonly int priority;
        public HunterSelectionPathAttribute(string path)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.priority = 0;
        }

        public HunterSelectionPathAttribute(string path, int priority)
        {
            this.path = path ?? throw new ArgumentNullException(nameof(path));
            this.priority = priority;
        }
    }
}
