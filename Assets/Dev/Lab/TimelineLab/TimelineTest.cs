using System.Collections.Generic;
using PJR.ClassExtension;
using PJR.Timeline;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class TimelineTest : MonoBehaviour
{
    public Transform trans;
    public float speed;
    public Color color;
    public AnimationClip animationClip;

    [Button, HorizontalGroup("OP")]
    void Clear()
    {
         handle?.Release();
         handle = null;
    }
    [Button, HorizontalGroup("OP")]
    void Pause()
    { 
    }

    [Button]
    void LogClipType()
    {
        Global.GetAllClipType();
    }

    SequenceRunner handle;
    private void Update()
    {
        if (handle != null)
        {
            if (handle.state == SequenceRunner.EState.None)
            {
                Profiler.BeginSample("SequenceStart");
                handle.OnStart();
                handle.OnUpdate(Time.unscaledDeltaTime);
                Profiler.EndSample();
            }
            else
            {
                handle.OnUpdate(Time.unscaledDeltaTime);
            }
        }
    }
}
