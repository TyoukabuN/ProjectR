using PJR;
using System.Collections;
using UnityEngine;
using PJR.Systems;
using PJR.Systems.PlayerLoopUpdateAgent;
using Sirenix.OdinInspector;

public class GameEntry : MonoBehaviour
{
    void Awake()
    {
        StartCoroutine(InitAll());
    }

    IEnumerator InitAll()
    { 
        yield return InitSystems();
        yield return InitConfig();

        OnAfterInitAll();
        yield return null;
    }

    IEnumerator InitSystems()
    {
        yield return PlayerLoopUpdateAgentSystem.instance.Initialize();
        yield return ResourceSystem.instance.Initialize();
        yield return InputSystem.instance.Initialize();
        yield return SceneSystem.instance.Initialize();
    }
    IEnumerator InitConfig()
    {
        yield return ResourceSystem.LoadAsset<EntityPhysicsConfigAsset>("EntityPhysicsConfig.asset");
    }
    void OnAfterInitAll()
    {
        try
        {
            InitGame();
            OnAfterInitGame();
        }
        catch (System.Exception e)
        {
            LogSystem.LogError(e.ToString());
        }
    }
    void InitGame()
    {
    }

    void OnAfterInitGame()
    {
        if (SceneSystem.CheckReadyInValidGameScene()) { 
            SceneSystem.instance.OnEnterScene();
        }
    }

    [Button]
    void Test()
    {
        LogSystem.Log("Test");
    }
}
