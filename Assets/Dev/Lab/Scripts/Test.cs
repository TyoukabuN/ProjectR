using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
