//-----------------------------------------------------------------------
// <copyright file="SimpleProfiler.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor
{
#pragma warning disable

    using System;
    using UnityEngine;

    internal struct SimpleProfiler : IDisposable
    {
        public string Name;
        public System.Diagnostics.Stopwatch Watch;

        public static SimpleProfiler Section(string name)
        {
            return new SimpleProfiler()
            {
                //Name = name,
                //Watch = System.Diagnostics.Stopwatch.StartNew()
            };
        }

        public void Dispose()
        {
            //this.Watch.Stop();
            //Debug.Log(this.Name + " took " + this.Watch.Elapsed.TotalMilliseconds + "ms");
        }
    }
}
#endif