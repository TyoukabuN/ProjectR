//-----------------------------------------------------------------------
// <copyright file="AnotherPropertysStateExample.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor.Examples.Internal;
    using System.Collections.Generic;

    [AttributeExample(typeof(OnStateUpdateAttribute), "The following example shows how OnStateUpdate can be used to control the state of another property.")]
	[ExampleAsComponentData(Namespaces = new string[] { "System.Collections.Generic" })]
	internal class AnotherPropertysStateExample
	{
		public List<string> list;
		
		[OnStateUpdate("@#(list).State.Expanded = $value")]
		public bool ExpandList;
	}
}
#endif