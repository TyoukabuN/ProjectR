using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PJR;
using static UnityEditor.Progress;
using UnityEditor;


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

    private LogicEntity player = null;
    [Button("GenPlayer")]
    public void Test1()
    {
        //Process.Start(Application.persistentDataPath);
        //Debug.Log(GUI.skin.FindStyle("ToolbarSeachTextField"));
        if (player != null)
        {
            EntitySystem.DestroyEntity(player);
            player = null;
        }
        player = EntitySystem.CreatePlayer();
    }

    public string key = string.Empty;
    public Vector3 Force = Vector3.zero;
    public float Duration = 0.333f;
    public float Damp = -1f;
    [ShowInInspector]
    public Easing.Ease easing = Easing.Ease.Linear;

    [Button("AddForce")]
    public void AddForce()
    {
        if (player != null)
        {
            var controller = player.AddExtraVelocity(key,Force, Duration, Damp);
            controller.easeType = easing;
        }
    }
}
