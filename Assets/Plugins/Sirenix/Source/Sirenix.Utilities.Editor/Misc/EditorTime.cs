//-----------------------------------------------------------------------
// <copyright file="EditorTime.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.Utilities.Editor
{
#pragma warning disable

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UIElements;
    using Utilities;
    using Debug = UnityEngine.Debug;

    /// <summary>
    /// A utility class for getting delta time for the GUI editor.
    /// </summary>
    public class GUITimeHelper
    {
        private static object topLevelFallback = new object();
        private static Func<object> topLevelGetter = DeepReflection.CreateValueGetter<object>(typeof(GUILayoutUtility), "current.windows", false);
        private static Func<Stack<IMGUIContainer>> imGUIContainerStack = DeepReflection.CreateValueGetter<Stack<IMGUIContainer>>(typeof(VisualElement).Assembly.GetType("UnityEngine.UIElements.UIElementsUtility"), "s_ContainerStack", false);

        // Gets us an object, which hashcode is unique to the current gui frame. (Layout + Mousemove + ect + Repaint) = 1 frame.

        private const float fallbackDeltaTime = 0.02f;
        private static Stopwatch sw = Stopwatch.StartNew();

        private static int layout_prevTopLevelHash;
        private static int repaint_prevTopLevelHash;
        private static Dictionary<Key, GUITimeHelper> layout_windowTimeHelpers = new Dictionary<Key, GUITimeHelper>();
        private static Dictionary<Key, GUITimeHelper> repaint_windowTimeHelpers = new Dictionary<Key, GUITimeHelper>();
        private static GUITimeHelper layout_currentTimeHelper = new GUITimeHelper(EventType.Layout);
        private static GUITimeHelper repaint_currentTimeHelper = new GUITimeHelper(EventType.Layout);

        public static float RepaintDeltaTime
        {
            get
            {
                var topLevel = topLevelGetter();
                if (topLevel == null)
                    topLevel = topLevelFallback;

                var topLevelHash = topLevel.GetHashCode();

                if (repaint_prevTopLevelHash != topLevelHash)
                {
                    var currView = GUIHelper.GUIViewGetter() as UnityEngine.Object;
                    if (currView == null)
                        return fallbackDeltaTime;

                    var key = new Key();
                    key.WindowHash = currView.GetInstanceID();
                    var stack = imGUIContainerStack();
                    if (stack != null && stack.Count > 0)
                        key.CurrentContainer = stack.Peek();

                    // Get or create a new GUITimeHelper for the current window / GUIView.
                    if (!repaint_windowTimeHelpers.TryGetValue(key, out var time))
                    {
                        time = repaint_windowTimeHelpers[key] = new GUITimeHelper(EventType.Repaint);
                    }

                    repaint_prevTopLevelHash = topLevelHash;

                    time.Update();
                    repaint_currentTimeHelper = time;
                }

                return repaint_currentTimeHelper.deltaTime;
            }
        }

        public static float LayoutDeltaTime
        {
            get
            {
                var topLevel = topLevelGetter();
                if (topLevel == null)
                    topLevel = topLevelFallback;

                var topLevelHash = topLevel.GetHashCode();

                if (layout_prevTopLevelHash != topLevelHash)
                {
                    var currView = GUIHelper.GUIViewGetter() as UnityEngine.Object;
                    if (currView == null)
                        return fallbackDeltaTime;

                    var key = new Key();
                    key.WindowHash = currView.GetInstanceID();

                    var stack = imGUIContainerStack();
                    if (stack != null && stack.Count > 0)
                        key.CurrentContainer = stack.Peek();

                    // Get or create a new GUITimeHelper for the current window / GUIView.
                    if (!layout_windowTimeHelpers.TryGetValue(key, out var time))
                    {
                        time = layout_windowTimeHelpers[key] = new GUITimeHelper(EventType.Layout);
                    }

                    layout_prevTopLevelHash = topLevelHash;

                    time.Update();
                    layout_currentTimeHelper = time;
                }

                return layout_currentTimeHelper.deltaTime;
            }
        }


        private float deltaTime;
        private double lastTime;
        private EventType trackingEvent;

        private GUITimeHelper(EventType trackingEvent)
        {
            this.deltaTime = fallbackDeltaTime;
            this.lastTime = sw.Elapsed.TotalSeconds;
            this.trackingEvent = trackingEvent;
        }

        private void Update()
        {
            if (Event.current.type == this.trackingEvent)
            {
                var time = sw.Elapsed.TotalSeconds;
                var newDeltaTime = (float)(time - this.lastTime);

                if (newDeltaTime <= 0.2f)
                {
                    this.deltaTime = newDeltaTime;
                }

                this.lastTime = time;
            }
        }

        private struct Key : IEquatable<Key>
        {
            public int WindowHash;
            public UnityEngine.UIElements.IMGUIContainer CurrentContainer;

            public override bool Equals(object obj) => obj is Key key && this.Equals(key);
            public bool Equals(Key other) => this.WindowHash == other.WindowHash && EqualityComparer<IMGUIContainer>.Default.Equals(this.CurrentContainer, other.CurrentContainer);
            public override int GetHashCode() => this.WindowHash + this.CurrentContainer.GetHashCode();
        }
    }




    /// <summary>
    /// A utility class for getting delta time for the GUI editor.
    /// </summary>
    [Obsolete("Use GUITimeHelper.LayoutDeltaTime or GUITimeHelper.RepaintDeltaTime instead depending on which event you're tracking delta time in.", Consts.IsSirenixInternal)]
    public class EditorTimeHelper
    {
        public static readonly EditorTimeHelper Time = new EditorTimeHelper();
        public float DeltaTime => GUITimeHelper.LayoutDeltaTime;
        public void Update()
        {
        }
    }
}
#endif