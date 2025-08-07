using PJR;
using PJR.Core.Pooling;
using PJR.Core.StateMachine;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class FSMTest : MonoBehaviour
{
    public Transform target;
    
    private FSM<FSMTestContext> _fsm;

    [Button]
    public void NewOne()
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            return;
        }
        _fsm?.Release();
        _fsm = FSM<FSMTestContext>.Get(new()
            {
                owner = this,
                target = target
            },
            Forward.Get(),
            Backward.Get(),
            Left.Get(),
            Right.Get()
        );
    }

    [Button("↑"),HideInEditorMode, HorizontalGroup("Vertical")]
    public void DOForward()
    {
        _fsm?.ChangeState<Forward>();
    }
    [Button("↓"),HideInEditorMode, HorizontalGroup("Vertical")]
    public void DOBackward()
    {
        _fsm?.ChangeState<Backward>();
    }
    [Button("←"),HideInEditorMode, HorizontalGroup("Horizontal")]
    public void DOLeft()
    {
        _fsm?.ChangeState<Left>();
    }
    [Button("→"),HideInEditorMode, HorizontalGroup("Horizontal")]
    public void DORight()
    {
        _fsm?.ChangeState<Right>();
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
    public class Forward : SimpleTransitionFSMState<FSMTestContext>
    {
        public override void OnEnter(FSM<FSMTestContext> fsm)
            => Debug.Log($"Enter {GetType().Name}");
        public override void OnUpate(IUpdateContext context, FSM<FSMTestContext> fsm)
            => fsm.Context.target?.Translate(Vector3.forward * context.DeltaTime);
        public static Forward Get() => GenerialPool<Forward>.Get();
        public override void Release() => GenerialPool<Forward>.Release(this);
    }
    public class Backward : SimpleTransitionFSMState<FSMTestContext>
    {
        public override void OnEnter(FSM<FSMTestContext> fsm)
            => Debug.Log($"Enter {GetType().Name}");
        public override void OnUpate(IUpdateContext context, FSM<FSMTestContext> fsm)
            => fsm.Context.target?.Translate(Vector3.back * context.DeltaTime);
        public static Backward Get() => GenerialPool<Backward>.Get();
        public override void Release() => GenerialPool<Backward>.Release(this);
    }
    public class Left : SimpleTransitionFSMState<FSMTestContext>
    {
        public override void OnEnter(FSM<FSMTestContext> fsm)
            => Debug.Log($"Enter {GetType().Name}");
        public override void OnUpate(IUpdateContext context, FSM<FSMTestContext> fsm)
            => fsm.Context.target?.Translate(Vector3.left * context.DeltaTime);
        public static Left Get() => GenerialPool<Left>.Get();
        public override void Release() => GenerialPool<Left>.Release(this);
    }
    public class Right : SimpleTransitionFSMState<FSMTestContext>
    {
        public override void OnEnter(FSM<FSMTestContext> fsm)
            => Debug.Log($"Enter {GetType().Name}");
        public override void OnUpate(IUpdateContext context, FSM<FSMTestContext> fsm)
            => fsm.Context.target?.Translate(Vector3.right * context.DeltaTime);
        public static Right Get() => GenerialPool<Right>.Get();
        public override void Release() => GenerialPool<Right>.Release(this);
    }
}
