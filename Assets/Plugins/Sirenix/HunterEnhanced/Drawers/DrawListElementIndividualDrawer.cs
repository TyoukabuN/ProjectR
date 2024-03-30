using UnityEngine;

namespace Sirenix.OdinInspector.Editor
{
    [AllowGUIEnabledForReadonly]
    [DrawerPriority(0, 0, 1)]
    public class DrawListElementIndividualDrawer<T> : OdinAttributeDrawer<DrawListElementIndividualAttribute, T>
    {
        protected override bool CanDrawAttributeValueProperty(InspectorProperty property)
        {
            return property.ChildResolver is ICollectionResolver;
        }

        protected override void DrawPropertyLayout(GUIContent label)
        {
            foreach (var child in Property.Children)
            {
                if (child == null) continue;
                foreach (var c in child.Children)
                {
                    c.Draw();
                }
            }
        }
    }
}