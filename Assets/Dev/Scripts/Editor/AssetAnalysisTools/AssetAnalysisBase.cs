using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class AssetAnalysisBase : EditorWindow
{
    public string reportPathPrefabKey
    {
        get { return "__reportPathPrefabKey_" + this.GetType().Name; }
    }
    public static string reporttPath = string.Empty;
    public string targetPathPrefabKey
    {
        get { return "__targetPathPrefabKey_" + this.GetType().Name; }
    }
    public static string targetPath = string.Empty;

    public string filterStrPrefabKey
    {
        get { return "__filterStrPrefabKey_" + this.GetType().Name; }
    }
    public string filterStr = string.Empty;

    public string applyFilterStrPrefabKey
    {
        get { return "__applyFilterStrPrefabKey_" + this.GetType().Name; }
    }
    public static bool ApplyFilter = false;

    public string displayProgressBarStrPrefabKey
    {
        get { return "__displayProgressBarStrPrefabKey_" + this.GetType().Name; }
    }
    public static bool displayProgressBar = true;

    public string sceneCheckModeStrPrefabKey
    {
        get { return "__sceneCheckModeStrPrefabKey_" + this.GetType().Name; }
    }
    public static bool sceneCheckMode = false;

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

    protected bool checkAppRuning()
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

    public static string outsOutputPath = "";

    public static GUIContent m_FilterByType;

    protected virtual void OnGUI()
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
    protected static IEnumerable<string> FindAssets(string filter)
    {
        return AssetDatabase.FindAssets(filter).Select(x => AssetDatabase.GUIDToAssetPath(x));
    }
    protected static IEnumerable<string> FindAssets(string filter, string[] searchInFolders)
    {
        return AssetDatabase.FindAssets(filter, searchInFolders).Select(x => AssetDatabase.GUIDToAssetPath(x));
    }

    protected virtual HashSet<string> ExtensionMap
    {
        get; set;
    }

    protected string[] GetAllAssetPathsByFullPath(string fullPath)
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


    protected static HashSet<string> assetAnalysisCache = new HashSet<string>();

    protected static MethodInfo GetStorageMemorySizeLong;
    protected static MethodInfo GetAnimationClipStats;
    protected static FieldInfo sizeInfo;
    protected virtual void process(string[] allPath)
    {
        Texture target = Selection.activeObject as Texture;
        //var type = Types.GetType("UnityEditor.TextureUtil", "UnityEditor.dll");
        var type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
        GetStorageMemorySizeLong = type.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

        GetAnimationClipStats = typeof(AnimationUtility).GetMethod("GetAnimationClipStats", BindingFlags.Static | BindingFlags.NonPublic);
        Assembly asm = Assembly.GetAssembly(typeof(Editor));
        Type aniclipstats = asm.GetType("UnityEditor.AnimationClipStats");
        sizeInfo = aniclipstats.GetField("size", BindingFlags.Public | BindingFlags.Instance);

        CSVReport.Init(reporttPath);

        try
        {
            var res = PathProgress(allPath);
        }
        catch (Exception e)
        {

            Debug.LogError(e);
            EditorUtility.ClearProgressBar();
        }

        if (!sceneCheckMode)
        {
            CSVReport.Output();
        }

        EditorUtility.ClearProgressBar();
    }

    protected struct Result
    {
        public int tri;
        public int memory;
        public bool iscancel;

    };

    protected virtual Result PathProgress(string[] allPath, string parent = "")
    {
        Result result = new Result()
        {
            tri = 0,
            memory = 0,
            iscancel = false,
        };
        return result;
    }


    public class AnimBoolHandle : AnimBool
    {
        protected string m_RecordKey = string.Empty;
        public string RecordKey
        {
            get { return m_RecordKey; }
            protected set { m_RecordKey = value; }
        }
        public AnimBoolHandle(string recordKey, bool value) : base(value)
        {
            value = EditorPrefs.GetBool(recordKey, value);
            target = value;
            m_RecordKey = recordKey;
        }
        public new bool target
        {
            get { return base.target; }
            set
            {
                if (value != target)
                {
                    EditorPrefs.SetBool(RecordKey, value);
                }
                base.target = value;
            }
        }
        public bool GetPref()
        {
            return EditorPrefs.GetBool(RecordKey, false);
        }
    }

    public class ValueHandle
    {
        protected string m_RecordKey = string.Empty;
        protected bool m_isChanged = false;
        public bool IsChanged
        {
            get
            {
                return m_isChanged;
            }
        }

        public string RecordKey
        {
            get { return m_RecordKey + m_AdditionRecordKey; }
            protected set { m_RecordKey = value; }
        }

        protected string m_AdditionRecordKey = string.Empty;
        public string AdditionRecordKey
        {
            get { return m_AdditionRecordKey; }
            set { m_AdditionRecordKey = value; }
        }
        public ValueHandle()
        {
        }
        public ValueHandle(string recordKey)
        {
            m_RecordKey = recordKey;
            Update();
        }
        public virtual void Save() { }
        public virtual void Update() { }

        public static void Init()
        {
            cacheBook = new Dictionary<string, XStreamingEditorCache>();
            cacheQueue = new List<XStreamingEditorCache>();
            dirtyCacheList = new Dictionary<XStreamingEditorCache, System.Diagnostics.Stopwatch>();
        }

        public static Dictionary<string, XStreamingEditorCache> cacheBook = new Dictionary<string, XStreamingEditorCache>();
        public static List<XStreamingEditorCache> cacheQueue = new List<XStreamingEditorCache>();
        public static XStreamingEditorCache currentEditorCache
        {
            get
            {
                if (cacheQueue == null || cacheQueue.Count <= 0)
                    return null;
                return cacheQueue[cacheQueue.Count - 1];
            }
        }
        public static void BeginCacheValue(string cacheFilePath)
        {
            XStreamingEditorCache cache = null;
            if (!cacheBook.TryGetValue(cacheFilePath, out cache) || cache == null)
            {
                cache = AssetDatabase.LoadAssetAtPath(cacheFilePath, typeof(XStreamingEditorCache)) as XStreamingEditorCache;
                if (cache == null)
                {
                    cache = ScriptableObject.CreateInstance<XStreamingEditorCache>();
                    var dir = Path.GetDirectoryName(cacheFilePath);
                    if (!Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                    AssetDatabase.CreateAsset(cache, cacheFilePath);
                    AssetDatabase.Refresh();
                    cache = AssetDatabase.LoadAssetAtPath(cacheFilePath, typeof(XStreamingEditorCache)) as XStreamingEditorCache;
                    SetCachaDirty(cache);
                }
                cacheBook[cacheFilePath] = cache;
            }
            if (cacheQueue.Count <= 0 || cacheQueue[cacheQueue.Count - 1] != cache)
                cacheQueue.Add(cache);
        }
        public static Dictionary<XStreamingEditorCache, System.Diagnostics.Stopwatch> dirtyCacheList = new Dictionary<XStreamingEditorCache, System.Diagnostics.Stopwatch>();
        public static void SetCachaDirty(XStreamingEditorCache cache)
        {
            if (dirtyCacheList.ContainsKey(cache))
            {
                dirtyCacheList[cache].Restart();
                return;
            }
            dirtyCacheList[cache] = System.Diagnostics.Stopwatch.StartNew();
        }
        public static bool IsCacheDirty(XStreamingEditorCache cache)
        {
            return dirtyCacheList.ContainsKey(cache);
        }
        public static void SetCacheClean(XStreamingEditorCache cache)
        {
            if (!dirtyCacheList.ContainsKey(cache))
                return;
            dirtyCacheList.Remove(cache);
        }
        public static void SaveDirtyCaches()
        {
            if (dirtyCacheList == null || dirtyCacheList.Count <= 0)
                return;

            foreach (var pair in dirtyCacheList)
            {
                EditorUtility.SetDirty(pair.Key);
            }

            dirtyCacheList.Clear();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 延时保存tick
        /// </summary>
        public static void SaveDirtyCachesTick()
        {
            if (dirtyCacheList == null)
                return;

            XStreamingEditorCache cache = null;
            foreach (var pair in dirtyCacheList)
            {
                if (pair.Value.ElapsedMilliseconds * 0.001f > 0.333f)
                {
                    cache = pair.Key;
                    EditorUtility.SetDirty(pair.Key);
                    break;
                }
            }
            if (cache)
            {
                dirtyCacheList.Remove(cache);
                AssetDatabase.SaveAssets();
            }
        }
        public static bool EndCacheValue()
        {
            if (cacheQueue.Count <= 0)
                return false;

            XStreamingEditorCache cache = cacheQueue[cacheQueue.Count - 1];
            if (cache == null)
            {
                cacheQueue.RemoveAt(cacheQueue.Count - 1);
                return false;
            }
            //
            if (cache.isChanged)
            {
                cache.isChanged = false;
                SetCachaDirty(cache);
            }
            //else if (IsCacheDirty(cache))//delay save
            //{
            //    EditorUtility.SetDirty(cache);
            //    AssetDatabase.SaveAssets();
            //}

            cacheQueue.RemoveAt(cacheQueue.Count - 1);
            return true;
        }

        public string GetString(string key, string defaultValue)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                return cache.GetString(key, defaultValue);

            return EditorPrefs.GetString(key, defaultValue);
        }
        public void SetString(string key, string value)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                cache.SetString(key, value);
            else
                EditorPrefs.SetString(key, value);

            m_isChanged = true;
        }
        public int GetInt(string key, int defaultValue)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                return cache.GetInt(key, defaultValue);

            return EditorPrefs.GetInt(key, defaultValue);
        }
        public void SetInt(string key, int value)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                cache.SetInt(key, value);
            else
                EditorPrefs.SetInt(key, value);

            m_isChanged = true;
        }
        public float GetFloat(string key, float defaultValue)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                return cache.GetFloat(key, defaultValue);

            return EditorPrefs.GetFloat(key, defaultValue);
        }
        public void SetFloat(string key, float value)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                cache.SetFloat(key, value);
            else
                EditorPrefs.SetFloat(key, value);

            m_isChanged = true;
        }
        public bool GetBool(string key, bool defaultValue)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                return cache.GetBool(key, defaultValue);

            return EditorPrefs.GetBool(key, defaultValue);
        }
        public void SetBool(string key, bool value)
        {
            XStreamingEditorCache cache = currentEditorCache;
            if (cache != null)
                cache.SetBool(key, value);
            else
                EditorPrefs.SetBool(key, value);

            m_isChanged = true;
        }
    }
    public class IntHandle : ValueHandle
    {
        protected int m_value;
        protected int m_DefaultValue;

        public virtual int value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    SetInt(RecordKey, value);
                    this.m_value = value;
                }
            }
        }
        public IntHandle()
        {
        }
        public IntHandle(string recordKey, int defaultValue, string additionKey = "")
        {
            this.m_DefaultValue = defaultValue;
            this.m_RecordKey = recordKey;
            this.m_AdditionRecordKey = additionKey;
            Update();
        }
        
        public T ToEnum<T>() where T : System.Enum
        {
            return (T)(object)value;
        }
        public override void Save()
        {
            SetInt(RecordKey, m_value);
        }
        public override void Update()
        {
            m_value = GetInt(RecordKey, m_DefaultValue);
        }
        public static implicit operator int(IntHandle handle)
        {
            return handle.value;
        }
    }


    public class FloatHandle : ValueHandle
    {
        protected float m_value;
        protected float m_DefaultValue;

        public virtual float value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    SetFloat(RecordKey, value);
                    this.m_value = value;
                }
            }
        }
        public FloatHandle()
        {
        }
        public FloatHandle(string recordKey, float defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Save()
        {
            SetFloat(RecordKey, m_value);
        }
        public override void Update()
        {
            m_value = GetFloat(RecordKey, m_DefaultValue);
        }
        public static implicit operator float(FloatHandle handle)
        {
            return handle.value;
        }
    }
    public class BoolHandle : ValueHandle
    {
        protected bool m_value;
        protected bool m_DefaultValue;

        public bool value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                m_isChanged = false;

                if (this.m_value != value)
                {
                    SetBool(RecordKey, value);
                    this.m_value = value;
                    m_isChanged = true;
                }
            }
        }
        public BoolHandle()
        {
        }
        public BoolHandle(string recordKey, bool defaultValue, string additionKey = "")
        {
            m_RecordKey = recordKey;
            this.m_DefaultValue = defaultValue;
            this.m_AdditionRecordKey = additionKey;
            Update();
        }
        public override void Save()
        {
            SetBool(RecordKey, m_value);
        }
        public override void Update()
        {
            m_value = GetBool(RecordKey, m_DefaultValue);
        }
        public static implicit operator bool(BoolHandle handle)
        {
            return handle.value;
        }
    }
    public class StringHandle : ValueHandle
    {
        protected string m_value;
        protected string m_DefaultValue;

        public string value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    m_isChanged = true;
                    SetString(RecordKey, value);
                    this.m_value = value;
                }
            }
        }
        public StringHandle()
        {
        }
        public StringHandle(string recordKey, string defaultValue, string additionKey = "")
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            m_AdditionRecordKey = additionKey;
            Update();
        }
        public override void Save()
        {
            SetString(RecordKey, m_value);
        }
        public override void Update()
        {
            m_value = GetString(RecordKey, m_DefaultValue);
        }
        public static implicit operator string(StringHandle handle)
        {
            return handle.value;
        }
    }
    public class Vector4Handle : ValueHandle
    {
        protected Vector4 m_value;
        protected Vector4 m_DefaultValue;

        public Vector4 value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public Vector4Handle()
        {
        }
        public Vector4Handle(string recordKey, Vector4 defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Save()
        {
            SetFloat(RecordKey + "_x", value.x);
            SetFloat(RecordKey + "_y", value.y);
            SetFloat(RecordKey + "_z", value.z);
            SetFloat(RecordKey + "_w", value.w);
        }
        public override void Update()
        {
            float x = GetFloat(RecordKey + "_x", m_DefaultValue.x);
            float y = GetFloat(RecordKey + "_y", m_DefaultValue.y);
            float z = GetFloat(RecordKey + "_z", m_DefaultValue.z);
            float w = GetFloat(RecordKey + "_w", m_DefaultValue.w);
            m_value = new Vector4(x, y, z, w);
        }
        public static implicit operator Vector4(Vector4Handle handle)
        {
            return handle.value;
        }
    }
    public class Vector3Handle : ValueHandle
    {
        protected Vector3 m_value;
        protected Vector3 m_DefaultValue;

        public Vector3 value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public Vector3Handle()
        {
        }
        public Vector3Handle(string recordKey, Vector3 defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Save()
        {
            SetFloat(RecordKey + "_x", value.x);
            SetFloat(RecordKey + "_y", value.y);
            SetFloat(RecordKey + "_z", value.z);
        }
        public override void Update()
        {
            float x = GetFloat(RecordKey + "_x", m_DefaultValue.x);
            float y = GetFloat(RecordKey + "_y", m_DefaultValue.y);
            float z = GetFloat(RecordKey + "_z", m_DefaultValue.z);
            m_value = new Vector3(x, y, z);
        }
        public static implicit operator Vector3(Vector3Handle handle)
        {
            return handle.value;
        }
    }
    public class Vector2Handle : ValueHandle
    {
        protected Vector2 m_value;
        protected Vector2 m_DefaultValue;

        public Vector2 value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public Vector2Handle()
        {
        }
        public Vector2Handle(string recordKey, Vector2 defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Update()
        {
            float x = GetFloat(RecordKey + "_x", m_DefaultValue.x);
            float y = GetFloat(RecordKey + "_y", m_DefaultValue.y);
            m_value = new Vector2(x, y);
        }
        public override void Save()
        {
            SetFloat(RecordKey + "_x", value.x);
            SetFloat(RecordKey + "_y", value.y);
        }
        public static implicit operator Vector2(Vector2Handle handle)
        {
            return handle.value;
        }
    }
    public class Vector2IntHandle : ValueHandle
    {
        protected Vector2Int m_value;
        protected Vector2Int m_DefaultValue;

        public Vector2Int value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public Vector2IntHandle()
        {
        }
        public Vector2IntHandle(string recordKey, Vector2Int defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Update()
        {
            int x = GetInt(RecordKey + "_x", m_DefaultValue.x);
            int y = GetInt(RecordKey + "_y", m_DefaultValue.y);
            m_value = new Vector2Int(x, y);
        }
        public override void Save()
        {
            SetFloat(RecordKey + "_x", value.x);
            SetFloat(RecordKey + "_y", value.y);
        }
        public static implicit operator Vector2Int(Vector2IntHandle handle)
        {
            return handle.value;
        }
    }

    public class ColorHandle : ValueHandle
    {
        protected Color m_value;
        protected Color m_DefaultValue;

        public Color value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public ColorHandle()
        {
        }
        public ColorHandle(string recordKey, Color defaultValue)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Update();
        }
        public override void Save()
        {
            SetString(RecordKey, ColorToUint(value).ToString());
        }
        public override void Update()
        {
            string str = GetString(RecordKey, ColorToUint(m_DefaultValue).ToString());
            m_value = UintToColor(uint.Parse(str));
        }
        public static implicit operator Color(ColorHandle handle)
        {
            return handle.value;
        }
    }
    public class ObjectHandle<T> : ValueHandle where T : UnityEngine.Object
    {
        protected T m_value;
        protected T m_DefaultValue = null;
        public string assetPath = string.Empty;
        public BoolHandle Enabled;
        public T value
        {
            get
            {
                return this.m_value;
            }
            set
            {
                if (this.m_value != value)
                {
                    this.m_value = value;
                    Save();
                }
            }
        }
        public ObjectHandle()
        {
        }
        public ObjectHandle(string recordKey, T defaultValue, bool enable = true)
        {
            this.m_DefaultValue = defaultValue;
            m_RecordKey = recordKey;
            Enabled = new BoolHandle(recordKey + "Enabled", enable);
            Update();
        }
        public override void Save()
        {
            string path = AssetDatabase.GetAssetPath(value);
            SetString(RecordKey, path);
            assetPath = path;
        }
        public override void Update()
        {
            var path = GetString(RecordKey, string.Empty);
            if (string.IsNullOrEmpty(path))
            {
                m_value = null;
                return;
            }
            assetPath = path;
            m_value = AssetDatabase.LoadAssetAtPath<T>(path) as T;
        }
    }
    public class ObjectHandleList
    {
        public List<ObjectHandle<UnityEngine.Object>> Infos;
        public IntHandle InfosCount;
        public string label = string.Empty;
        public string editorPrefKey = string.Empty;
        public BoolHandle Enabled;
        public ObjectHandleList(string label, string editorPrefKey)
        {
            this.label = editorPrefKey;
            this.editorPrefKey = editorPrefKey;
            Enabled = new BoolHandle(editorPrefKey + "Enabled", true);
        }
        public void Init()
        {
            var handle = this;
            if (handle.InfosCount == null)
                handle.InfosCount = new IntHandle(handle.editorPrefKey + "InfosCount", 0);
            if (handle.Infos == null)
            {
                handle.Infos = new List<ObjectHandle<UnityEngine.Object>>();
                for (int i = 0; i < handle.InfosCount.value; i++)
                    handle.Infos.Add(new ObjectHandle<UnityEngine.Object>(handle.editorPrefKey + i.ToString(), null));
            }
        }
        public void Save()
        {
            if (Infos == null)
                return;
            for (int i = 0; i < Infos.Count; i++)
            {
                var handle = Infos[i];
                handle.AdditionRecordKey = i.ToString();
                handle.Save();
            }
        }
        public void OnGUI()
        {
            var handle = this;
            Init();

            EditorGUILayout.BeginHorizontal();
            {
                Enabled.value = EditorGUILayout.Toggle(Enabled.value, GUILayout.Width(12));
                if (Enabled.IsChanged)
                {
                    foreach (var info in Infos)
                        if (info != null)
                            info.Enabled.value = Enabled.value;
                }

                EditorGUILayout.LabelField(this.label);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    Infos.Add(new ObjectHandle<UnityEngine.Object>(handle.editorPrefKey + Infos.Count.ToString(), null));
                    InfosCount.value = Infos.Count;
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < InfosCount.value; i++)
            {
                if (i >= Infos.Count)
                    break;

                if (Infos[i] == null)
                    Infos[i] = new ObjectHandle<UnityEngine.Object>(handle.editorPrefKey + Infos.Count.ToString(), null);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        Infos[i].Enabled.value = EditorGUILayout.Toggle(Infos[i].Enabled.value, GUILayout.Width(12));
                        EditorGUILayout.LabelField(string.Format("[{0}]", i + 1), GUILayout.Width(30));
                        Infos[i].value = EditorGUILayout.ObjectField(Infos[i].value, typeof(UnityEngine.Object), false) as UnityEngine.Object;
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        Infos.RemoveAt(i);
                        InfosCount.value = Infos.Count;
                        Save();
                        EditorGUILayout.EndHorizontal();
                        GUIUtility.ExitGUI();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
    }
    public class StringHandleList
    {
        public List<StringHandle> Infos;
        public IntHandle InfosCount;
        public string label = string.Empty;
        public string m_editorPrefKey = string.Empty;
        public BoolHandle Enabled;
        public string editorPrefKey
        {
            get { return m_editorPrefKey + m_AdditionRecordKey; }
            protected set { m_editorPrefKey = value; }
        }

        protected string m_AdditionRecordKey = string.Empty;
        public string AdditionRecordKey
        {
            get { return m_AdditionRecordKey; }
            set
            {
                if (value == m_AdditionRecordKey)
                    return;
                m_AdditionRecordKey = value;
                Init(true);
            }
        }
        public StringHandleList(string label, string editorPrefKey, string additionKey = "")
        {
            this.label = label;
            this.editorPrefKey = editorPrefKey;
            this.m_AdditionRecordKey = additionKey;
            Enabled = new BoolHandle(editorPrefKey + "Enabled", true);
            InfosCount = new IntHandle(editorPrefKey + "InfosCount", 0);
        }
        public void Init(bool clear = false)
        {
            var handle = this;
            if (clear)
            {
                handle.InfosCount = null;
                handle.Infos = null;
                Enabled = new BoolHandle(editorPrefKey + "Enabled", true);
            }
            if (handle.InfosCount == null)
                handle.InfosCount = new IntHandle(editorPrefKey + "InfosCount", 0);
            if (handle.Infos == null)
            {
                handle.Infos = new List<StringHandle>();
                for (int i = 0; i < handle.InfosCount.value; i++)
                {
                    var value = new StringHandle(editorPrefKey + i.ToString(), string.Empty);
                    if (!string.IsNullOrEmpty(value.value))
                        handle.Infos.Add(value);
                }
                handle.InfosCount.value = handle.Infos.Count;
            }
        }
        public void AddInfoValue(string value, bool checkContain = true)
        {
            if (Infos == null)
                return;
            if (checkContain && Infos.Any(item => item.value.Equals(value)))
                return;
            var stringHandle = new StringHandle(editorPrefKey + Infos.Count().ToString(), value);
            stringHandle.value = value;
            Infos.Add(stringHandle);
            InfosCount.value = Infos.Count;
            Save();
        }
        public void SetInfoValue(int index, string value)
        {
            if (Infos == null)
                return;
            if (index >= Infos.Count)
                return;
            Infos[index].value = value;
        }
        public void Save()
        {
            if (Infos == null)
                return;
            for (int i = 0;i< Infos.Count; i++)
            {
                var handle = Infos[i];
                handle.AdditionRecordKey = i.ToString();
                handle.Save();
            }
        }
        public void OnGUI()
        {
            var handle = this;
            Init();

            EditorGUILayout.BeginHorizontal();
            {
                Enabled.value = EditorGUILayout.Toggle(Enabled.value, GUILayout.Width(12));

                EditorGUILayout.LabelField(this.label);
                if (GUILayout.Button("+", GUILayout.Width(20)))
                {
                    handle.Infos.Add(new StringHandle(handle.editorPrefKey + Infos.Count.ToString(), string.Empty));
                    InfosCount.value = Infos.Count;
                }
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < InfosCount.value; i++)
            {
                if (i >= Infos.Count)
                    break;

                if (Infos[i] == null)
                    Infos[i] = new StringHandle(handle.editorPrefKey + Infos.Count.ToString(), string.Empty);

                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField(string.Format("[{0}]", i + 1), GUILayout.Width(30));
                        Infos[i].value = EditorGUILayout.TextField(Infos[i].value);
                        EditorGUILayout.EndHorizontal();
                    }
                    if (GUILayout.Button("-", GUILayout.Width(20)))
                    {
                        Infos.RemoveAt(i);
                        InfosCount.value = Infos.Count;
                        Save();
                        EditorGUILayout.EndHorizontal();
                        GUIUtility.ExitGUI();
                        return;
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
        }
        public List<string> ToList()
        {
            List<string> temp = new List<string>();
            foreach (var info in Infos)
            {
                temp.Add(info.value);
            }
            return temp;
        }
        public string[] ToArray()
        {
            List<string> temp = new List<string>();
            foreach (var info in Infos)
            {
                temp.Add(info.value);
            }
            return temp.ToArray();
        }
    }
    public static Color UintToColor(uint value)
    {
        Color color = new Color();
        color.a = (float)((value & 0xff000000) >> 24) / 0xff;
        color.r = (float)((value & 0xff0000) >> 16) / 0xff;
        color.g = (float)((value & 0xff00) >> 8) / 0xff;
        color.b = (float)((value & 0xff)) / 0xff;
        return color;
    }

    public static uint ColorToUint(Color color)
    {
        var a = (uint)(color.a * 0xff);
        var r = (uint)(color.r * 0xff);
        var g = (uint)(color.g * 0xff);
        var b = (uint)(color.b * 0xff);
        return (a << 24) | (r << 16) | (g << 8) | b;
    }

    public enum LabelIcon
    {
        Gray = 0,
        Blue,
        Teal,
        Green,
        Yellow,
        Orange,
        Red,
        Purple
    }

    public enum Icon
    {
        CircleGray = 0,
        CircleBlue,
        CircleTeal,
        CircleGreen,
        CircleYellow,
        CircleOrange,
        CircleRed,
        CirclePurple,
        DiamondGray,
        DiamondBlue,
        DiamondTeal,
        DiamondGreen,
        DiamondYellow,
        DiamondOrange,
        DiamondRed,
        DiamondPurple
    }

    protected static GUIContent[] labelIcons;
    protected static GUIContent[] largeIcons;

    public static void SetIcon(GameObject gObj, LabelIcon icon)
    {
        if (labelIcons == null)
        {
            labelIcons = GetTextures("sv_label_", string.Empty, 0, 8);
        }

        SetIcon(gObj, labelIcons[(int)icon].image as Texture2D);
    }

    public static void SetIcon(GameObject gObj, Icon icon)
    {
        if (largeIcons == null)
        {
            largeIcons = GetTextures("sv_icon_dot", "_pix16_gizmo", 0, 16);
        }

        SetIcon(gObj, largeIcons[(int)icon].image as Texture2D);
    }

    protected static void SetIcon(GameObject gObj, Texture2D texture)
    {
#if UNITY_2021_3_OR_NEWER
        EditorGUIUtility.SetIconForObject(gObj, texture);
#else
        var ty = typeof(EditorGUIUtility);
        var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        mi.Invoke(null, new object[] { gObj, texture });
#endif
    }

    public static void CleanIcon(GameObject gObj)
    {
#if UNITY_2021_3_OR_NEWER
        EditorGUIUtility.SetIconForObject(gObj, null);
#else
        var ty = typeof(EditorGUIUtility);
        var mi = ty.GetMethod("SetIconForObject", BindingFlags.NonPublic | BindingFlags.Static);
        mi.Invoke(null, new object[] { gObj, null });
#endif
    }

    protected static GUIContent[] GetTextures(string baseName, string postFix, int startIndex, int count)
    {
        GUIContent[] guiContentArray = new GUIContent[count];

        for (int index = 0; index < count; ++index)
        {
            guiContentArray[index] = EditorGUIUtility.IconContent(baseName + (object)(startIndex + index) + postFix);
        }

        return guiContentArray;
    }
    protected static bool Foldout(bool display, string title, float woffset = 0, float hoffset = 0)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);

        var rect = GUILayoutUtility.GetRect(16f + woffset, 22f + hoffset, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, display, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            display = !display;
            e.Use();
        }

        return display;
    }

    protected static bool Foldout(AnimBoolHandle animBoolHandle, string title, float woffset = 0, float hoffset = 0)
    {
        var style = new GUIStyle("ShurikenModuleTitle");
        style.font = new GUIStyle(EditorStyles.boldLabel).font;
        style.border = new RectOffset(15, 7, 4, 4);
        style.fixedHeight = 22;
        style.contentOffset = new Vector2(20f, -2f);

        var rect = GUILayoutUtility.GetRect(16f + woffset, 22f + hoffset, style);
        GUI.Box(rect, title, style);

        var e = Event.current;

        var toggleRect = new Rect(rect.x + 4f, rect.y + 2f, 13f, 13f);
        if (e.type == EventType.Repaint)
        {
            EditorStyles.foldout.Draw(toggleRect, false, false, animBoolHandle.target, false);
        }

        if (e.type == EventType.MouseDown && rect.Contains(e.mousePosition))
        {
            animBoolHandle.target = !animBoolHandle.target;
            e.Use();
            GUIUtility.ExitGUI();
            return true;
        }

        return false;
    }

    public const string TYPE_TEXTURE = "Texture";
    public const string TYPE_MODEL = "Model";
    public const string TYPE_MATERIAL = "Material";
    public const string TYPE_ANIMATION_CLIP = "AnimationCilp";
    //lower case
    public static Dictionary<string, string> extension2filetypeConfig = new Dictionary<string, string>() {
        {".png", TYPE_TEXTURE},
        {".jpg", TYPE_TEXTURE},
        {".tga", TYPE_TEXTURE},
        {".fbx", TYPE_MODEL},
        {".fbc", TYPE_MODEL},
        {".anim", TYPE_ANIMATION_CLIP},
        {".mat", TYPE_MATERIAL},
    };
    public static string FilePath2FileType(string path)
    {
        return Extension2FileType(Path.GetExtension(path).ToLower());
    }
    public static string Extension2FileType(string ext)
    {
        string type = string.Empty;
        extension2filetypeConfig.TryGetValue(ext, out type);
        return type;
    }

    //获取对象所占内存KB
    protected static float GetObjectMemorySize(UnityEngine.Object obj, Dictionary<string, float> cache = null, bool ignoreExist = false)
    {
        if (GetStorageMemorySizeLong == null)
        {
            var _type = System.Reflection.Assembly.Load("UnityEditor.dll").GetType("UnityEditor.TextureUtil");
            GetStorageMemorySizeLong = _type.GetMethod("GetStorageMemorySizeLong", BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
        }


        float memery = 0;
        float SubMeshCount = 0;
        string path = AssetDatabase.GetAssetPath(obj);
        //if (string.IsNullOrEmpty(path)) continue;
        string type = Extension2FileType(Path.GetExtension(path).ToLower());
        //if (string.IsNullOrEmpty(type)) continue;

        if (type == TYPE_TEXTURE)
        {
            if (cache != null && cache.TryGetValue(path, out memery))
                return ignoreExist ? 0 : memery;

            memery = float.Parse(GetStorageMemorySizeLong.Invoke(null, new object[] { obj }).ToString()) / 1024f;

            if (cache != null && !cache.ContainsKey(path))
                cache[path] = memery;
        }
        else if (type == TYPE_MATERIAL)
        {
            var mat = obj as Material;
            if (mat == null)
                return memery;

            if (cache != null && cache.TryGetValue(path, out memery))
                return ignoreExist ? 0 : memery;

            string[] texNames = mat.GetTexturePropertyNames();
            foreach (var texName in texNames)
            {
                var tex = mat.GetTexture(texName);
                if (tex)
                    memery += GetObjectMemorySize(tex, cache, ignoreExist);
            }
            if (cache != null && !cache.ContainsKey(path))
                cache[path] = memery;
        }
        else if (type == TYPE_MODEL)
        {
            //var model = AssetDatabase.LoadAssetAtPath(assetPath, typeof(UnityEngine.Object)) as UnityEngine.Object;
            var _mesh = obj as Mesh;
            if (_mesh != null && cache != null && cache.TryGetValue(path + "/" + _mesh.name, out memery))
                return ignoreExist ? 0 : memery;

            var assets = AssetDatabase.LoadAllAssetsAtPath(path);
            List<Mesh> meshFilterList = new List<Mesh>();
            foreach (var asset in assets)
            {
                if (asset is Mesh)
                {
                    var mesh = asset as Mesh;
                    if (!mesh || meshFilterList.Contains(mesh))
                        continue;
                    float temp = Profiler.GetRuntimeMemorySizeLong(asset) / 1024f;
                    meshFilterList.Add(mesh);
                    SubMeshCount++;
                    string fullPath = path + "/" + mesh.name;
                    //Debug.Log(string.Format("Path:{0}  内存:{1}  子网格索引:{2}", path + "/" + mesh.name, temp, SubMeshCount));
                    if (obj == asset)
                        memery = temp;

                    if (cache != null && !cache.ContainsKey(path + "/" + mesh.name))
                        cache[path + "/" + mesh.name] = memery;
                }
            }
        }
        return memery;
    }
    public static string GetHierarchyPath(GameObject gobj)
    {
        return GetHierarchyPath(gobj.transform);
    }
    public static string GetHierarchyPath(Transform trans)
    {
        string path = trans.gameObject.name;
        while (trans.parent != null)
        {
            trans = trans.parent;
            path = trans.gameObject.name + "/" + path;
        }
        return path;
    }

    public static string GetGameObjectPath(Transform trans, int deep)
    {
        string path = trans.gameObject.name;
        deep--;
        while (trans.parent != null && deep > 0)
        {
            trans = trans.parent;
            path = trans.gameObject.name + "/" + path;
            deep--;
        }
        return path;
    }

    /// <summary>
    /// 保存当前场景
    /// </summary>
    /// <param name="saveAsWithEx"></param>
    /// <param name="fileNameExtension"></param>
    public static void SaveCurrentScene(bool saveAsWithEx = false, string fileNameExtension = "_streaming")
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene != null)
        {
            EditorSceneManager.MarkSceneDirty(scene);
            if (!saveAsWithEx)
            {
                EditorSceneManager.SaveOpenScenes();
                return;
            }

            EditorSceneManager.MarkSceneDirty(scene);
            string path = scene.path;
            string dir = Path.GetDirectoryName(path);
            string fileName = Path.GetFileNameWithoutExtension(path);
            string ext = Path.GetExtension(path);
            string newPath = Path.Combine(dir, fileName + fileNameExtension + ext);
            var res = EditorSceneManager.SaveScene(scene, newPath);

            scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        }
    }
    public static void MarkSceneDirty()
    {
        var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
        if (scene != null)
        {
            EditorSceneManager.MarkSceneDirty(scene);
        }
    }
    public static Scene GetActiveScene()
    {
        return UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
    }
    public static bool CurrentActiveSceneIsStreamingScene()
    {
        var scene = GetActiveScene();
        if (scene == null)
            return false;
        return scene.name.IndexOf("_streaming") >= 0;
    }

    public static GameObject GetSceneRoot()
    {
        var xscene = GameObject.Find("XScene");
        if (xscene == null)
        {
            var curScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();
            var rootGameObjects = curScene.GetRootGameObjects();
            if (rootGameObjects.Length >= 0)
                xscene = rootGameObjects[0];
        }
        return xscene;

    }

    public static void DestroyAllChild(GameObject gobj)
    {
        var childs = gobj.GetComponentsInChildren<Transform>(true);
        for (int child_index = 0; child_index < childs.Length; child_index++)
        {
            var child = childs[child_index];
            if (child == null || child.gameObject == null)
                continue;
            if (gobj.transform == child)
                continue;
            GameObject.DestroyImmediate(child.gameObject);
        }
    }
    public static string GetPorjectPath(string fullPath)
    {
        return fullPath.Substring(Application.dataPath.Length - 6).Replace("\\", "/");
    }
    public static string GetAssetFullPath(string assetPath)
    {
        string root = Application.dataPath.Substring(0, Application.dataPath.Length - 7);
        return Path.Combine(root, assetPath);
    }

    protected static void OpenWebsite(string url)
    {
        System.Diagnostics.Process.Start("explorer.exe", url);
        TextEditor textEditor = new TextEditor();
        textEditor.text = url;
        textEditor.OnFocus();
        textEditor.Copy();
        EditorUtility.DisplayDialog("Tips", "已复制链接,如果没有自动打开,请自行粘贴到浏览器打开", "ok", "cancal");
        //WWW www = new WWW(helpDecURL);
        //Application.OpenURL(www.url);
    }

    /// <summary>
    /// 有对应layer的GameObject?
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    public static bool AnyGameObjectInLayout(Transform trans, int layer)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            if (AnyGameObjectInLayout(child, layer))
                return true;
        }
        if (trans.gameObject.layer == layer)
            return true;

        return false;
    }
    /// <summary>
    /// 获取对应layer的GameObject:List
    /// </summary>
    /// <param name="trans"></param>
    /// <param name="layer"></param>
    /// <param name="gobjs"></param>
    public static void GetGameObjectInLayout(Transform trans, int layer, List<GameObject> gobjs)
    {
        for (int i = 0; i < trans.childCount; i++)
        {
            var child = trans.GetChild(i);
            GetGameObjectInLayout(child, layer, gobjs);
        }
        if (trans.gameObject.layer == layer)
            gobjs.Add(trans.gameObject);
    }
}
