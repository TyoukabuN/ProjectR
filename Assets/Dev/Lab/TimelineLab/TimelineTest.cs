using PJR.Timeline;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class TimelineTest : MonoBehaviour
{
    public SequenceDirector Director;

    [OnInspectorGUI]
    void OnInspectorGUI()
    {
        SirenixEditorGUI.BeginBox("Runner State");
        if (runner != null)
        {
            GUILayout.Label($"State: {runner.State}");
        }
        else
            GUILayout.Label("No Runner");
        SirenixEditorGUI.EndBox();
    }

    [Button, HorizontalGroup("OP")]
    void Clear()
    {
         runner?.Release();
         runner = null;
    }
    // [Button, HorizontalGroup("OP")]
    // void Pause()
    // { 
    // }
    //
    [Button, HorizontalGroup("OP")]
    void Play()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        Clear();
        runner = SequenceRunner.Get();
        runner.Reset(Director.gameObject, Director.Sequence);
    }

    SequenceRunner runner;
    private void Update()
    {
        if (runner != null)
        {
            if (runner.State == SequenceRunner.EState.None)
            {
                Profiler.BeginSample("SequenceStart");
                runner.OnStart();
                runner.OnUpdate(Time.unscaledDeltaTime);
                Profiler.EndSample();
            }
            else
            {
                runner.OnUpdate(Time.unscaledDeltaTime);
            }
        }
    }
}
