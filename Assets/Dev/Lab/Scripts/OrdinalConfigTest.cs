using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using PJR.Config;
using Sirenix.OdinInspector;
using UnityEditor;

public class OrdinalConfigTest : MonoBehaviour
{
    [AvatarConfigID]
    public int AvatarConfigId;
    [TestConfigAID]
    public int TestConfigA;

    public string ClassName;

    [Button]
    public void AnyClassName()
    {
        // Debug.Log(typeof(TestConfigA).FullName);
        // Debug.Log(typeof(TestConfigA).Assembly);
        // Debug.Log(Assembly.GetExecutingAssembly());
        // Debug.Log("+++++++++++++++++++++++++++++++++");

        var assembly1 = typeof(TestConfigA).Assembly;
        Debug.Log(assembly1.GetType(GetClassFullName(ClassName)) != null);
        
        Debug.Log(TryGetLoadedAssemblyByName(GetConfigAssemblyName(), out var assembly));
        Debug.Log(ExistClass(GetClassFullName(ClassName)));
    }

    public string GetClassFullName(string className) => $"{GetConfigAssemblyName()}.Config.{className}";
    public string GetConfigAssemblyName() => "PJR";
    public bool ExistClass(string className)
    {
        if (TryGetLoadedAssemblyByName(GetConfigAssemblyName(), out var assembly))
            return assembly.GetType(className) != null;

        var type = Type.GetType(className);
        return type != null;
    }
    bool TryGetLoadedAssemblyByName(string assemblyName,out Assembly res)
    {
        res = null;
        // 获取所有已加载的程序集
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
    
        // 查找匹配的程序集（不包含版本等信息的基本名称）
        foreach (Assembly assembly in assemblies)
        {
            string name = assembly.GetName().Name;
            if (name.Equals(assemblyName, StringComparison.OrdinalIgnoreCase))
            {
                res = assembly;
                return true;
            }
        }
    
        return false;
    }
}
