using System;
using System.Collections;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PJR
{
    public class ItemRepopHandler :MonoBehaviour
    {
        [LabelText("再生次数")]
        [Tooltip("<=0为无限次数，>0对应次数")]
        public int CanRegenerateTimes;
        public ItemConfigAsset itemconfig;
        public bool isMugen => itemconfig.CanRegenerateTimes < 1;
        [LabelText("再生间隔")]
        public float interval;
        protected virtual void GenFunc()
        {

        }
        public virtual IEnumerator CountDown(Action action)
        {
            yield return new WaitForSeconds(interval);
            action();
        }
    }
    public class ScriptTypeRestrictionAttribute : PropertyAttribute
    {
        public Type allowedType;

        public ScriptTypeRestrictionAttribute(Type allowedType)
        {
            this.allowedType = allowedType;
        }
    }
}

