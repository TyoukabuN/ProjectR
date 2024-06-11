using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;

public static class CMDUtility
{
    public const string CMDKEY_BatchMode = "-batchmode";
    public const string CMDKEY_ExecuteMethod = "-executeMethod";

    /// <summary>
    /// 是不是通过Command Line运行的
    /// </summary>
    public static bool IsBatchMode
    {
        get {
            if(ExistArgumentKey(CMDKEY_BatchMode))
                return true;
            if(ExistArgumentKey(CMDKEY_ExecuteMethod))
                return true;
            return false;
        }
    }
    /// <summary>
    /// 获取命令行中的某个参数的值
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static string GetArgumentValue(string key, string[] args = null)
    {
        args = args ?? Environment.GetCommandLineArgs();

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith(key))
            {
                return args[i].Split('=')[1];
            }
        }
        return null;
    }

    /// <summary>
    /// 命令行中有对应key
    /// </summary>
    /// <param name="key"></param>
    /// <param name="args"></param>
    /// <returns></returns>
    public static bool ExistArgumentKey(string key, string[] args = null)
    {
        return !string.IsNullOrEmpty(GetArgumentValue(key,args));
    }
}
