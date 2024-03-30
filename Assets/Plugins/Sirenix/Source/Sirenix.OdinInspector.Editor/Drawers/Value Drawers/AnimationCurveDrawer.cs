//-----------------------------------------------------------------------
// <copyright file="AnimationCurveDrawer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using Sirenix.Utilities.Editor;

#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using Sirenix.Utilities;
    using System;
    using System.Reflection;
    using UnityEngine;

    /// <summary>
    /// Animation curve property drawer.
    /// </summary>
    public sealed class AnimationCurveDrawer : DrawWithUnityBaseDrawer<AnimationCurve>
    {
        private AnimationCurve[] curvesLastFrame;
        private static Action clearCache;
        private static IAtomHandler<AnimationCurve> atomHandler = AtomHandlerLocator.GetAtomHandler<AnimationCurve>();

        #region Modified By Hunter (jb) -- 2023年1月12日

        private bool drawEdit;
        private Vector2 scaleTime;

        #endregion
        static AnimationCurveDrawer()
        {
            MethodInfo mi = null;
            var type = AssemblyUtilities.GetTypeByCachedFullName("UnityEditorInternal.AnimationCurvePreviewCache");
            if (type != null)
            {
                var method = type.GetMethod("ClearCache", Flags.StaticAnyVisibility);
                var pars = method.GetParameters();
                if (pars != null && pars.Length == 0)
                {
                    mi = method;
                }
            }

            if (mi != null)
            {
                clearCache = EmitUtilities.CreateStaticMethodCaller(mi);
            }
#if SIRENIX_INTERNAL
            else
            {
                Debug.LogError("AnimationCurve fix no longer works, has Unity fixed it?");
            }
#endif
        }

        protected override void Initialize()
        {
            base.Initialize();

            if (clearCache != null)
            {
                // Unity bugfix:
                // The preview of animations curves doesn't work well with reordering, 
                // I suspect they use ControlId's as the pointer to the preview cache lookup.
                clearCache();

                this.curvesLastFrame = new AnimationCurve[this.ValueEntry.ValueCount];

                for (int i = 0; i < this.ValueEntry.ValueCount; i++)
                {
                    var value = this.ValueEntry.Values[i];
                    this.curvesLastFrame[i] = atomHandler.CreateInstance();
                    atomHandler.Copy(ref value, ref this.curvesLastFrame[i]);
                }
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (clearCache != null)
            {
                for (int i = 0; i < this.ValueEntry.ValueCount; i++)
                {
                    if (atomHandler.Compare(this.curvesLastFrame[i], this.ValueEntry.Values[i]) == false)
                    {
                        // An animation curve was changed from the outside!
                        clearCache();
                        break;
                    }
                }
            }

            GUILayout.BeginHorizontal();
            base.DrawPropertyLayout(label);
            #region Modified By Hunter (jb) -- 2023年1月12日

            if (SirenixEditorGUI.IconButton(EditorIcons.Pen))
            {
                drawEdit = !drawEdit;
            }
            GUILayout.EndHorizontal();
            if (drawEdit)
            {
                this.scaleTime = UnityEditor.EditorGUILayout.Vector2Field(GUIHelper.TempContent("时间调整"), scaleTime);
                if (GUILayout.Button(GUIHelper.TempContent("应用")))
                {
                    foreach (var curve in this.ValueEntry.Values)
                    {
                        if (curve != null && curve.keys != null && curve.keys.Length > 1)
                        {
                            var first = curve.keys[0];
                            var last = curve.keys[curve.keys.Length- 1];
                            var oriStart = first.time;
                            var oriTime = last.time - first.time;
                            var scaleTo = this.scaleTime.y - scaleTime.x;
                            if (scaleTo <= 0) oriTime = scaleTo;
                            var scale = scaleTo / oriTime;
                            for (int i = 0; i < curve.keys.Length; i++)
                            {
                                var k = curve.keys[i];
                                var t = k.time;
                                var newTime = scaleTime.x + (t - oriStart) * scale;
                                curve.keys[i] = new Keyframe(newTime, k.value, k.inTangent, k.outTangent, k.inWeight, k.outWeight);
                            }
                        }    
                    }
                    
                }
            }

            #endregion

            

            if (clearCache != null)
            {
                for (int i = 0; i < this.ValueEntry.ValueCount; i++)
                {
                    var value = this.ValueEntry.Values[i];
                    atomHandler.Copy(ref value, ref this.curvesLastFrame[i]);
                }
            }
        }
    }
}
#endif