using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using NUnit.Framework;
using UnityEditor.Build.Player;

namespace PJR.Editor
{
    public static class EditorUtility
    {
        public static class Build
        {
            private const string TempDir = "Temp/PlayerScriptCompilationTests";

            public static void BuildTargetCompiles(BuildTarget buildTarget)
            {
                var settings = new ScriptCompilationSettings
                {
                    group = BuildPipeline.GetBuildTargetGroup(buildTarget),
                    target = buildTarget,
                    options = ScriptCompilationOptions.None
                };

                PlayerBuildInterface.CompilePlayerScripts(settings, TempDir + "_" + buildTarget);
            }
            public static void BuildCurrentTargetCompiles()
            {
                BuildTargetCompiles(EditorUserBuildSettings.activeBuildTarget);
            }
        }
        public static class Asset
        {
            public static bool OpenScriptOfType(System.Type type)
            {
                var mono = MonoScriptFromType(type);
                if (mono == null)
                    return false;

                AssetDatabase.OpenAsset(mono);
                return true;
            }

            public static MonoScript MonoScriptFromType(System.Type targetType)
            {
                if (targetType == null) return null;
                var typeName = targetType.Name;
                if (targetType.IsGenericType)
                {
                    targetType = targetType.GetGenericTypeDefinition();
                    typeName = typeName.Substring(0, typeName.IndexOf('`'));
                }

                return AssetDatabase.FindAssets(string.Format("{0} t:MonoScript", typeName))
                    .Select(AssetDatabase.GUIDToAssetPath)
                    .Select(AssetDatabase.LoadAssetAtPath<MonoScript>)
                    .FirstOrDefault(m => m != null && m.GetClass() == targetType);
            }
        }
    }
}