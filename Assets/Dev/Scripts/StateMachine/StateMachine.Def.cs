using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

namespace PJR
{
    public static class EPlayerState
    {
        public static int None = 0;        
        public static int Stand = 1;
        public static int Walk = 2;
        public static int Running = 3;
        public static int Jump_Begin = 4;
        public static int Jump_Falling = 5;
        public static int Jump_Land = 6;
        public static int Dushing = 7;
        public static int End = 8;
    }

    public abstract class State
    {
        public enum Phase
        { 
            Running,
            End,
        }
        public Phase phase;

        public StateMachineEntity entity;

        public State() {  OnInit(); }
        public virtual void HandleInput(StateMachineEntity entity) { }
        public virtual void Update(StateContext stateContext) { }
        public virtual void FixedUpdate(StateContext stateContext) { }
        public virtual bool CanChange(int from) { return true; }
        public virtual float normalizeTime {  get { return 1f; } }

        #region Phase
        protected void ToPhaseEnd() { phase = Phase.End; }
        public virtual void OnInit() { }
        public virtual void OnEnter(StateMachineEntity entity) { phase = Phase.Running; this.entity = entity; }
        public virtual void OnUpdate() { }
        public virtual void OnChange(int from) { }
        public virtual void OnFinish() { }
        #endregion

        #region Input System Message
        public virtual void OnMove(InputValue value) { }
        public virtual void OnLook(InputValue value) { }
        public virtual void OnRun(InputValue value) { }
        public virtual void OnJump(InputValue value) { }
        public virtual void OnDash(InputValue value) { }
        public virtual void OnFire(InputValue value) { }
        #endregion

        #region Entity Collision
        public virtual void OnGrounded() { }
        #endregion
    }

    public class StateMachineEntity : TEntity
    {
        [SerializeField]
        protected int currentEState = EPlayerState.None;

        protected State[] states;
        protected StateContext _stateContext;

        protected Dictionary<int, Transition[]> state2transition;

        public StateContext stateContext
        {
            get { 
                if (_stateContext == null)
                    _stateContext = new StateContext();
                return _stateContext; 
            }
        }
        protected virtual bool State_Change(int ePlayerState) { return true; }

        protected virtual void Update_State()
        {
            State_UpdateContext();
            if (currentEState != 0)
            {
                var state = states[currentEState];
                if (state != null)
                {
                    state.Update(stateContext);
                    if (CheckTransition(currentEState, out int toState))
                    {
                        State_Change(toState);
                    }
                }
            }
        }
        protected virtual void State_UpdateContext(){ }
        public bool CheckTransition(int state,out int toState)
        {
            toState = 0; 
            if (!state2transition.TryGetValue(state, out var transitions))
                return false;
            for (int i = 0; i < transitions.Length; i++)
            {
                if (transitions[i].Check(states[state])) {
                    toState = transitions[i].toState;
                    if (states[toState] == null)
                        continue;
                    return true;
                }
            }
            return false;
        }

    }

    public interface IPoolItem
    {
        public void OnGet();
        public void OnRelease();
    }
    public abstract class Transition : IPoolItem
    {
        public int toState = -1;
        public bool inverse = false;

        protected Transition() { }
        protected Transition(int toState)
        { 
            this.toState = toState;
        }
        public virtual bool Check(State state) => true;

        public float canExitNormalizeTime = 0;
        public Transition SetCanExitNormalizeTime(float canExitNormalizeTime = 0f)
        {
            this.canExitNormalizeTime = canExitNormalizeTime;
            return this;
        }
        public Transition SetInverse(bool inverse)
        { 
            this.inverse = inverse;
            return this;
        }

        public Transition SetToState(int toState)
        { 
            this.toState = toState;
            return this;
        }

        public virtual void OnGet() { }
        public virtual void OnRelease() { }
    }
    public abstract class Transition<TransitionType> : Transition where TransitionType : Transition<TransitionType>
    {
        private static Dictionary<Type, Queue<TransitionType>> typeToTransition;
        protected Transition() { }
        protected Transition(int toState) : base(toState)
        {
        }
        private static Queue<TransitionType> GetQueue()
        {
            if (typeToTransition == null)
                typeToTransition = new Dictionary<Type, Queue<TransitionType>>();

            Type type = typeof(TransitionType);
            if (!typeToTransition.TryGetValue(typeof(TransitionType), out var queue))
            {
                queue = new Queue<TransitionType>();
                typeToTransition[type] = queue;
            }
            return queue;
        }
        public static TransitionType Get(int toState)
        {
            var list = GetQueue();

            TransitionType transition = null;
            if (list.Count > 0)
            { 
                transition = list.Dequeue();
            }
            else
            { 
                transition = (TransitionType)Activator.CreateInstance(typeof(TransitionType),true);
                transition.toState = toState;
            }

            transition.OnGet();
            return transition;
        }

        public static bool Release(TransitionType transition)
        {
            if(transition == null)
                return false;
            var list = GetQueue();
            list.Enqueue(transition);

            transition.OnRelease();
            return true;
        }
    }
    public class Trans_OnGrounded : Transition<Trans_OnGrounded>
    {
        protected Trans_OnGrounded() { }
        public override bool Check(State state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            return state.entity.stateContext.grounded > 0;
        }
    }
    public class Trans_OnStateFinish : Transition<Trans_OnStateFinish>
    {
        protected Trans_OnStateFinish() { }
        public override bool Check(State state)
        {
            if (state.normalizeTime < canExitNormalizeTime)
                return false;
            return state.phase == State.Phase.End;
        }
    }
    public class Trans_OnWalking : Transition<Trans_OnWalking>
    {
        protected Trans_OnWalking() { }
        public override bool Check(State state)
        {
            return state.entity.stateContext.inputAxi.magnitude > 0;
        }
    }
    public class Trans_OnRunning : Transition<Trans_OnRunning>
    {
        protected Trans_OnRunning() { }
        public override bool Check(State state)
        {
            var context = state.entity.stateContext;
            if (context.inputAxi.magnitude > 0)
            {
                return inverse ? (context.runValue <= 0) : context.runValue > 0;
            }
            return inverse;
        }
    }

    public class StateContext
    {
        public Vector2 inputAxi;
        public Vector2 mouseDelta;
        public float runValue;
        public int grounded;
        struct State { }
    }

    public struct DirectionNameSet { public string F, FL, FR, B, BL, BR; }

    public static class AnimationNameSet 
    {
        public const string IDLE = "Idle";

        public const string JUMP_START = "Jump_Start";
        public const string JUMP_KEEP = "Jump_Keep";
        public const string JUMP_LAND_W = "Jump_Land_Wait";
        public const string JUMP_LAND_M = "Jump_Land_Move";

        public const string WALK_F = "Walk_Front";
        public const string WALK_FL = "Walk_Front_L";
        public const string WALK_FR = "Walk_Front_R";
        public const string WALK_B = "Walk_Back";
        public const string WALK_BL = "Walk_Back_L";
        public const string WALK_BR = "Walk_Back_R";

        public const string DASH_F = "Dash_Front";
        public const string DASH_FL = "Dash_Front_L";
        public const string DASH_FR = "Dash_Front_R";
        public const string DASH_B = "Dash_Back";
        public const string DASH_BL = "Dash_Back_L";
        public const string DASH_BR = "Dash_Back_R";


        public static DirectionNameSet Walk = new DirectionNameSet()
        {
            F = WALK_F,
            FL = WALK_FL,
            FR = WALK_FR,
            B = WALK_B,
            BL = WALK_BL,
            BR = WALK_BR,
        };
        public static DirectionNameSet Dash = new DirectionNameSet()
        {
            F = DASH_F,
            FL = DASH_FL,
            FR = DASH_FR,
            B = DASH_B,
            BL = DASH_BL,
            BR = DASH_BR,
        };
    }
}