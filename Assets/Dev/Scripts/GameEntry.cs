using PJR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //for (int i = 0; i < ResourceDefine.Assets.Length; i++)
        //{ 
        //    string assetFullName = ResourceDefine.Assets[i];
        //    yield return ResourceSystem.LoadAsset<UnityEngine.Object>(assetFullName);
        //}
        yield return ResourceSystem.instance.Initialize();
        yield return InputSystem.instance.Initialize();
        yield return SceneSystem.instance.Initialize();
        yield return UISystem.instance.Initialize();
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
}
