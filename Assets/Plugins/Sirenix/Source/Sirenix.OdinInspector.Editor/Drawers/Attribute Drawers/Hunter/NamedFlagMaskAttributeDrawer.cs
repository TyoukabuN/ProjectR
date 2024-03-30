using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor.ValueResolvers;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor.Drawers
{
    public class NamedFlagMaskAttributeDrawer<T> : OdinAttributeDrawer<NamedFlagMaskAttribute, T> where T : struct
    {
        private static int _maxSize;
        private List<string> _names;
        private List<int> _selects;
        private ValueResolver<IEnumerable<string>> _namesGetter;
        
        private static List<Type> _canDrawTypes = new List<Type>
        {
            typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long),
            typeof(ulong)
        };

        public override bool CanDrawTypeFilter(Type type)
        {
            return _canDrawTypes.Contains(type);
        }
        
        static NamedFlagMaskAttributeDrawer()
        {
            _maxSize = sizeof(byte);
            var type = typeof(T);
            if (type == typeof(short) || type == typeof(ushort)) _maxSize = sizeof(short);
            else if (type == typeof(int) || type == typeof(uint)) _maxSize = sizeof(int);
            else if (type == typeof(long) || type == typeof(ulong)) _maxSize = sizeof(long);
            _maxSize *= 8;
        }

        protected override void Initialize()
        {
            _names = new List<string>();
            _selects = new List<int>();
            _namesGetter = ValueResolver.Get<IEnumerable<string>>(Property, Attribute.namesGetter);
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            _names.Clear();
            foreach (var name in _namesGetter.GetValue())
            {
                if (_names.Count >= _maxSize) break;
                if (string.IsNullOrEmpty(name)) continue;
                _names.Add(name);
            }
            
            var value = (ulong)Convert.ChangeType(ValueEntry.SmartValue, typeof(ulong));
            _selects.Clear();
            for (int i = 0; i < _names.Count; i++)
            {
                if ((value & (0x1ul << i)) != 0) _selects.Add(i);
            }

            SirenixEditorFields.Dropdown(label, _selects, _names, true);

            value = 0;
            foreach (var s in _selects)
            {
                value |= 1ul << s;
            }

            ValueEntry.SmartValue = (T)Convert.ChangeType(value, typeof(T));
        }
    }
}