//-----------------------------------------------------------------------
// <copyright file="SceneExtensions.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using UnityEngine.SceneManagement;

    public static class SceneExtensions
    {
        public static SceneReference ToSceneReference(this Scene scene)
        {
            return new SceneReference(scene);
        }
    }
}
#endif