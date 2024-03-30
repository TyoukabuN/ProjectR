using System;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using Debug = UnityEngine.Debug;

namespace Sirenix.Utilities.Editor
{
    public static class EditScriptUtitlity
    {
        public static bool OpenScriptOfType(System.Type type)
        {
            var mono = MonoScriptFromType(type);
            if (mono != null)
            {
                AssetDatabase.OpenAsset(mono);
                return true;
            }

            var attr = type.GetAttribute<ScriptingRedirectAttribute>();
            if (attr?.scriptType != null && MonoScriptFromType(attr.scriptType) is MonoScript s)
            {
                var lines = s.text.Split("\n");
                int foundLine = -1;
                var guessName = $"class {type.Name}"; 
                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].Contains(guessName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        foundLine = i;
                    }
                }

                if (foundLine >= 0)
                {
                    AssetDatabase.OpenAsset(s, foundLine);
                }
                else
                {
                    AssetDatabase.OpenAsset(s);
                }

                return true;
            }

            if (type.IsNested && type.DeclaringType != null && type.DeclaringType!= type)
            {
                return OpenScriptOfType(type.DeclaringType);
            }
            
            Debug.LogError($"Can't open script of type '{0}', because a script with the same name does not exist.{type.Name}");
            Debug.LogError("这个正常运作的前提条件是存在与类名相同的脚本, 毕竟本质逻辑是打开同名脚本");
            return false;
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
