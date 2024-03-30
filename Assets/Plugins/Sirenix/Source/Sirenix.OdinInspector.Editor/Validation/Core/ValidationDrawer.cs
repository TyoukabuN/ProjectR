//-----------------------------------------------------------------------
// <copyright file="ValidationDrawer.cs" company="Sirenix ApS">
// Copyright (c) Sirenix ApS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
#if UNITY_EDITOR
namespace Sirenix.OdinInspector.Editor.Validation
{
#pragma warning disable

    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using UnityEngine;

    [DrawerPriority(0, 10000, 0)]
    public class ValidationDrawer<T> : OdinValueDrawer<T>, IDisposable
    {
        private List<ValidationResult> oldValidationResults;
        private List<ValidationResult> validationResults;
        private bool rerunFullValidation;
        private bool revalidateEveryFrame;
        private object shakeGroupKey;

        private bool subscribedToEvents;

        private ValidationComponent validationComponent;

        protected override bool CanDrawValueProperty(InspectorProperty property)
        {
            var validation = property.GetComponent<ValidationComponent>();
            if (validation == null) return false;
            if (property.GetAttribute<DontValidateAttribute>() != null) return false;
            return validation.ValidatorLocator.PotentiallyHasValidatorsFor(property);
        }

        protected override void Initialize()
        {
            this.validationComponent = this.Property.GetComponent<ValidationComponent>();
            this.validationComponent.ValidateProperty(ref this.validationResults, true);

            if (this.validationResults.Count > 0)
            {
                this.shakeGroupKey = UniqueDrawerKey.Create(this.Property, this);

                this.Property.Tree.OnUndoRedoPerformed += this.OnUndoRedoPerformed;
                this.ValueEntry.OnValueChanged += this.OnValueChanged;
                this.ValueEntry.OnChildValueChanged += this.OnChildValueChanged;
                this.subscribedToEvents = true;

                foreach (var result in this.validationResults)
                {
                    if (result.Setup.Validator.RevalidationCriteria == RevalidationCriteria.Always)
                    {
                        this.revalidateEveryFrame = true;
                    }

                    ValidationEvents.InvokeOnValidationStateChanged(new ValidationStateChangeInfo()
                    {
                        ValidationResult = result,
                    });
                }
            }
            else
            {
                this.SkipWhenDrawing = true;
            }
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.validationResults.Count == 0)
            {
                this.CallNextDrawer(label);
                return;
            }

            GUILayout.BeginVertical();
            SirenixEditorGUI.BeginShakeableGroup(this.shakeGroupKey);

            if (Event.current.type == EventType.Layout && (this.revalidateEveryFrame || this.rerunFullValidation))
            {
                var results = this.oldValidationResults;
                this.oldValidationResults = this.validationResults;

                if (results != null)
                    results.Clear();

                this.validationComponent.ValidateProperty(ref results, true);
                this.validationResults = results;

                //int oldErrorCount = 0,
                //    oldWarningCount = 0,
                //    newErrorCount = 0,
                //    newWarningCount = 0;

                //for (int i = 0; i < results.Count; i++)
                //{
                //    var result = results[i];

                //    if (result.ResultType == ValidationResultType.Error)
                //        newErrorCount++;
                //    else if (result.ResultType == ValidationResultType.Warning)
                //        newWarningCount++;
                //}

                //for (int i = 0; i < this.oldValidationResults.Count; i++)
                //{
                //    var result = this.oldValidationResults[i];

                //    if (result.ResultType == ValidationResultType.Error)
                //        oldErrorCount++;
                //    else if (result.ResultType == ValidationResultType.Warning)
                //        oldWarningCount++;
                //}

                bool shakeGroup = false;

                //if (newErrorCount > oldErrorCount || newWarningCount > oldWarningCount)
                //{
                //    shakeGroup = true;
                //}

                if (results.Count != this.oldValidationResults.Count)
                {
                    shakeGroup = true;

                    for (int i = 0; i < results.Count; i++)
                    {
                        ValidationEvents.InvokeOnValidationStateChanged(new ValidationStateChangeInfo()
                        {
                            ValidationResult = results[i],
                        });
                    }
                }
                else
                {
                    for (int i = 0; i < results.Count; i++)
                    {
                        var result = results[i];

                        if (!result.IsMatch(this.oldValidationResults[i]))
                        {
                            if (result.ResultType == ValidationResultType.Error || result.ResultType == ValidationResultType.Warning)
                            {
                                shakeGroup = true;
                            }

                            ValidationEvents.InvokeOnValidationStateChanged(new ValidationStateChangeInfo()
                            {
                                ValidationResult = result,
                            });
                        }
                    }
                }

                if (shakeGroup)
                {
                    SirenixEditorGUI.StartShakingGroup(this.shakeGroupKey);
                }
            }

            for (int i = 0; i < this.validationResults.Count; i++)
            {
                var result = this.validationResults[i];
                UnityEditor.MessageType messageType = UnityEditor.MessageType.None;

                if (result.ResultType == ValidationResultType.Error)
                {
                    messageType = UnityEditor.MessageType.Error;
                }
                else if (result.ResultType == ValidationResultType.Warning)
                {
                    messageType = UnityEditor.MessageType.Warning;
                }
                else if (result.ResultType == ValidationResultType.Valid && !string.IsNullOrEmpty(result.Message))
                {
                    messageType = UnityEditor.MessageType.Info;
                }

                if (messageType != UnityEditor.MessageType.None)
                {
                    SirenixEditorGUI.MessageBox(result.Message, messageType, SirenixGUIStyles.MessageBox, true, result[0].OnContextClick);
                }

                //#if !SIRENIX_INTERNAL
                //#error This is temporary UI, for gods sake make Tor or someone make it nicer. Also, only show these when the validator is installed.
                //#endif

// #if SIRENIX_INTERNAL
//                 ref var entry = ref result[0];
//
//                 if (entry.Fix != null && entry.Fix.OfferInInspector)
//                 {
//                     SirenixEditorGUI.InfoMessageBox("TODO: NICER FIX BUTTONZ");
//
//                     var title = entry.Fix.Title ?? "Fix";
//
//                     if (entry.Fix.Action is Action action)
//                     {
//                         if (GUILayout.Button(title))
//                         {
//                             this.Property.RecordForUndo(title, true);
//                             action();
//                             GUIUtility.ExitGUI();
//                         }
//                     }
//                     else
//                     {
//                         GUIHelper.PushGUIEnabled(false);
//                         GUILayout.Label(title + " (Fixes with args not yet supported)", SirenixGUIStyles.Button);
//                         GUIHelper.PopGUIEnabled();
//                     }
//                 }
// #endif
            }

            if (Event.current.type == EventType.Layout)
            {
                this.rerunFullValidation = false;
            }

            this.CallNextDrawer(label);
            SirenixEditorGUI.EndShakeableGroup(this.shakeGroupKey);
            GUILayout.EndVertical();
        }

        public void Dispose()
        {
            if (this.subscribedToEvents)
            {
                this.Property.Tree.OnUndoRedoPerformed -= this.OnUndoRedoPerformed;
                this.ValueEntry.OnValueChanged -= this.OnValueChanged;
                this.ValueEntry.OnChildValueChanged -= this.OnChildValueChanged;
            }

            this.validationResults = null;
            this.oldValidationResults = null;
        }

        private void OnUndoRedoPerformed()
        {
            this.rerunFullValidation = true;
        }

        private void OnValueChanged(int index)
        {
            this.rerunFullValidation = true;
        }

        private void OnChildValueChanged(int index)
        {
            this.rerunFullValidation = true;
        }
    }
}
#endif