#if UNITY_EDITOR
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public static class RuntionEditorUtil
{
    private static MethodInfo loadIconMethodInfo;


    private static Dictionary<string, Texture2D> name2Tex;

    public static Texture2D LoadIcon(string name)
    {
        if (loadIconMethodInfo == null)
        {
            loadIconMethodInfo ??= typeof(EditorGUIUtility).GetMethod("LoadIcon", BindingFlags.Static | BindingFlags.NonPublic);
        }

        name2Tex ??= new Dictionary<string, Texture2D>();

        if (!name2Tex.TryGetValue(name, out var tex))
        {
            tex = loadIconMethodInfo.Invoke(null, new object[] { name }) as Texture2D;
            name2Tex[name] = tex;
        }
        return tex;
    }
}
#endif