#if UNITY_EDITOR
using PJR.Systems;
using PJR.Systems.Log;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.LowLevel;
using Logger = PJR.Systems.Log.Logger;

[HelpURL("https://docs.unity3d.com/ScriptReference/LowLevel.PlayerLoopSystem.html")]
public class PlayerLoopTest : MonoBehaviour
{
    [Button()]
    public void LogCurrentPlayerLoopSystem()
    {
        string str = null;
        using (new ReportScope(out var report))
            LogPlayerLoopSystem2(PlayerLoop.GetCurrentPlayerLoop(),report);
        Debug.Log(str);
    }

    [Button]
    public void Test2()
    {
        var copyList = PlayerLoop.GetCurrentPlayerLoop().subSystemList.ToArray();
    }


    [Button]
    public void ReportTest()
    {
        var report = new Report();
        report.BeginLog();
        report.AppendLine("1");
        using (report.BeginGroupScope("组1"))
        {
            report.AppendLine("1");
            report.AppendLine("2");
            using (report.BeginGroupScope("组2"))
            {
                report.AppendLine("1");
                report.AppendLine("2");
                using (report.BeginGroupScope("组3"))
                {
                    report.AppendLine("1");
                    report.AppendLine("2");
                    report.AppendLine("3");
                }
                report.AppendLine("3");
            }
            report.AppendLine("3");
        }
        report.EndLog();
    }

    private string LogPlayerLoopSystem(PlayerLoopSystem loopSys, Report report,ref string str, int depth = 0)
    {
        string _str = "";
        if (loopSys.type != null)
        {
            if (loopSys.subSystemList == null)
                _str = $"   L {loopSys}";
            else
                _str = loopSys.ToString();
        }

        if (string.IsNullOrEmpty(str))
            str = _str;
        else
            str += ("\n" + _str);

        if (loopSys.subSystemList != null)
            foreach (var sub in loopSys.subSystemList)
            {
                LogPlayerLoopSystem(sub , report, ref str, ++depth);
            }

        return str;
    }
    private void LogPlayerLoopSystem2(PlayerLoopSystem loopSys, Report report)
    {
        string loopName = "Unnamed Group";
        if (loopSys.type != null)
            loopName = loopSys.type.ToString();

        if (loopSys.subSystemList != null)
        {
            using (report.BeginGroupScope(loopName))
            {
                foreach (var sub in loopSys.subSystemList)
                {
                    LogPlayerLoopSystem2(sub, report);
                }
            }
        }
        else
        {
            report.AppendLine(loopName);
        }
    }
}
#endif
