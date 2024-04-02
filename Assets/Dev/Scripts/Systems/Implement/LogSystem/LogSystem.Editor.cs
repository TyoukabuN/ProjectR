#if UNITY_EDITOR
using UnityEditor;
#endif
namespace PJR
{
    public partial class LogSystem : StaticSystem
    {
#if UNITY_EDITOR
        [InitializeOnLoadMethod]
        static void RegisterEditorEvent()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }
        static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                Log("/////////////////////////////////////////////////////////");
                Log("////////////////////[EnteredPlayMode]////////////////////");
                Log("/////////////////////////////////////////////////////////");
            }
            else if (state == PlayModeStateChange.ExitingPlayMode)
            {
                Log("/////////////////////////////////////////////////////////");
                Log("////////////////////[ExitingPlayMode]////////////////////");
                Log("/////////////////////////////////////////////////////////");
            }
        }
#endif
    }
}
