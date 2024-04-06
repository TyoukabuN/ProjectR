using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;


public class Test : MonoBehaviour
{
    [Button("LogProjPaths")]
    public void LogProjPaths()
    {
        LogSystem.Log(Application.dataPath);
        LogSystem.Log(Application.persistentDataPath);
        LogSystem.Log(Application.consoleLogPath);
        LogSystem.Log(Application.streamingAssetsPath);
    }

    [Button("Test1")]
    public void Test1()
    {
        //Process.Start(Application.persistentDataPath);
        //Debug.Log(GUI.skin.FindStyle("ToolbarSeachTextField"));
        EntitySystem.CreatePlayer();
    }
}
