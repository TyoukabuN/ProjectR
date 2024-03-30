using System;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.ActionResolvers;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.TestTools.Constraints;
using NamedValue = Sirenix.OdinInspector.Editor.ValueResolvers.NamedValue;

namespace Sirenix.OdinInspector.Editor
{
    public class HunterPropertySearchFilter :PropertySearchFilter
    {
        private readonly ValueResolver<bool> CustomMatchFunc;
        private readonly ActionResolver CustomDrawBeforeSearchBar;
        private readonly ValueResolver<bool> CustomSearchIsEmpty;
        private readonly ValueResolver<bool> CustomFilterIsDirtyFunc;


        public HunterPropertySearchFilter(InspectorProperty property) : base(property)
        {
        }

        public HunterPropertySearchFilter(InspectorProperty property, SearchableAttribute config) : base (property, config)
        {
            if (config is HunterSearchableAttribute hs)
            {
                if (hs.CustomMatchFunc != null)
                {
                    this.CustomMatchFunc = ValueResolver.Get<bool>(property, hs.CustomMatchFunc, new ValueResolvers.NamedValue("object", typeof(object)));
                }

                if (hs.DrawBeforeSearchBar != null)
                {
                    this.CustomDrawBeforeSearchBar = ActionResolver.Get(property, hs.DrawBeforeSearchBar);
                }

                if (hs.CustomFilterDirtyFunc != null)
                {
                    this.CustomFilterIsDirtyFunc = ValueResolver.Get<bool>(property, hs.CustomFilterDirtyFunc);
                }

                if (hs.CustomSearchIsEmpty != null)
                {
                    this.CustomSearchIsEmpty = ValueResolver.Get<bool>(property, hs.CustomSearchIsEmpty);
                }
            }

            this.Recursive = false;
        }

        public override bool IsMatch(InspectorProperty property, string searchTerm)
        {
            if (this.CustomMatchFunc == null || this.CustomMatchFunc.HasError)
            {
                return base.IsMatch(property, searchTerm);
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                return base.IsMatch(property, searchTerm) && IsCustomMatch(property);    
            }
            return IsCustomMatch(property);
        }

        private bool IsCustomMatch(InspectorProperty prop)
        {
            if (this.CustomMatchFunc == null || this.CustomMatchFunc.HasError)
            {
                return true;
            }

            try
            {
                CustomMatchFunc.Context.NamedValues.Set("object", prop.ValueEntry.WeakSmartValue);
                var customFilterValue = CustomMatchFunc.GetValue();
                return customFilterValue;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return true;
            }
        }
        
        

        public override void DrawDefaultSearchFieldLayout(GUIContent label)
        {
            Rect rect = EditorGUILayout.GetControlRect(label != null);

            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            else
            {
                rect = rect.AddXMin(GUIHelper.CurrentIndentAmount);
            }

            DrawCustomSearchOptions(out var shouldUpdate);
            var newTerm = SirenixEditorGUI.SearchField(rect, this.SearchTerm, false, this.searchFieldControlName);
            
             
            if (newTerm != this.SearchTerm)
            {
                this.SearchTerm = newTerm;
                shouldUpdate = true;
            }

            shouldUpdate |= CheckCustomFilterDirty();
            
            if (shouldUpdate)
            {
                this.Property.Tree.DelayActionUntilRepaint(() =>
                {
                    this.UpdateSearch();
                    GUIHelper.RequestRepaint();
                });
            }

            var separatorRect = EditorGUILayout.GetControlRect(true, 3f);
            SirenixEditorGUI.DrawThickHorizontalSeperator(separatorRect.AddXMin(GUIHelper.CurrentIndentAmount));
            GUILayout.Space(2f);
        }

        
        private bool CheckCustomFilterDirty()
        {
            if (CustomFilterIsDirtyFunc != null && !CustomFilterIsDirtyFunc.HasError)
            {
                var customDirty = CustomFilterIsDirtyFunc.GetValue();
                if (customDirty)
                {
                    return true;
                }
            }
            return false;
        }

        public override bool CheckSearchIsEmtpy()
        {
            if (CustomSearchIsEmpty != null && !CustomSearchIsEmpty.HasError)
            {
                return CustomSearchIsEmpty.GetValue() && base.CheckSearchIsEmtpy();
            }
            return base.CheckSearchIsEmtpy();
        }

        public override bool DrawCollectionSearchField(string controlName)
        {
            DrawCustomSearchOptions(out var customSearchFieldChange);
            var searchFieldChanged = base.DrawCollectionSearchField(controlName);
            return customSearchFieldChange || searchFieldChanged || CheckCustomFilterDirty();
        }

        
        private void DrawCustomSearchOptions(out bool changed)
        {
            changed = false;
            if (this.CustomDrawBeforeSearchBar == null || CustomDrawBeforeSearchBar.HasError) return ;
            EditorGUI.BeginChangeCheck();
            CustomDrawBeforeSearchBar.DoAction();
            if (EditorGUI.EndChangeCheck())
            {
                // changed = true;
            }
            GUILayout.FlexibleSpace();
        }
    }
}

