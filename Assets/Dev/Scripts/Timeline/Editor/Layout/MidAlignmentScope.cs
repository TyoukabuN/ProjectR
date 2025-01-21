using System;
using UnityEngine;

namespace PJR.Timeline.Editor
{
    public static class MidAlignmentScope
    {
        /// <summary>
        /// 想在Vertical里的Horizontal方向居中
        /// </summary>
        public class Vertical : IDisposable
        {
            /// <summary>
            /// 想在Vertical里的Horizontal方向居中
            /// </summary>
            public Vertical() : this(null) { }
            public Vertical(params GUILayoutOption[] options)
            {
                GUILayout.BeginHorizontal(options);
                GUILayout.FlexibleSpace();

            }
            public void Dispose()
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 想在Horizontal里的Vertical方向居中
        /// </summary>
        public class Horizontal : IDisposable
        {
            /// <summary>
            /// 想在Horizontal里的Vertical方向居中
            /// </summary>
            public Horizontal() : this(null) { }
            public Horizontal(params GUILayoutOption[] options)
            {
                GUILayout.BeginVertical(options);
                GUILayout.FlexibleSpace();
            }
            public void Dispose()
            {
                GUILayout.FlexibleSpace();
                GUILayout.EndVertical();
            }
        }
    }
}
