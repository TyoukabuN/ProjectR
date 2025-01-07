using PJR;
using PJR.Timeline;
using Sirenix.OdinInspector;
using UnityEngine;

public class TImelineTest : MonoBehaviour
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

    public Color color;

    [Button]
    void Test2()
    {
        Debug.Log(PJR.Timeline.Utility.GetGenericType(typeof(TestClipHandle), typeof(ClipHandle<>))?.Name ?? "Null");
    }


    [Button]
    void RunTestClip()
    {
        Sequence seq = new Sequence();
        seq.frameRateType = Define.EFrameRate.Game;
        
        var clips = new Clip[] {
            new TestClip() { 
                start = 0,
                end = 2,
                intValue = 3 
            }
        };

        seq.clips = clips;
        handle = new SequenceHandle(seq);
    }

    SequenceHandle handle;
    private void Update()
    {
        if (handle != null)
        { 
            handle.OnUpdate();
        }
    }
}
