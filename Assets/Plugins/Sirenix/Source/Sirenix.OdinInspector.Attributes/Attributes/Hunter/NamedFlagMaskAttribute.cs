using System;
using UnityEngine;

namespace Sirenix.OdinInspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class NamedFlagMaskAttribute : Attribute
    {
        public string namesGetter;
        public NamedFlagMaskAttribute(string namesGetter)
        {
            this.namesGetter = namesGetter;
        }
    }
}