using PJR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    void Start()
    {
        try {
            InitGame();
        }catch (System.Exception e)
        {
            LogSystem.LogError(e.ToString());
        }
    }

    void InitGame()
    {
        ResourceSystem.instance.Init();
        InputSystem.instance.Init();
    }
}
