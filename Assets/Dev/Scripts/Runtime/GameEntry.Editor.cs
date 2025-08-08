using PJR;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class GameEntryEditor
{
    [InitializeOnLoadMethod]
    static void EditorEventRegister()
    {
        DebugMenu.OnCreateLanuchSceneAndHierarchy -= OnCreateLanuchSceneAndHierarchy;
        DebugMenu.OnCreateLanuchSceneAndHierarchy += OnCreateLanuchSceneAndHierarchy;
    }

    static void OnCreateLanuchSceneAndHierarchy()
    {
        var gEntry = new GameObject("GameEntry");
        var entry = gEntry.AddComponent<GameEntry>();
        GameObject.DontDestroyOnLoad(gEntry);
    }
}
#endif
