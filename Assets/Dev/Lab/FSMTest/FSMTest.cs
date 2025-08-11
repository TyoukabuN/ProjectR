using PJR;
using PJR.Core.Pooling;
using PJR.Core.StateMachine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;
using static FSMTest.States;
using static FSMTest.Transitions;

public class FSMTest : MonoBehaviour
{
    public Transform target;
    
    //private IFsm _fsm;
    private Fsm<FSMTestContext> _fsm;

    [Button]
    public void DoAutoMoveFsmTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        _fsm?.Release();
        _fsm = Fsm<FSMTestContext>.Get(new()
            {
                owner = this,
                target = target
            },
            Idle.Get(),
            Forward.Get(),
            Backward.Get(),
            Left.Get(),
            Right.Get()
        );
        _fsm.AddTransition<States.Idle,States.Forward>(OverOneSecond.Get());
        _fsm.AddTransition<States.Forward,States.Right>(OverOneSecond.Get());
        _fsm.AddTransition<States.Right,States.Backward>(OverOneSecond.Get());
        _fsm.AddTransition<States.Backward,States.Left>(OverOneSecond.Get());
        _fsm.AddTransition<States.Left,States.Forward>(OverOneSecond.Get());
        _fsm.ChangeState<Idle>();
    }
    
    [Button]
    public void DoOnFinishTest()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        _fsm?.Release();
        _fsm = Fsm<FSMTestContext>.Get(new()
            {
                owner = this,
                target = target
            }, 
            OnFinishTest.WaitForSecond<FSMTestContext>.Get(2),
            OnFinishTest.WaitForSecond<FSMTestContext>.Get(3)
        );
        _fsm.ConnectStatesWithOnFinish();
        _fsm.ChangeState(0);
    }

    void Update()
    {
        CommonUpdateContext context = new()
        {
            DeltaTime = Time.deltaTime,
            UnscaleDeltaTime = Time.unscaledDeltaTime
        };
        _fsm?.Update(context);
    }
    public class FSMTestContext
    {
        public FSMTest owner;
        public Transform target;
    }

    public static class States
    {
        public class Idle : SimpleTransitionFsmState<FSMTestContext>
        {
            public override void OnEnter()
                => Debug.Log($"Enter {GetType().Name}");

            public override void OnUpate(IUpdateContext context)
                => fsm.Context.target?.Translate(Vector3.forward * context.DeltaTime);

            public static Idle Get() => GenerialPool<Idle>.Get();
            public override void Release() => GenerialPool<Idle>.Release(this);
        }

        public class Forward : SimpleTransitionFsmState<FSMTestContext>
        {
            public override void OnEnter()
                => Debug.Log($"Enter {GetType().Name}");

            public override void OnUpate(IUpdateContext context)
                => fsm.Context.target?.Translate(Vector3.forward * context.DeltaTime);

            public static Forward Get() => GenerialPool<Forward>.Get();
            public override void Release() => GenerialPool<Forward>.Release(this);
        }

        public class Backward : SimpleTransitionFsmState<FSMTestContext>
        {
            public override void OnEnter()
                => Debug.Log($"Enter {GetType().Name}");

            public override void OnUpate(IUpdateContext context)
                => fsm.Context.target?.Translate(Vector3.back * context.DeltaTime);

            public static Backward Get() => GenerialPool<Backward>.Get();
            public override void Release() => GenerialPool<Backward>.Release(this);
        }

        public class Left : SimpleTransitionFsmState<FSMTestContext>
        {
            public override void OnEnter()
                => Debug.Log($"Enter {GetType().Name}");

            public override void OnUpate(IUpdateContext context)
                => fsm.Context.target?.Translate(Vector3.left * context.DeltaTime);

            public static Left Get() => GenerialPool<Left>.Get();
            public override void Release() => GenerialPool<Left>.Release(this);
        }

        public class Right : SimpleTransitionFsmState<FSMTestContext>
        {
            public override void OnEnter()
                => Debug.Log($"Enter {GetType().Name}");

            public override void OnUpate(IUpdateContext context)
                => fsm.Context.target?.Translate(Vector3.right * context.DeltaTime);

            public static Right Get() => GenerialPool<Right>.Get();
            public override void Release() => GenerialPool<Right>.Release(this);
        }
    }
    public static class Transitions
    {
        public class OverOneSecond : FsmTransition<FSMTestContext>
        {
            public override bool CanTransition()
            {
                return CurrentStateTime > 1;
            }
            public static OverOneSecond Get() => GenerialPool<OverOneSecond>.Get();
            public override void Release() => GenerialPool<OverOneSecond>.Release(this);
        }
    }

    public static class OnFinishTest
    {
        public class WaitForSecond<TContext> : SimpleTransitionFsmState<FSMTestContext>
        {
            private float _second = 1f;
            public override void OnUpate(IUpdateContext context)
            {
                Debug.Log($"({CurrentStateTime}/{_second})");
                if (CurrentStateTime >= _second)
                {
                    Status = EStatus.Finish;
                    Debug.Log("完成");
                }
            }
            public static WaitForSecond<TContext> Get(float second = 1f)
            {
                var temp = GenerialPool<WaitForSecond<TContext>>.Get();
                temp._second = second;
                return temp;
            }
            public override void Release() => GenerialPool<WaitForSecond<TContext>>.Release(this);
        }
    }
}
