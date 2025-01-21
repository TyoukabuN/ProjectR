using PJR;
using PJR.ClassExtension;
using PJR.Timeline;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class TimelineTest : MonoBehaviour
{
    //[Button]
    //void Test()
    //{
    //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
    //    var derivedTypes = assemblies
    //    .SelectMany(assembly => assembly.GetTypes()) // 获取所有类型
    //    .Where(type => PJR.Timeline.Utility.InheritsFrom(type, typeof(ClipHandle<>)) && !type.IsAbstract) // 筛选继承类，排除抽象类
    //    .ToList();
    //    foreach (var _handleType in derivedTypes)
    //    {
    //        var clipType = PJR.Timeline.Utility.GetGenericType(_handleType, typeof(ClipHandle<>));
    //        Debug.Log($"{clipType.Name}  {_handleType.Name}");
    //    }
    //}
    public Transform trans;
    public float speed;
    public Color color;
    public AnimationClip animationClip;

    //[Button]
    //void Test2()
    //{
    //    Debug.Log(PJR.Timeline.Utility.GetGenericType(typeof(TestClipHandle), typeof(ClipRunner<>))?.Name ?? "Null");
    //}


    [Button]
    void RunTestClip()
    {
        if (!EditorApplication.isPlaying)
        { 
            EditorApplication.isPlaying = true;
            return;
        }
        Clear();

        Sequence seq = new Sequence();
        seq.FrameRateType = Define.EFrameRate.Game;

        seq.Tracks = new Track[] {
            new Track()
        };

        trans.ResetValue();
        seq.Tracks[0].clips = new Clip[] {
            new AnimancerClip() {
                start = 0,
                end = 2,
                animationClip = animationClip
            }
        };

        handle = SequenceRunner.Get();
        handle.Init(trans.gameObject, seq);
    }

    [Button, HorizontalGroup("OP")]
    void Clear()
    {
         handle?.Pool();
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
