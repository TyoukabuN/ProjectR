#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;

namespace OrdinalConfigExtensions
{
    public static class EditorWindowDockingUtility
    {
        public static void DockWindow(EditorWindow mainWindow, EditorWindow sideWindow)
        {
            const float unitWidth = 600;
            const float height = 800;

            if (mainWindow == null || sideWindow == null)
                return;
            
            DockArea mainDockArea = mainWindow.m_Parent as DockArea;
            if (mainDockArea == null)
                return;
            mainDockArea.position = new Rect(0, 0, unitWidth, height);
            
            SplitView splitView = mainWindow.m_Parent.window.rootSplitView;
            splitView.position = new Rect(100, 100, unitWidth * 2, height);
            //不用创建一个新的话屏幕上会出现一个不会消失的半透明圆圈
            DockArea newDockArea = ScriptableObject.CreateInstance<DockArea>();
            newDockArea.position = new Rect(0, 0, unitWidth, height);
            splitView.AddChild(newDockArea);
            splitView.vertical = false;
            //
            DockArea dockArea = sideWindow.m_Parent as DockArea;
            dockArea?.RemoveTab(sideWindow, true, false);
            //
            newDockArea.AddTab(sideWindow, false);
            newDockArea.SetMinMaxSizes(new Vector2(unitWidth, height),sideWindow.m_Parent.m_MaxSize);
            newDockArea.MakeVistaDWMHappyDance();
            
            mainWindow.m_Parent.window.Show(ShowMode.NormalWindow, false, true, setFocus: true);
        }

        /// <summary>   
        /// 将传入的Window side by side 水平连起来
        /// </summary>
        /// <param name="height"></param>
        /// <param name="windows"></param>
        /// <param name="totalWidth"></param>
        public static void DockWindows(float totalWidth, float height,params EditorWindow[] windows)
        {
            if (windows == null || windows.Length <= 1)
                return;
            
            var mainWindow = windows[0];
            
            float unitWidth = totalWidth / windows.Length;
            DockArea mainDockArea = mainWindow.m_Parent as DockArea;
            SplitView splitView = mainWindow.m_Parent.window.rootSplitView;
            splitView.position = new Rect(100, 100, totalWidth, height);
            
            mainDockArea.position = new Rect(0, 0, unitWidth, height);

            for (var i = 1; i < windows.Length; i++)
            {
                var subWindow = windows[i];
                DockArea newDockArea = ScriptableObject.CreateInstance<DockArea>();
                newDockArea.position = new Rect(0, 0, unitWidth, height);
                splitView.AddChild(newDockArea);
                splitView.vertical = false;
                
                DockArea dockArea = subWindow.m_Parent as DockArea;
                dockArea?.RemoveTab(subWindow, true, false);
                newDockArea.AddTab(subWindow, false);
                newDockArea.SetMinMaxSizes(new Vector2(unitWidth, height),subWindow.m_Parent.m_MaxSize);
                newDockArea.MakeVistaDWMHappyDance();
            }
            mainWindow.m_Parent.window.Show(ShowMode.NormalWindow, false, true, setFocus: true);
        }
    }
}
#endif
