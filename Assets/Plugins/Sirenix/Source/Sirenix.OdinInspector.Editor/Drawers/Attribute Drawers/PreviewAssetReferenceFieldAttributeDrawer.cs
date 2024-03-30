using Sirenix.OdinInspector.Editor.ActionResolvers;
using UnityEngine.AddressableAssets;

#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="PreviewFieldAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
#pragma warning disable

    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="PreviewFieldAttribute"/> as a square ObjectField which renders a preview for UnityEngine.Object types.
    /// This object field also adds support for drag and drop, dragging an object to another square object field, swaps the values.
    /// If you hold down control while letting go it will replace the value, And you can control + click the object field to quickly delete the value it holds.
    /// </summary>
    [AllowGUIEnabledForReadonly]
    public sealed class PreviewAssetReferenceFieldAttributeDrawer<TR, T> : OdinAttributeDrawer<PreviewAssetReferenceFieldAttribute, TR>
        where T : UnityEngine.Object
        where TR : AssetReferenceT<T>
    {
        public ActionResolver OnPreviewIconRect;

        protected override void Initialize()
        {
            if(!string.IsNullOrEmpty(this.Attribute.OnPreviewGUIRect))
                OnPreviewIconRect = ActionResolver.Get(this.Property, this.Attribute.OnPreviewGUIRect, new ActionResolvers.NamedValue("rect", typeof(Rect)));
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            this.CallNextDrawer(label);

            ObjectFieldAlignment alignment;

            if (this.Attribute.AlignmentHasValue)
            {
                alignment = (ObjectFieldAlignment)this.Attribute.Alignment;
            }
            else
            {
                alignment = GeneralDrawerConfig.Instance.SquareUnityObjectAlignment;
            }

            if (this.ValueEntry.SmartValue is AssetReference ar)
            {
                var resultObject = SirenixEditorFields.UnityPreviewObjectField(
                    label: string.Empty,
                    value: ar.editorAsset as UnityEngine.Object,
                    objectType : typeof(T),
                    false,
                    this.Attribute.Height == 0 ? GeneralDrawerConfig.Instance.SquareUnityObjectFieldHeight : this.Attribute.Height,
                    alignment
                );
                if (resultObject != ar.editorAsset)
                {
                    ar.SetEditorAsset(resultObject);
                }
                if (OnPreviewIconRect != null && !OnPreviewIconRect.HasError)
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    OnPreviewIconRect.Context.NamedValues.Set("rect", rect);
                    OnPreviewIconRect.DoAction();
                }
            }


            // if (EditorGUI.EndChangeCheck())
            // {
            //     this.ValueEntry.Values.ForceMarkDirty();
            // }
        }
    }
}
#endif
