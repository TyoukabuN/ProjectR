#if UNITY_EDITOR
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static AnimationTest;

public class FieldTest : BaseClass
{
    //[HideInInspector]
    //private EventFrameType type;
    //public virtual EventFrameType EventType
    //{
    //    get { return type; }
    //    protected set { type = value; }
    //}
}


public class BaseClass : MonoBehaviour
{
    [SerializeField]
    [HideInInspector]
    private EventFrameType eventType;

    [ShowInInspector]
    public virtual EventFrameType EventType
    {
        get { return eventType; }
        protected set { eventType = value; }
    }
}
#endif
