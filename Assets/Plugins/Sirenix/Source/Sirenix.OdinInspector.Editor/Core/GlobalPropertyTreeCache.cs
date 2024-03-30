using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor
{
    public static class GlobalPropertyTreeCache
    {
        private static Dictionary<int, PropertyTree<OdinInpectorWrapper>> _instanceIdToPropertyTrees = new Dictionary<int, PropertyTree<OdinInpectorWrapper>>();
        private static Dictionary<object, PropertyTree<OdinInpectorWrapper>> _refrenceToPropertyTrees = new Dictionary<object, PropertyTree<OdinInpectorWrapper>>();

        public static PropertyTree<OdinInpectorWrapper> GetPropertyTree(object obj)
        {
            if (obj is UnityEngine.Object uo)
            {
                var id = uo.GetInstanceID();
                if (!_instanceIdToPropertyTrees.ContainsKey(id)) return null;
                return _instanceIdToPropertyTrees[id];
            }
            else
            {
                if (!_refrenceToPropertyTrees.ContainsKey(obj)) return null;
                return _refrenceToPropertyTrees[obj];
            }
        }

        public static PropertyTree<OdinInpectorWrapper> GetOrCreatePropertyTree(object obj)
        {
            var pt = GetPropertyTree(obj);
            if (pt != null) return pt;
            
            pt = (PropertyTree<OdinInpectorWrapper>) PropertyTree.Create(new OdinInpectorWrapper{value = obj}/*, new TestOdinAttributeProcessorLocator()*/);
            if (pt != null)
            {
                if (obj is UnityEngine.Object uo)
                {
                    _instanceIdToPropertyTrees.Add(uo.GetInstanceID(), pt);
                }
                else
                {
                    _refrenceToPropertyTrees.Add(obj, pt);
                }
            }
            return pt;
        }
        
        [HideReferenceObjectPicker]
        [ShowOdinSerializedPropertiesInInspector]
        public class OdinInpectorWrapper
        {
            [HideReferenceObjectPicker]
            [HideLabel]
            public object value;
        }
        
        /*public class TestOdinAttributeProcessorLocator : OdinAttributeProcessorLocator
        {
            public override List<OdinAttributeProcessor> GetChildProcessors(InspectorProperty parentProperty, MemberInfo member)
            {
                throw new System.NotImplementedException();
            }

            public override List<OdinAttributeProcessor> GetSelfProcessors(InspectorProperty property)
            {
                throw new System.NotImplementedException();
            }
        }*/
    }
}