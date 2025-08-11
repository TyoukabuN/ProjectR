#if  UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PJR.BlackBoard.Inspector;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector.Editor.Drawers;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Serialization;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace PJR.BlackBoard.Editor.Drawers
{
    /// <summary>
    /// 重写了Odin的TypeFilterAttributeDrawer<br/>
    /// 会通过Type.GetTypeFilter()获取过滤后的类型<br/>
    /// 让点开类型选择菜单的时候不那么卡<br/>
    /// 也限制了可选类型的数量,不用找那么久
    /// </summary>
    [DrawerPriority(0.0, 0.0, 2002.0)]
    public sealed class TypeFilterAttributeDrawer : OdinAttributeDrawer<GenericTypeFilterAttribute>
    {
        private string error;
        private bool useSpecialListBehaviour;
        private Func<IEnumerable<ValueDropdownItem>> getValues;
        private Func<IEnumerable<object>> getSelection;
        private IEnumerable<object> result;
        private Dictionary<object, string> nameLookup;
        private ValueResolver<object> rawGetter;

        protected override bool CanDrawAttributeProperty(InspectorProperty property)
        {
            return property.ValueEntry != null;
        }

        /// <summary>Initializes this instance.</summary>
        protected override void Initialize()
        {
            // this.rawGetter = ValueResolver.Get<object>(this.Property, this.Attribute.FilterGetter);
            // this.error = this.rawGetter.ErrorMessage;
            this.useSpecialListBehaviour =
                this.Property.ChildResolver is ICollectionResolver && !this.Attribute.DrawValueNormally;
            this.getSelection = (Func<IEnumerable<object>>)(() => this.Property.ValueEntry.WeakValues.Cast<object>());
            this.getValues = (Func<IEnumerable<ValueDropdownItem>>)(() =>
                (Property.ValueEntry.TypeOfValue.GetTypeFilter() as IEnumerable).Cast<object>()
                .Where<object>((Func<object, bool>)(x => x != null))
                .Select<object, ValueDropdownItem>((Func<object, ValueDropdownItem>)(x =>
                {
                    switch (x)
                    {
                        case ValueDropdownItem valueDropdownItem3:
                            return valueDropdownItem3;
                        case IValueDropdownItem _:
                            IValueDropdownItem valueDropdownItem2 = x as IValueDropdownItem;
                            return new ValueDropdownItem(valueDropdownItem2.GetText(), valueDropdownItem2.GetValue());
                        default:
                            return new ValueDropdownItem((string)null, x);
                    }
                })));
            this.ReloadDropdownCollections();
        }

        private void ReloadDropdownCollections()
        {
            if (this.error != null)
                return;
            object obj = (object)null;
            if (Property.ValueEntry.TypeOfValue.GetTypeFilter() != null)
                obj = (Property.ValueEntry.TypeOfValue.GetTypeFilter() as IEnumerable).Cast<object>()
                    .FirstOrDefault<object>();
            else
                this.nameLookup = (Dictionary<object, string>)null;
        }

        private static IEnumerable<ValueDropdownItem> ToValueDropdowns(IEnumerable<object> query)
        {
            return query.Select<object, ValueDropdownItem>((Func<object, ValueDropdownItem>)(x =>
            {
                switch (x)
                {
                    case ValueDropdownItem valueDropdowns2:
                        return valueDropdowns2;
                    case IValueDropdownItem _:
                        IValueDropdownItem valueDropdownItem = x as IValueDropdownItem;
                        return new ValueDropdownItem(valueDropdownItem.GetText(), valueDropdownItem.GetValue());
                    default:
                        return new ValueDropdownItem((string)null, x);
                }
            }));
        }

        /// <summary>
        /// Draws the property with GUILayout support. This method is called by DrawPropertyImplementation if the GUICallType is set to GUILayout, which is the default.
        /// </summary>
        protected override void DrawPropertyLayout(GUIContent label)
        {
            if (this.Property.ValueEntry == null)
                this.CallNextDrawer(label);
            else if (this.Property.ValueEntry.WeakValues != null)
            {
                this.CallNextDrawer(label);
            }
            else if (this.error != null)
            {
                SirenixEditorGUI.ErrorMessageBox(this.error);
                this.CallNextDrawer(label);
            }
            else if (this.useSpecialListBehaviour)
            {
                CollectionDrawerStaticInfo.NextCustomAddFunction = new Action(this.OpenSelector);
                this.CallNextDrawer(label);
                if (this.result != null)
                {
                    this.AddResult(this.result);
                    this.result = (IEnumerable<object>)null;
                }

                CollectionDrawerStaticInfo.NextCustomAddFunction = (Action)null;
            }
            else
                this.DrawDropdown(label);
        }

        private void AddResult(IEnumerable<object> query)
        {
            if (!query.Any<object>())
                return;
            if (this.useSpecialListBehaviour)
            {
                ICollectionResolver childResolver = this.Property.ChildResolver as ICollectionResolver;
                foreach (object obj in query)
                {
                    object[] values = new object[this.Property.ParentValues.Count];
                    for (int index = 0; index < values.Length; ++index)
                    {
                        Type type = obj as Type;
                        if (type != (Type)null)
                            values[index] = this.CreateInstance(type);
                    }

                    childResolver.QueueAdd(values);
                }
            }
            else
            {
                Type type = query.FirstOrDefault<object>() as Type;
                for (int index = 0; index < this.Property.ValueEntry.WeakValues.Count; ++index)
                {
                    if (type != (Type)null)
                        this.Property.ValueEntry.WeakValues[index] = this.CreateInstance(type);
                }
            }
        }

        private object CreateInstance(Type type)
        {
            if (this.Property.ValueEntry.SerializationBackend == SerializationBackend.Unity)
            {
                object initializedObject = UnitySerializationUtility.CreateDefaultUnityInitializedObject(type);
                if (initializedObject != null)
                    return initializedObject;
            }

            if (type == typeof(string))
                return (object)"";
            if (type.IsAbstract || type.IsInterface)
            {
                Debug.LogError((object)("TypeFilter was asked to instantiate a value of type '" +
                                        type.GetNiceFullName() +
                                        "', but it is abstract or an interface and cannot be instantiated."));
                return (object)null;
            }

            return type.IsValueType ||
                   type.GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                       (Binder)null,
                       Type.EmptyTypes, (ParameterModifier[])null) != (ConstructorInfo)null
                ? Activator.CreateInstance(type)
                : FormatterServices.GetUninitializedObject(type);
        }

        private void DrawDropdown(GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            string currentValueName = this.GetCurrentValueName();
            IEnumerable<object> query;
            if (this.Attribute.DrawValueNormally)
            {
                query = OdinSelector<object>.DrawSelectorDropdown(label, currentValueName,
                    new Func<Rect, OdinSelector<object>>(this.ShowSelector), (GUIStyle)null);
                this.CallNextDrawer(label);
            }
            else if (this.Property.Children.Count > 0)
            {
                Rect valueRect;
                this.Property.State.Expanded =
                    SirenixEditorGUI.Foldout(this.Property.State.Expanded, label, out valueRect);
                query = OdinSelector<object>.DrawSelectorDropdown(valueRect, currentValueName,
                    new Func<Rect, OdinSelector<object>>(this.ShowSelector));
                if (SirenixEditorGUI.BeginFadeGroup((object)this, this.Property.State.Expanded))
                {
                    ++EditorGUI.indentLevel;
                    for (int index = 0; index < this.Property.Children.Count; ++index)
                    {
                        InspectorProperty child = this.Property.Children[index];
                        child.Draw(child.Label);
                    }

                    --EditorGUI.indentLevel;
                }

                SirenixEditorGUI.EndFadeGroup();
            }
            else
                query = OdinSelector<object>.DrawSelectorDropdown(label, currentValueName,
                    new Func<Rect, OdinSelector<object>>(this.ShowSelector), (GUIStyle)null);

            if (!EditorGUI.EndChangeCheck() || query == null)
                return;
            this.AddResult(query);
        }

        private void OpenSelector()
        {
            this.ReloadDropdownCollections();
            this.ShowSelector(new Rect(Event.current.mousePosition, Vector2.zero)).SelectionConfirmed +=
                (Action<IEnumerable<object>>)(x => this.result = x);
        }

        private OdinSelector<object> ShowSelector(Rect rect)
        {
            GenericSelector<object> selector = this.CreateSelector();
            rect.x = (float)(int)rect.x;
            rect.y = (float)(int)rect.y;
            rect.width = (float)(int)rect.width;
            rect.height = (float)(int)rect.height;
            selector.ShowInPopup(rect, new Vector2(0.0f, 0.0f));
            return (OdinSelector<object>)selector;
        }

        private GenericSelector<object> CreateSelector()
        {
            IEnumerable<ValueDropdownItem> source1 = this.getValues() ?? Enumerable.Empty<ValueDropdownItem>();
            bool flag = source1.Take<ValueDropdownItem>(10).Count<ValueDropdownItem>() == 10;
            GenericSelector<object> selector = new GenericSelector<object>(this.Attribute.DropdownTitle, false,
                source1.Select<ValueDropdownItem, GenericSelectorItem<object>>(
                    (Func<ValueDropdownItem, GenericSelectorItem<object>>)(x =>
                        new GenericSelectorItem<object>(x.Text, x.Value))));
            selector.CheckboxToggle = false;
            selector.EnableSingleClickToSelect();
            selector.SelectionTree.Config.DrawSearchToolbar = flag;
            IEnumerable<object> source2 = Enumerable.Empty<object>();
            if (!this.useSpecialListBehaviour)
                source2 = this.getSelection();
            IEnumerable<object> selection =
                source2.Select<object, object>((Func<object, object>)(x =>
                    x != null ? (object)x.GetType() : (object)null));
            selector.SetSelection(selection);
            selector.SelectionTree.EnumerateTree().AddThumbnailIcons(true);
            return selector;
        }

        private string GetCurrentValueName()
        {
            if (EditorGUI.showMixedValue)
                return "—";
            object key = this.Property.ValueEntry.WeakSmartValue;
            string name = (string)null;
            if (this.nameLookup != null && key != null)
                this.nameLookup.TryGetValue(key, out name);
            if (key != null)
                key = (object)key.GetType();
            return new GenericSelectorItem<object>(name, key).GetNiceName();
        }
    }
}
#endif