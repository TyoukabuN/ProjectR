using System;
using System.Text;
using UnityEngine;

public static class CMDUtility
{
    public static bool RunBatchFile(string batFilePath)
    {
        if (string.IsNullOrEmpty(batFilePath))
            return false;
        // 创建一个新的进程启动信息对象
        var processStartInfo = new System.Diagnostics.ProcessStartInfo();
        processStartInfo.FileName = batFilePath;  // 设置要运行的文件
        processStartInfo.UseShellExecute = false; // 使用操作系统外壳程序执行进程
        processStartInfo.RedirectStandardOutput = true; // 重定向标准输出
        processStartInfo.RedirectStandardError = true; // 重定向错误输出
        processStartInfo.CreateNoWindow = true; // 不创建新窗口

        processStartInfo.StandardOutputEncoding = Encoding.GetEncoding("gb2312");
        processStartInfo.StandardErrorEncoding = Encoding.GetEncoding("gb2312");

        try
        {
            // 启动进程
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(processStartInfo))
            {
                // 读取输出和错误流
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // 等待进程退出
                process.WaitForExit();

                // 获取退出码
                int exitCode = process.ExitCode;

                // 打印输出和错误信息
                if (string.IsNullOrEmpty(error))
                    Debug.Log($"[Log]: {output}");
                else
                    Debug.LogError($"[Exit Code]:{exitCode}\n[Error]: {error}");

                return string.IsNullOrEmpty(error);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    public const string Default_fileName = "cmd.exe";
    public static bool RunCommonLine(string Arguments, string FileName = null)
    {
        if (string.IsNullOrEmpty(Arguments))
            return false;
        // 创建一个新的进程启动信息对象
        var processStartInfo = new System.Diagnostics.ProcessStartInfo();
        processStartInfo.FileName = string.IsNullOrEmpty(FileName) ? Default_fileName : FileName;  // 设置要运行的文件
        processStartInfo.Arguments = Arguments;  // 设置要运行的文件
        processStartInfo.UseShellExecute = false; // 使用操作系统外壳程序执行进程
        processStartInfo.RedirectStandardOutput = true; // 重定向标准输出
        processStartInfo.RedirectStandardError = true; // 重定向错误输出
        processStartInfo.CreateNoWindow = true; // 不创建新窗口

        processStartInfo.StandardOutputEncoding = Encoding.GetEncoding("gb2312");
        processStartInfo.StandardErrorEncoding = Encoding.GetEncoding("gb2312");

        try
        {
            // 启动进程
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(processStartInfo))
            {
                // 读取输出和错误流
                string output = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                // 等待进程退出
                process.WaitForExit();

                // 获取退出码
                int exitCode = process.ExitCode;

                // 打印输出和错误信息
                if (string.IsNullOrEmpty(error))
                    Debug.Log($"[Log]: {output}");
                else
                    Debug.LogError($"[Exit Code]:{exitCode}\n[Error]: {error}");

                return string.IsNullOrEmpty(error);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }

    public const string CMDKEY_BatchMode = "-batchmode";
    public const string CMDKEY_ExecuteMethod = "-executeMethod";

    /// <summary>
    /// 是不是通过Command Line运行的
    /// </summary>
    public static bool IsBatchMode
    {
        get
        {
            if (ExistArgumentKey(CMDKEY_BatchMode))
                return true;
            if (ExistArgumentKey(CMDKEY_ExecuteMethod))
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
        return !string.IsNullOrEmpty(GetArgumentValue(key, args));
    }
}
