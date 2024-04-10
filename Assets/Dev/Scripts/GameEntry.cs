using PJR;
using PJR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    public static string[] PreloadAssets =
{
        "Assets/Art/Character/GlazyRunner/Prefabs/Avater_DefaultPlayer.prefab",
        "Assets/Art/Character/GlazyRunner/Animations/AnimatiomClipTransitionSet.asset",
        "Assets/Dev/Prefabs/ConfigAsset/EntityPhysicsConfig.asset",
    };
    void Start()
    {
        StartCoroutine(Preload());
    }

    void InitGame()
    {
        ResourceSystem.instance.Init();
        InputSystem.instance.Init();
    }

    IEnumerator Preload()
    {
        for (int i = 0; i < PreloadAssets.Length; i++)
        { 
            string assetFullName = PreloadAssets[i];
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


}
