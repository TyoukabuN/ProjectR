#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System.Reflection;

public static class FlagRuntimeEditorUtil
{
    private static Type GetFlagConfigType()
    {
        var flagMenuClasses = TypeCache.GetTypesWithAttribute(typeof(FlagConfigMarkAttribute)).ToList();
        if (flagMenuClasses == null || flagMenuClasses.Count <= 0)
        {
            Debug.Log("failure to found class");
            return null;
        }
        return flagMenuClasses.First();
    }
    public static void Editor_ShowFlagGenericMenu(Action<FlagDefine> callback = null, int category = -1)
    {
        var flagMenu = GetFlagConfigType();
        if (flagMenu == null)
            return;
        var Method_Editor_ShowFlagGenericMenu = flagMenu.GetMethod("Editor_ShowFlagGenericMenu", BindingFlags.Static | BindingFlags.Public);
        Method_Editor_ShowFlagGenericMenu.Invoke(null, new object[] { callback, category });
    }
    public static void Editor_ShowFilteredFlagGenericMenu(Action<FlagDefine> callback = null, bool categoryOnly = false, params int[] categorys)
    {
        var flagMenu = GetFlagConfigType();
        if (flagMenu == null)
            return;
        var Editor_ShowFilteredFlagGenericMenu = flagMenu.GetMethod("Editor_ShowFilteredFlagGenericMenu", BindingFlags.Static | BindingFlags.Public);
        Editor_ShowFilteredFlagGenericMenu.Invoke(null, new object[] { callback, categoryOnly, categorys });
    }
    public static FlagDefine Editor_GetFlagDefine(int id)
    {
        var flagMenu = GetFlagConfigType();
        if (flagMenu == null)
            return null;
        var GetFlagDefine = flagMenu.GetMethod("GetFlagDefine", BindingFlags.Static | BindingFlags.Public);
        return (FlagDefine)GetFlagDefine.Invoke(null, new object[] { id });
    }
    public static string Editor_GetMenuName(int id)
    {
        var flagMenu = GetFlagConfigType();
        if (flagMenu == null)
            return null;
        var Editor_GetMenuName = flagMenu.GetMethod("Editor_GetMenuName", BindingFlags.Static | BindingFlags.Public);
        return (string)Editor_GetMenuName.Invoke(null, new object[] { id });
    }
    public static string Editor_GetFlagDefineMenuName(FlagDefine flagDefine)
    {
        var flagMenu = GetFlagConfigType();
        if (flagMenu == null)
            return null;
        var Editor_GetFlagDefineMenuName = flagMenu.GetMethod("Editor_GetFlagDefineMenuName", BindingFlags.Static | BindingFlags.Public);
        return (string)Editor_GetFlagDefineMenuName.Invoke(null, new object[] { flagDefine });
    }
} 
#endif