#if UNITY_EDITOR
using System;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace Sirenix.OdinInspector.Editor.Drawers
{
    public class HunterDirectionPanelAttributeDrawer :OdinAttributeDrawer<HunterDirectionPanelAttribute>
    {
        private GUIContent centerLabelContent;
        private Vector2 centerLabelSize;

        private enum DirectionType
        {
            Up,
            UpperRight,
            Right,
            DownRight,
            Down,
            DownLeft,
            Left,
            UpperLeft,
            Forward,
            Backward,
        }
        protected override void Initialize()
        {
            centerLabelContent = new GUIContent($"000.00");
            centerLabelSize = SirenixGUIStyles.CenteredGreyMiniLabel.CalcSize(centerLabelContent);
            base.Initialize();
        }

        public override bool CanDrawTypeFilter(Type type)
        {
            return type == typeof(Vector3);
        }

        private bool dragging = false;
        protected override void DrawPropertyLayout(GUIContent label)
        {
            GUILayout.BeginHorizontal();
            this.CallNextDrawer(label);
            if (GUILayout.Button(EditorIcons.Pen.Active,SirenixGUIStyles.MiniIconButton))
            {
                var picker = new Picker();
                picker.window = OdinEditorWindow.InspectObjectInDropDown(picker,180f,180f);
                picker.onPicked = (dir) => this.Property.ValueEntry.WeakSmartValue = dir;
            }
            GUILayout.EndHorizontal();
            
        }

        private class Picker
        {
            [HideInInspector]
            public EditorWindow window;
            [HideInInspector]
            public Action<Vector3> onPicked;
            [OnInspectorGUI]
            public void DrawPicker()
            {
                var size = 150f;
                var rect = GUILayoutUtility.GetRect(size,size,size,size);
                
                var cellSize = size / 3f;
                rect.size = new Vector2(size, size);
                Do(rect.SplitGrid(cellSize, cellSize, 0), DirectionType.UpperLeft);
                Do(rect.SplitGrid(cellSize, cellSize, 1), DirectionType.Up);
                Do(rect.SplitGrid(cellSize, cellSize, 2), DirectionType.UpperRight);
                Do(rect.SplitGrid(cellSize, cellSize, 3), DirectionType.Left);
                var centerRect = rect.SplitGrid(cellSize, cellSize, 4);
                Do(centerRect.Split(0, 2), DirectionType.Forward);
                Do(centerRect.Split(1, 2), DirectionType.Backward);
                
                Do(rect.SplitGrid(cellSize, cellSize, 5), DirectionType.Right);
                Do(rect.SplitGrid(cellSize, cellSize, 6), DirectionType.DownLeft);
                Do(rect.SplitGrid(cellSize, cellSize, 7), DirectionType.Down);
                Do(rect.SplitGrid(cellSize, cellSize, 8), DirectionType.DownRight);
            }

            private void Do(Rect rect, DirectionType dir)
            {
                if (GUI.Button(rect, GetContent(dir)))
                {
                    onPicked?.Invoke(GetValue(dir));
                    window?.Close();
                }
            }
            

            private GUIContent GetContent(DirectionType dir)
            {
                switch (dir)
                {
                    case DirectionType.Up:
                        return GUIHelper.TempContent("↑", "上");
                        break;
                    case DirectionType.UpperRight:
                        return GUIHelper.TempContent("↗", "右上");
                        break;
                    case DirectionType.Right:
                        return GUIHelper.TempContent("→", "右");
                        break;
                    case DirectionType.DownRight:
                        return GUIHelper.TempContent("↘", "右下");
                        break;
                    case DirectionType.Down:
                        return GUIHelper.TempContent("↓", "下");
                        break;
                    case DirectionType.DownLeft:
                        return GUIHelper.TempContent("↙", "左下");
                        break;
                    case DirectionType.Left:
                        return GUIHelper.TempContent("←", "左");
                        break;
                    case DirectionType.UpperLeft:
                        return GUIHelper.TempContent("↖", "左上");
                        break;
                    case DirectionType.Forward:
                        return GUIHelper.TempContent("↺", "前");
                        break;
                    case DirectionType.Backward:
                        return GUIHelper.TempContent("↻", "后");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
                }
            }

            private Vector3 GetValue(DirectionType dir)
            {
                switch (dir)
                {
                    case DirectionType.Up:
                        return Vector3.up;
                        break;
                    case DirectionType.UpperRight:
                        return (Vector3.up + Vector3.right).normalized;
                        break;
                    case DirectionType.Right:
                        return Vector3.right;
                        break;
                    case DirectionType.DownRight:
                        return (Vector3.down + Vector3.right).normalized;
                        break;
                    case DirectionType.Down:
                        return Vector3.down;
                        break;
                    case DirectionType.DownLeft:
                        return (Vector3.down + Vector3.left).normalized;
                        break;
                    case DirectionType.Left:
                        return Vector3.left;
                        break;
                    case DirectionType.UpperLeft:
                        return (Vector3.up + Vector3.left).normalized;
                        break;
                    case DirectionType.Forward:
                        return Vector3.forward;
                        break;
                    case DirectionType.Backward:
                        return Vector3.back;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
                }
            }
        }
    }
}
#endif