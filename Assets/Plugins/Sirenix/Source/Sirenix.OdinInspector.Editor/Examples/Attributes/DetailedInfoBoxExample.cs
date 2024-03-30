//-----------------------------------------------------------------------
// <copyright file="DetailedInfoBoxExample.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Examples
{
#pragma warning disable

    [AttributeExample(typeof(DetailedInfoBoxAttribute))]
    internal class DetailedInfoBoxExample
    {
        [DetailedInfoBox("Click the DetailedInfoBox...",
            "... to reveal more information!\n" +
            "This allows you to reduce unnecessary clutter in your editors, and still have all the relavant information available when required.")]
        public int Field;
    }
}
#endif