using PJR;
using PJR;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    void Start()
    {
        ResourceSystem.instance.Init();
        StartCoroutine(PreloadGameResource());
    }
    IEnumerator PreloadGameResource()
    {
        for (int i = 0; i < ResourceDefine.Assets.Length; i++)
        { 
            string assetFullName = ResourceDefine.Assets[i];
            yield return ResourceSystem.LoadAsset<UnityEngine.Object>(assetFullName);
        }
        OnPreloadDone();

        yield return null;
    }
    void OnPreloadDone()
    {
        try
        {
            InitGame();
        }
        catch (System.Exception e)
        {
            LogSystem.LogError(e.ToString());
        }
    }
    void InitGame()
    {
        InputSystem.instance.Init();
        SceneSystem.instance.Init();

        OnInitGame();
    }

    void OnInitGame()
    {
        if (SceneSystem.CheckReadyInValidGameScene()) { 
            SceneSystem.instance.OnEnterScene();
        }
    }
}
