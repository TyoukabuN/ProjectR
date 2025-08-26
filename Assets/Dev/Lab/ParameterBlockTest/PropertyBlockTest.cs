using System;
using PJR.Dev.Game.DataContext;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

public partial class PropertyBlockTest : SerializedMonoBehaviour
{
    [NonSerialized, OdinSerialize] public TestProp Block;

    public TestProp temp1;

    private bool _runtimeTest = false;

    [Button]
    public void RuntimeTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        _runtimeTest = !_runtimeTest;
    }

    void Update()
    {
        if (_runtimeTest)
        {
            Profiler.BeginSample("PropertyBlockTest");
            using (var temp = TestProp.GetTemp())
            {
                temp.Attack = 100;
                temp.CD = 2;
                temp1?.Release();

                temp1 = TestProp.GetTemp(temp.PropertyBlock);
            }

            Profiler.EndSample();
        }
    }

    [Button]
    public void NewTemp2()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }

        using var temp = TestProp.GetTemp();
        temp.Attack = 200;
        temp.CD = 1;
        temp1?.Release();
        temp1 = TestProp.GetTemp(temp.PropertyBlock);
    }
}

public class TestProp : EffectProperty<TestProp>
{
    [LabelText("攻击力"), ShowInInspector, DisableIf("@!AllowToModify")]
    public float Attack
    {
        get => PropertyBlock?.GetFloat(0) ?? 0f;
        set => Set(0, value);
    }

    [LabelText("冷却时间"), ShowInInspector, DisableIf("@!AllowToModify")]
    public float CD
    {
        get => PropertyBlock?.GetFloat(1) ?? 0f;
        set => Set(1, value);
    }
}