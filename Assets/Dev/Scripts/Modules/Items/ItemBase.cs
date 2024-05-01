using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PJR
{
    public class ItemBase :MonoBehaviour
    {
        [LabelText("��������")]
        [Tooltip("<=0Ϊ���޴�����>0��Ӧ����")]
        public int CanRegenerateTimes;
        public ItemConfig itemconfig;
        public bool isMugen => itemconfig.CanRegenerateTimes < 1;
        [LabelText("�������")]
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
        public System.Type allowedType;

        public ScriptTypeRestrictionAttribute(System.Type allowedType)
        {
            this.allowedType = allowedType;
        }
    }
}

