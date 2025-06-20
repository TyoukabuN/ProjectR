using UnityEngine;
using System;

namespace PJR
{
    public class ExtraControl
    {
        public float duration = 1f;
        public Action onBegin;
        public Action onComplete;
        public Action<ExtraControl> onUpdate;
        public Easing.Ease easeType = Easing.Ease.Linear;
        //
        public float counter = 0f;
        protected bool m_onBeginTrigger = true;
        protected bool m_onCompleteTrigger = true;
        //
        public ExtraControl(float duration)
        {
            Init(duration);
        }
        public float counterNormalize
        {
            get { 
                return Easing.DoEasing(easeType, counter / duration) ; 
            }
        }
        public void Init(float duration)
        {
            this.duration = duration;
            this.counter = duration;
            m_onBeginTrigger = true;
            m_onCompleteTrigger = true;
        }
        public virtual void Reset()
        {
            this.counter = duration;
        }
        public virtual bool IsValid()
        {
            if (IsPersistent())
                return true;
            return counter > 0;
        }
        public virtual bool IsPersistent()
        {
            return duration < 0;
        }
        public virtual void Update(float deltaTime)
        {
            OnBegin();
            if(IsValid()) OnUpdate();
            counter -= deltaTime;
            if (!IsValid())
                counter = 0;
            if (IsPersistent())
                Reset();
            if (!IsValid()) OnComplete(); 
        }
        public virtual void OnUpdate()
        {
            try
            {
                onUpdate?.Invoke(this);
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }

        public virtual void OnBegin()
        {
            if (!m_onBeginTrigger)
                return;
            m_onBeginTrigger = false;
            try
            {
                onBegin?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
        public virtual void OnComplete()
        {
            if (!m_onCompleteTrigger)
                return;
            m_onCompleteTrigger = false;
            try
            {
                onComplete?.Invoke();
            }
            catch (Exception e)
            {
                Debug.LogError(e.ToString());
            }
        }
    }

}