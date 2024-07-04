using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PJR
{
    public interface IActionControl {
        void OnActionControlBegin(ExtraAction extraAction);
        void OnActionControlUpdate(ExtraAction extraAction);
        void OnActionControlComplete(ExtraAction extraAction);
        void OnActionControlRemove(ExtraAction extraAction);
    }
    public class ExtraAction: ExtraControl
    {
        public int actionType;
        public IActionControl target;
        public EActionEvent tActionEvent;
        public ExtraAction(int actionType, float duration) : base(duration)
        {
            Init(actionType, duration);
        }
        public void Init(int actionType, float duration)
        {
            base.Init(duration);
            this.actionType = actionType;
        }

        public override void OnUpdate()
        {
            if (target == null)
                return;
            target.OnActionControlUpdate(this);
        }

        public override void OnBegin()
        {
            if (!m_onBeginTrigger)
                return;
            m_onBeginTrigger = false;
            if (target == null)
                return;
            target.OnActionControlBegin(this);
        }
        public override void OnComplete()
        {
            if (!m_onCompleteTrigger)
                return;
            m_onCompleteTrigger = false;
            if (onComplete != null)
            {
                try
                {
                    onComplete.Invoke();
                }
                catch (Exception e)
                {
                    LogSystem.LogError(e.ToString());
                }
            }
            if (target == null)
                return;
            target.OnActionControlComplete(this);
        }
    }

    public static class ExtraActionExtension
    {
        static Dictionary<IActionControl, Dictionary<int, ExtraAction>> m_MapDict = new Dictionary<IActionControl, Dictionary<int, ExtraAction>>();

        public static void Clear(this IActionControl target)
        {
            var repo = RegisterExtraActionMap(target);
            if (repo == null)
                return;
            repo.Clear();
        }
        public static Dictionary<int, ExtraAction> RegisterExtraActionMap(this IActionControl obj)
        {
            Dictionary<int, ExtraAction> temp;
            if (obj == null)
                return null;
            if (m_MapDict.TryGetValue(obj, out temp))
                return temp;
            temp = new Dictionary<int, ExtraAction>();
            m_MapDict.Add(obj, temp);
            return temp;
        }

        public static void UnregisterExtraActionMap(this IActionControl obj)
        {
            if (obj == null && !m_MapDict.ContainsKey(obj))
                return;
            m_MapDict.Remove(obj);
        }

        public static ExtraAction ExtraActionMapAdd(this IActionControl target,int key, float duration,bool coverExist = true)
        {
            var repo = RegisterExtraActionMap(target);
            if (repo == null)
                return null;

            ExtraAction value = null;
            if (!repo.TryGetValue(key, out value))
            {
                value = new ExtraAction(key, duration);
                value.target = target;
                repo.Add(key, value);
            }
            else if(!coverExist && !value.IsPersistent())
            {
                duration += value.counter >= 0 ? value.counter : 0;
            }
            value.Init(key, duration );
            return value;
        }

        public static ExtraAction ExtraActionMapAdd(this IActionControl target, EActionType actionType, float duration)
        {
            return ExtraActionMapAdd(target, (int)actionType, duration);
        }

        public static void UpdateExtraAction(this IActionControl target, float deltaTime)
        {
            var map = RegisterExtraActionMap(target);

            if (map != null)
            {
                //clear invaild ExtendSpeedMap item
                int invalidKey = -1;
                do
                {
                    invalidKey = -1;
                    foreach (var item in map)
                    {
                        if (item.Value.IsValid())
                            continue;
                        invalidKey = item.Key;
                        break;
                    }
                    if (invalidKey >= 0)
                        target.ExtraActionMapRemove(invalidKey);
                }
                while (invalidKey >= 0);
                //
                foreach (var item in map)
                {
                    if (!item.Value.IsValid())
                        continue;
                    item.Value.Update(Time.fixedDeltaTime);
                }
            }
        }
        public static void ExtraActionMapRemove(this IActionControl target, int key,bool isComplete = true)
        {
            var repo = RegisterExtraActionMap(target);
            if (repo == null)
                return;
            ExtraAction value = null;
            if (!repo.TryGetValue(key, out value))
                return;
            repo.Remove(key);
            if(isComplete)
                target.OnActionControlRemove(value);
        }
        public static void ExtraActionMapRemove(this IActionControl target, EActionType actionType, bool isComplete = true)
        {
            ExtraActionMapRemove(target, (int)actionType, isComplete);
        }
        public static bool ExtraActionMapExist(this IActionControl target, int key)
        {
            var repo = RegisterExtraActionMap(target);
            if (repo == null)
                return false;
            return repo.ContainsKey(key);
        }

        public static ExtraAction GetExtraActionMap(this IActionControl target, EActionType actionType)
        {
            return GetExtraActionMap(target, (int)actionType);
        }
        public static ExtraAction GetExtraActionMap(this IActionControl target, int key)
        {
            var repo = RegisterExtraActionMap(target);
            if (repo == null)
                return null;
            ExtraAction taction = null;
            repo.TryGetValue(key, out taction);
            return taction;
        }
        public static bool ExtraActionMapExist(this IActionControl target, EActionType actionType)
        {
            return ExtraActionMapExist(target,(int)actionType);
        }
    }
}