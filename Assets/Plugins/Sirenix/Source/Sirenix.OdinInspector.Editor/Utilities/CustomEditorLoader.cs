//-----------------------------------------------------------------------
// <copyright file="CustomEditorLoader.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using UnityEditor;

    [InitializeOnLoad]
    internal static class CustomEditorLoader
    {
        static CustomEditorLoader()
        {
            if (InspectorConfig.HasInstanceLoaded)
            {
                InspectorConfig.Instance.UpdateOdinEditors();
            }
            else
            {
                UnityEditorEventUtility.DelayAction(() => InspectorConfig.Instance.UpdateOdinEditors());
            }
        }
    }
}
#endif