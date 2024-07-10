#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using PJR;
using UnityEngine.WSA;

public static class ModelImportUtil
{
    [MenuItem("Assets/PJR/美术工具/提取动画")]
    public static void ExtractAnimation()
    {
        var obj = Selection.activeObject;
        if (GetImporter<ModelImporter>(obj, out var importer))
        {
            if (!importer.importAnimation)
            {
                importer.importAnimation = true;
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(obj), ImportAssetOptions.ForceUpdate);
            }
            AssetDatabase.StartAssetEditing();
            //
            var assetPath = AssetDatabase.GetAssetPath (obj);
            var dir = Path.GetDirectoryName(assetPath);
            var animaFolder = Path.Combine(Path.GetDirectoryName(dir), "Animations");
            if (AssetDatabase.IsValidFolder(animaFolder))
                dir = animaFolder;
            else if(!string.IsNullOrEmpty(AssetDatabase.CreateFolder(Path.GetDirectoryName(dir), "Animations")))
                dir = animaFolder;

            //抽取
            List<string> clipPaths = new List<string>();
            var clips = AssetDatabase.LoadAllAssetsAtPath(assetPath).Where(item => item is AnimationClip);
            try
            {
                foreach (var clip in clips)
                { 
                    var clipPath = CopyOrCreateClip(clip, dir);
                    if(!string.IsNullOrEmpty(clipPath))
                        clipPaths.Add(clipPath);
                }
            }
            finally { }
            importer.importAnimation = false;
            AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(obj), ImportAssetOptions.ForceUpdate);

            AssetDatabase.StopAssetEditing();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);

            //生成clipSet
            CreateClipTransitionSet(clipPaths);
        }
    }
    [MenuItem("Assets/PJR/美术工具/提取动画", true)]
    public static bool ExtractAnimationValid()
    {
        return IsModelAsset(Selection.activeObject);
    }

    [MenuItem("Assets/PJR/美术工具/创建ClipSet")] 
    public static void CreateClipTransitionSetFromClips()
    {
        string folder = string.Empty;
        string prefix = string.Empty;
        var clips = new List<AnimationClip>();
        foreach (var guid in Selection.assetGUIDs)
        {
            var assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (clip == null)
                continue;
            clips.Add(clip);
            //
            if (string.IsNullOrEmpty(prefix))
            {
                int index = clip.name.IndexOf("_");
                if (index > 0) {
                    prefix = clip.name.Substring(0, index);
                }
            }
            //
            if (string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(Path.GetDirectoryName(assetPath)))
                folder = Path.GetDirectoryName(assetPath);
        }
        Debug.Log(prefix);
        AnimatiomClipTransitionSet.CreateFromClips(clips, prefix, folder);
    }

    public static void CreateClipTransitionSet(List<string> clipPaths)
    {
        string folder = string.Empty;
        string prefix = string.Empty;
        var clips = new List<AnimationClip>();
        foreach (var assetPath in clipPaths)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath);
            if (clip == null)
                continue;
            clips.Add(clip);
            //
            if (string.IsNullOrEmpty(prefix))
            {
                int index = clip.name.IndexOf("_");
                if (index > 0)
                {
                    prefix = clip.name.Substring(0, index);
                }
            }
            //
            if (string.IsNullOrEmpty(folder) && AssetDatabase.IsValidFolder(Path.GetDirectoryName(assetPath)))
                folder = Path.GetDirectoryName(assetPath);
        }
        AnimatiomClipTransitionSet.CreateFromClips(clips, prefix, folder);
    }

    private static string CopyOrCreateClip(Object clip, string exportFolder)
    {
        if (clip.name.StartsWith("__preview__"))
            return string.Empty;

        var instance = Object.Instantiate(clip);
        AnimationClip newAnim = instance as AnimationClip;

        string clip_name = clip.name.Replace("|", "_") + ".anim"; // replace illegal character
        string exportPath = Path.Combine(exportFolder, clip_name);

        if (File.Exists(exportPath))
        {
            var actions = new List<System.Action>();
            var bindings = UnityEditor.AnimationUtility.GetCurveBindings(newAnim);
            AnimationClip existingAnim = (AnimationClip)AssetDatabase.LoadAssetAtPath(
                    exportPath, typeof(AnimationClip));

            existingAnim.ClearCurves();

            foreach (var binding in bindings)
            {
                var curve = UnityEditor.AnimationUtility.GetEditorCurve(newAnim, binding);
                UnityEditor.AnimationUtility.SetEditorCurve(existingAnim, binding, curve);
            }

            AssetDatabase.SaveAssets();
        }
        else
        {
            AssetDatabase.CreateAsset(newAnim, exportPath);
        }
        return exportPath;
    }

    public static bool GetImporter<T>(Object selectedObject,out T importer)where T : AssetImporter
    {
        importer = null;
        if (selectedObject == null)
            return false;
        string assetPath = AssetDatabase.GetAssetPath(selectedObject);
        try
        {
            importer = AssetImporter.GetAtPath(assetPath) as T;
        }
        finally { }
        return importer != null;
    }

    public static bool IsModelAsset(Object selectedObject)
    {
        if (!GetImporter<ModelImporter>(selectedObject,out var importer))
            return false;
        if (importer == null)
            return false;
        return true;
    }
}

#endif