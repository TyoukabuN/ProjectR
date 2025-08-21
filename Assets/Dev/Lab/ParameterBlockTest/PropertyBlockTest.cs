using System;
using LS.Game.DataContext;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Profiling;

public partial class PropertyBlockTest : SerializedMonoBehaviour
{
    [NonSerialized, OdinSerialize] public TestProp.Persistent Block;

    public TestProp.Temp temp1;

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
            using (var temp = TestProp.Temp.Get())
            {
                temp.Attack = 100;
                temp.CD = 2;
                temp1?.Release();

                temp1 = TestProp.Temp.Get(temp.PropertyBlock);
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

        using var temp = TestProp.Temp.Get();
        temp.Attack = 200;
        temp.CD = 1;
        temp1?.Release();
        temp1 = TestProp.Temp.Get(temp.PropertyBlock);
    }
}

public abstract class TestProp : LogicProperty
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

    public class Persistent : TestProp
    {
        public override bool IsTemp => false;
        public override PropertyBlock PropertyBlock => _propertyBlock;

        [SerializeField, ShowIf("@IsPropertyBlockNull")]
        private PropertyBlock.Persistent _propertyBlock = new();
    }

    public class Temp : TestProp
    {
        public override bool IsTemp => true;
        public override PropertyBlock PropertyBlock => _propertyBlock;
        [ShowIf("@false")] private PropertyBlock.Temp _propertyBlock;

        public static Temp Get()
        {
            var temp = GenericPool<Temp>.Get();
            temp._propertyBlock = PropertyBlock.Temp.Get();
            return temp;
        }

        public static Temp Get(PropertyBlock propertyBlock)
        {
            var temp = GenericPool<Temp>.Get();
            temp._propertyBlock = PropertyBlock.Temp.Get(propertyBlock);
            return temp;
        }

        public override void Release()
        {
            //todo: release的方式，时机再想想
            // _propertyBlock?.Release();
            // _propertyBlock = null;
            GenericPool<Temp>.Release(this);
        }
    }
}