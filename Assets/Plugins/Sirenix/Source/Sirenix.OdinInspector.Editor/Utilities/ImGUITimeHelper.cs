//-----------------------------------------------------------------------
// <copyright file="EditorPrefsUtilities.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;
    using System.Diagnostics;
    using UnityEngine;

    //public struct ImGUITimeHelper
    //{
    //    private static Stopwatch stopWatch = Stopwatch.StartNew();

    //    private float deltaTime;
    //    public double LastTime;
    //    public EventType TrackingEvent;
    //    public bool ThrowExceptionOnBadUsage;

    //    public float DeltaTime
    //    {
    //        get
    //        {
    //            if (this.ThrowExceptionOnBadUsage && (Event.current == null || Event.current.type != this.TrackingEvent))
    //            {
    //                throw new Exception($"DeltaTime may only be used during {this.TrackingEvent} events.");
    //            }

    //            return this.deltaTime;
    //        }
    //    }

    //    public static ImGUITimeHelper Create(EventType trackingEvent, bool throwExceptionOnBadUsage = true)
    //    {
    //        return new ImGUITimeHelper()
    //        {
    //            deltaTime = 0.02f,
    //            LastTime = stopWatch.Elapsed.TotalSeconds,
    //            TrackingEvent = trackingEvent,
    //            ThrowExceptionOnBadUsage = throwExceptionOnBadUsage,
    //        };
    //    }

    //    public void Update()
    //    {
    //        if (this.TrackingEvent == default(EventType) || Event.current.type == this.TrackingEvent)
    //        {
    //            var time = stopWatch.Elapsed.TotalSeconds;
    //            var newDeltaTime = (float)(time - this.LastTime);

    //            if (newDeltaTime <= 0.2f)
    //            {
    //                this.deltaTime = newDeltaTime;
    //            }

    //            this.LastTime = time;
    //        }
    //    }
    //}
}
#endif