using System;
using System.Diagnostics;

namespace PJR.BlackBoard.Inspector
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [Conditional("UNITY_EDITOR")]
    public class GenericTypeFilterAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the title for the dropdown. Null by default.
        /// </summary>
        public string DropdownTitle;

        /// <summary>
        /// If true, the value will be drawn normally after the type selector dropdown has been drawn. False by default.
        /// </summary>
        public bool DrawValueNormally;

        /// <summary>Creates a dropdown menu for a property.</summary>
        /// <param name="filterGetter">A resolved string that should evaluate to a value that is assignable to IList; e.g, arrays and lists are compatible.</param>
        public GenericTypeFilterAttribute()
        {
        }
    }

    public class GetValueApproachAttribute : Attribute
    {
    }
}