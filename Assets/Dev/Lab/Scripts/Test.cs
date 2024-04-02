using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;

public class Test : MonoBehaviour
{
    [ShowInInspector]
    public IActionInput actionInput;

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
        Debug.Log(GUI.skin.FindStyle("ToolbarSeachTextField"));
    }


    public interface IActionInput
    { 
    }

    public class CharacterActionInput:IActionInput { }
}
