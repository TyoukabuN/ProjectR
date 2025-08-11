using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using Object = UnityEngine.Object;

public class AssetMemoryAnalysis : EditorWindow
{
    [MenuItem("Tools/获取资源占用报告")]
    static void Open()
    {
        var handle = GetWindow<AssetMemoryAnalysis>();
    }

    public static string reportPathPrefabKey = "__reportPathPrefabKey";
    public static string reporttPath = string.Empty;

    public static string targetPathPrefabKey = "__targetPathPrefabKey";
    public static string targetPath = string.Empty;

    public static string outsOutputPath = "";

    public static GUIContent m_FilterByType;
    public static string[] typesDisplayNames = new string[15]
    {
        "AnimationClip",
        "AudioClip",
        "AudioMixer",
        "Font",
        "GUISkin",
        "Material",
        "Mesh",
        "Model",
        "PhysicMaterial",
        "Prefab",
        "Scene",
        "Script",
        "Shader",
        "Sprite",
        "Texture"
    };

    public static string filterStrPrefabKey = "__filterStrPrefabKey";
    public string filterStr = string.Empty;

    public static string applyFilterStrPrefabKey = "__applyFilterStrPrefabKey";
    public static bool ApplyFilter = false;

    void OnGUI()
    {
        if (checkAppRuning())
            return;
        if (m_FilterByType == null)
            m_FilterByType = new GUIContent((Texture)EditorGUIUtility.FindTexture("FilterByType"), "Search by Type");


        targetPath = EditorPrefs.GetString(targetPathPrefabKey);
        {
            EditorGUILayout.BeginHorizontal("HelpBox");
            EditorGUILayout.LabelField(targetPath, (GUIStyle)"HelpBox");
            if (GUILayout.Button("局部分析路径"))
            {
                var res = EditorUtility.OpenFolderPanel("选择局部分析路径", targetPath, targetPath);
                if (string.IsNullOrEmpty(res))
                {
                    return;
                }
                targetPath = res;
                EditorPrefs.SetString(targetPathPrefabKey, targetPath);
                Repaint();
                return;
            }

            if (GUILayout.Button("分析局部"))
            {
                if (!Directory.Exists(targetPath))
                    return;


                if (ApplyFilter)
                {
                    var tarDir = targetPath.Substring(targetPath.IndexOf("Assets/"));
                    process(FindAssets(filterStr, new string[] { tarDir }).ToArray());
                }
                else
                    process(GetAllAssetPathsByFullPath(targetPath));
            }

            EditorGUILayout.EndHorizontal();
        }



        GUILayout.FlexibleSpace();

        //result.csv
        reporttPath = EditorPrefs.GetString(reportPathPrefabKey);
        EditorGUILayout.BeginVertical("HelpBox");
        {
            EditorGUILayout.BeginHorizontal();
            {
                filterStr = EditorPrefs.GetString(filterStrPrefabKey, string.Empty);
                ApplyFilter = EditorPrefs.GetBool(applyFilterStrPrefabKey, false);
                var temp = EditorGUILayout.TextArea(filterStr);
                if (temp != filterStr)
                {
                    EditorPrefs.SetString(filterStrPrefabKey, temp);
                    filterStr = temp;
                }

                var tempb = GUILayout.Toggle(ApplyFilter, "应用过滤【对所有分析】");
                if (tempb != ApplyFilter)
                {
                    EditorPrefs.SetBool(applyFilterStrPrefabKey, tempb);
                    ApplyFilter = tempb;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(reporttPath, (GUIStyle)"HelpBox", GUILayout.ExpandWidth(true));
                if (GUILayout.Button("报告导出路径"))
                {
                    var res = EditorUtility.OpenFolderPanel("选择报告导出路径", reporttPath, reporttPath);
                    if (string.IsNullOrEmpty(res))
                    {
                        return;
                    }
                    reporttPath = res;
                    EditorPrefs.SetString(reportPathPrefabKey, reporttPath);
                    Repaint();
                    return;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("全局分析"))
        {
            if (ApplyFilter)
                process(FindAssets(filterStr).ToArray());
            else
                process(AssetDatabase.GetAllAssetPaths());
        }
    }
    static IEnumerable<string> FindAssets(string filter)
    {
        return AssetDatabase.FindAssets(filter).Select(x => AssetDatabase.GUIDToAssetPath(x));
    }
    static IEnumerable<string> FindAssets(string filter, string[] searchInFolders)
    {
        return AssetDatabase.FindAssets(filter, searchInFolders).Select(x => AssetDatabase.GUIDToAssetPath(x));
    }

    static string[] GetAllAssetPathsByFullPath(string fullPath)
    {
        List<string> paths = new List<string>();
        paths.AddRange(Directory.GetFiles(fullPath, "*.*", SearchOption.AllDirectories));

        for (int i = 0; i < paths.Count; i++)
        {
            var path = paths[i];
            var ext = Path.GetExtension(path);

            if (!ExtensionMap.Contains(ext.ToLower()))
            {
                paths.RemoveAt(i);
                i--;
                continue;
            }
            //
            var index = path.IndexOf("Assets/");
            if (index < 0)
            {
                paths.RemoveAt(i);
                i--;
                continue;
            }
            //
            paths[i] = path.Substring(index);
        }

        return paths.ToArray();
    }

    struct TextureSettings
    {
        public int maxTextureSize;
        public TextureImporterFormat textureFormat;
        public int compressionQuality;
        public bool etc1AlphaSplitEnabled;
    }
    static TextureSettings Texture_GetPlatformTextureSettings(string path)
    {
        var importer = TextureImporter.GetAtPath(path) as TextureImporter;
        var res = new TextureSettings();
        if (importer)
        {
            importer.GetPlatformTextureSettings("Android", out res.maxTextureSize, out res.textureFormat, out res.compressionQuality, out res.etc1AlphaSplitEnabled);
        }
        return res;
    }

    struct AnimationClipInfo
    {
        public float length;
        public string memory;
    }
    static AnimationClipInfo Animation_GetClipInfo(string assetPath)
    {
        var asset = AssetDatabase.LoadAssetAtPath<AnimationClip>(assetPath) as AnimationClip;
        return Animation_GetClipInfo(asset);
    }

    static AnimationClipInfo Animation_GetClipInfo(AnimationClip asset)
    {
        var res = new AnimationClipInfo();
        res.length = asset.length;
        var stats = GetAnimationClipStats.Invoke(null, new object[] { asset });
        if (stats != null)
        {
            res.memory = sizeInfo.GetValue(stats).ToString();
        }

        return res;
    }
    struct ModelInfo
    {
        public int vert;
        public int tri;
        public float memery;
    }
    static ModelInfo Model_GetFBXInfo(string assetPath)
    {
        var asset = AssetDatabase.LoadAllAssetsAtPath(assetPath);
        return Model_GetFBXInfo(asset, assetPath);
    }

    private static List<MeshFilter> meshFilters = new List<MeshFilter>();
    private static List<MeshFilter> renderers = new List<MeshFilter>();
    private static List<Mesh> hadcount = new List<Mesh>();

    static ModelInfo Model_GetFBXInfo(Object[] assets, string path = "")
    {
        var res = new ModelInfo();
        res.vert = 0;
        res.tri = 0;
        res.memery = 0;
        List<Mesh> meshFilterList = new List<Mesh>();

        if (assets == null)
            return res;

        float tri = 0;
        int vert = 0;
        foreach (var asset in assets)
        {
            if (asset is Mesh)
            {
                var mesh = asset as Mesh;
                vert += mesh.vertexCount;
                tri += mesh.triangles.Length / 3.0f;
                if (!mesh || meshFilterList.Contains(mesh))
                    continue;
                res.memery += Profiler.GetRuntimeMemorySizeLong(asset);
                meshFilterList.Add(mesh);
            }
            else if (asset is AnimationClip)
            {
                CSVLine line;
                var clip = asset as AnimationClip;
                if (Analysis_TryGetAnimationClipLine(clip, path, out line))
                {
                    line.SetPath(path + "\\" + clip.name);
                    line.ApplyLine();
                }
            }

        }
        res.vert = vert;
        res.tri = (int)tri;

        return res;
    }

    static HashSet<string> ExtensionMap = new HashSet<string>()
    {
        ".fbx",
        ".fbc",
        ".png",
        ".fbx",
        ".unity",
        ".anim"
    };

    static bool ShouldAnalysis(string key)
    {
        return ExtensionMap.Contains(key.ToLower());
    }

    struct Result
    {
        public int tri;
        public int memory;
        public bool iscancel;

    };

    static Result PathProgress(string[] allPath, string parent = "")
    {
        Result result = new Result()
        {
            tri = 0,
            memory = 0,
            iscancel = false,
        };

        float counter = 0;

        foreach (var path in allPath)
        {
            counter++;

            if (parent == path)
                continue;

            if (CSVReport.AnyCache(path))
                continue;

            var ext = Path.GetExtension(path).ToLower();
            if (!ShouldAnalysis(ext))
                continue;

            if (EditorUtility.DisplayCancelableProgressBar("Processing..", path, (float)counter / (float)allPath.Length))
            {
                result.iscancel = true;
                return result;
            }

            if (CSVReport.lineCount % 500 == 0)
                GC.Collect();

            AssetMemoryResult assetMemoryResult = new AssetMemoryResult();
            CSVLine line;
            if (ext == ".png" || ext == ".jpg" || ext == ".tga")
            {
                var asset = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
                if (!asset)
                    continue;

                line = new CSVLine(path);
                line.SetType("Texture");
                float memery = float.Parse(GetStorageMemorySizeLong.Invoke(null, new object[] { asset }).ToString()) / 1024f / 1024f;
                line.SetMemory(memery.ToString());
                line.SetSize(asset.width + "x" + asset.height);
                var res = Texture_GetPlatformTextureSettings(path);
                line.SetCompress(res.textureFormat.ToString());
                line.SetReportName(CSVReport.fileNamePre);
                line.ApplyLine(path);
                //
                assetMemoryResult.ext = ext;
                assetMemoryResult.type = "Texture";
                assetMemoryResult.memory = memery;
            }
            else if (ext == ".fbx" || ext == ".fbc")
            {
                line = new CSVLine(path);
                line.SetType("Model");

                var info = Model_GetFBXInfo(path);
                line.SetVert(info.vert.ToString());
                line.SetTriangle(info.tri.ToString());
                line.SetMemory((info.memery / 1024f / 1024f).ToString());// Profiler.GetRuntimeMemorySizeLong(info.asset).ToString());
                line.SetReportName(CSVReport.fileNamePre);
                line.ApplyLine(path);
                //
                assetMemoryResult.ext = ext;
                assetMemoryResult.type = "Model";
                assetMemoryResult.vert = info.vert;
                assetMemoryResult.triangle = info.tri;
                //
                result.tri += info.tri;
            }
            else if (ext == ".anim")
            {
                Analysis_AnimationClip(path);
            }
            else if (ext == ".unity")
            {
                line = new CSVLine(path);
                //var res = PathProgress(AssetDatabase.GetDependencies(path),path);
                line.SetType("Scence");
                //SetTriangle(res.tri.ToString());
                line.SetReportName(CSVReport.fileNamePre);
                line.ApplyLine(path);
            }

            if (!string.IsNullOrEmpty(assetMemoryResult.ext))
            {
                WriteToChache(path, assetMemoryResult);
            }
        }

        return result;
    }

    [Serializable]
    public struct AssetMemoryResult
    {
        public string ext;
        public string type;
        public float memory;
        public float size;
        public float triangle;
        public float vert;
    }

    private const string CACHE_PATH = "Library/AssetMemoryAnalysis";

    public static void WriteToChache(string path, object res)
    {
        if (string.IsNullOrEmpty(path))
            return;

        string guid = AssetDatabase.AssetPathToGUID(path);
        if (string.IsNullOrEmpty(guid))
            return;

        if (!Directory.Exists(CACHE_PATH))
            Directory.CreateDirectory(CACHE_PATH);

        string output = Path.Combine(CACHE_PATH, guid);

        //File.w(output, byte.);
        using (FileStream fs = File.OpenWrite(output))
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, res);
        }
    }

    public static object ReadFromCache(string path)
    {
        object obj = null;
        if (string.IsNullOrEmpty(path))
            return obj;

        string guid = AssetDatabase.AssetPathToGUID(path);
        if (string.IsNullOrEmpty(guid))
            return obj;


        string output = Path.Combine(CACHE_PATH, guid);

        if (!File.Exists(output))
            return obj;

        using (FileStream fs = File.OpenRead(output))
        {
            BinaryFormatter bf = new BinaryFormatter();
            obj = bf.Deserialize(fs);
        }
        return obj;
    }

    static CSVLine Analysis_GetAnimationClipLine(string path)
    {
        var line = new CSVLine(path);
        line.SetType("AnimationClip");
        var res = Animation_GetClipInfo(path);
        line.SetLength(res.length.ToString());
        line.SetMemory(res.memory.ToString());
        return line;
    }
    static HashSet<string> assetAnalysisCache = new HashSet<string>();
    static bool Analysis_TryGetAnimationClipLine(AnimationClip clip, string path, out CSVLine line)
    {
        string assetPath = AssetDatabase.GetAssetPath(clip);
        if (!string.IsNullOrEmpty(assetPath) && assetAnalysisCache.Contains(assetPath))
        {
            line = null;
            return false;
        }

        line = new CSVLine();
        if (!string.IsNullOrEmpty(path))
        {
            line.SplitPath(path);
            line.SetPath(path);
            line.SetFolderName();
            line.SetFileType();
        }
        line.SetType("AnimationClip");
        var res = Animation_GetClipInfo(clip);
        line.SetLength(res.length.ToString());
        line.SetMemory(res.memory.ToString());
        return true;
    }
    static void Analysis_AnimationClip(string path)
    {
        var line = Analysis_GetAnimationClipLine(path);
        if (line != null)
        {
            line.SetReportName(CSVReport.fileNamePre);
            line.ApplyLine();
        }
    }

    static MethodInfo GetStorageMemorySizeLong;
    static MethodInfo GetAnimationClipStats;
    static FieldInfo sizeInfo;
    static public string process(string[] allPath, string customReportPath = "", string fileNamePre = "", bool openSavePath = false)
    {
        Texture target = Selection.activeObject as Texture;
        //var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
        var type = Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        GetStorageMemorySizeLong = type.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

        GetAnimationClipStats = typeof(AnimationUtility).GetMethod("GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
        Assembly asm = Assembly.GetAssembly(typeof(Editor));
        Type aniclipstats = asm.GetType("UnityEditor.AnimationClipStats");
        sizeInfo = aniclipstats.GetField("size", BindingFlags.Public | BindingFlags.Instance);

        CSVReport.Init(string.IsNullOrEmpty(customReportPath) ? reporttPath : customReportPath, fileNamePre);

        try
        {
            var res = PathProgress(allPath);
        }
        catch (Exception e)
        {

            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }

        EditorUtility.ClearProgressBar();

        return CSVReport.Output(openSavePath);
    }

    static public string GetArtRoot()
    {
        return Path.Combine(Application.dataPath, "Art");
    }

    bool checkAppRuning()
    {
        if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode)
        {
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginHorizontal("HelpBox");

            GUILayout.Label("游戏运行中不显示", new GUIStyle() { alignment = TextAnchor.MiddleCenter }, GUILayout.ExpandWidth(true));

            EditorGUILayout.EndHorizontal();

            GUILayout.FlexibleSpace();

            return true;
        }

        return false;
    }

}
