#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;
using System.Collections.Generic;

public class FlagGenerator : EditorWindow
{
    [MenuItem("LS_Tool/Flag生成")]
    public static void ShowWindow()
    {
        EditorWindow myself = GetWindow<FlagGenerator>(false, "Flag Generator", true);
        myself.minSize = new Vector2(500, 200);
    }

    private int FlagBitwide = 32;
    private const string DefaultFileRoot = "Assets/Plugins/BitwiseFlags/Gen";
    private const string GenerateFolderName = "Gen";
    private const string KeyFileRoot = "FlagGenerator_KeyFileRoot";
    private const string Extension = ".cs";

    private BuildHandle h;

    class BuildHandle
    {
        public StringBuilder sb = new StringBuilder();
        public int tabCount = 0;
        public int FlagCount = 0;

        public string className;
        public string fileName;
        public string filePath;

        public void ForeachFlag(Action<int> action)
        {
            for (int i = 0; i < FlagCount; i++)
            {
                action(i);
            }
        }

        public string TabStr()
        {
            var stringBuilder = new StringBuilder();
            string tabStr = "\t";
            for (int i = 0; i < tabCount; i++)
            {
                stringBuilder.Append(tabStr);
            }

            return stringBuilder.ToString();
        }

        public void AppendLine()
        {
            sb.AppendLine(TabStr());
        }

        public void AppendLine(string str)
        {
            sb.AppendLine(TabStr() + str);
        }

        public TabDomain Tab(bool addScope = false, bool endPara = false)
        {
            return new TabDomain(this, addScope, endPara);
        }
    }

    class TabDomain : IDisposable
    {
        private BuildHandle handle;
        private bool addScope = false;
        private bool endPara = false;

        public TabDomain(BuildHandle handle, bool addScope = false, bool endPara = false)
        {
            this.handle = handle;
            this.addScope = addScope;
            this.endPara = endPara;
            if (addScope) handle.AppendLine("{");
            handle.tabCount++;
        }

        public void Dispose()
        {
            handle.tabCount--;
            if (addScope) handle.AppendLine("}");
            if (endPara) handle.AppendLine("");
        }
    }

    bool IsPowerOfTwo(ulong x)
    {
        return (x & (x - 1)) == 0;
    }

    int flagCount = 1;
    int bitCount = 32;
    private string fileRoot = string.Empty;

    private void OnGUI()
    {
        //EditorGUI.BeginDisabledGroup(true);
        GUILayout.BeginHorizontal();
        {
            fileRoot = GUILayout.TextField(fileRoot);
            if (GUILayout.Button("使用默认", GUILayout.Width(120)))
            {
                fileRoot = DefaultFileRoot;
                EditorPrefs.SetString(KeyFileRoot, DefaultFileRoot);
            }

            GUILayout.EndHorizontal();
        }

        //EditorGUI.EndDisabledGroup();

        if (string.IsNullOrEmpty(fileRoot))
        {
            fileRoot = EditorPrefs.GetString(KeyFileRoot);
            if (string.IsNullOrEmpty(fileRoot))
            {
                fileRoot = DefaultFileRoot;
            }
        }

        GUILayout.FlexibleSpace();

        EditorGUILayout.HelpBox(
            "Flag包含1+个字段value：UInt32 (称为SubFlag) \n根据位数创建：输入n 需要满足 n % 32 == 0\n根据SubFlag数创建：输入n > 0 即可",
            MessageType.Info);

        bool gen = false;
        bool accordingToByteCount = false;
        GUILayout.BeginHorizontal();
        {
            bitCount = EditorGUILayout.IntField("Flag位数：", bitCount);
            if (GUILayout.Button("根据位数创建", GUILayout.Width(120)))
            {
                gen = true;
                accordingToByteCount = true;
            }

            GUILayout.EndHorizontal();
        }

        GUILayout.BeginHorizontal();
        {
            flagCount = EditorGUILayout.IntField("SubFlag数：", flagCount);
            if (GUILayout.Button("根据SubFlag数", GUILayout.Width(120)))
            {
                gen = true;
                accordingToByteCount = false;
            }

            GUILayout.EndHorizontal();
        }

        if (gen)
        {
            gen = false;

            if (accordingToByteCount)
            {
                if (bitCount < 0)
                {
                    ShowNotification(new GUIContent("位数错误"));
                    return;
                }

                if ((bitCount % 32) != 0)
                {
                    ShowNotification(new GUIContent("位数错误"));
                    return;
                }

                flagCount = bitCount / 32;
            }
            else if (flagCount < 0)
            {
                ShowNotification(new GUIContent("SubFlag数错误"));
                return;
            }

            if (string.IsNullOrEmpty(fileRoot))
            {
                ShowNotification(new GUIContent($"路径无效： {fileRoot}"));
                Debug.LogWarning($"路径无效： {fileRoot}");
            }

            bool justCreateRoot = false;
            if (!AssetDatabase.IsValidFolder(fileRoot))
            {
                string direction = Path.GetDirectoryName(fileRoot);
                if (AssetDatabase.IsValidFolder(direction))
                {
                    AssetDatabase.CreateFolder(direction, GenerateFolderName);
                    justCreateRoot = true;
                    goto FolderValid;
                }

                ShowNotification(new GUIContent($"路径无效： {fileRoot}"));
                Debug.LogWarning($"路径无效： {fileRoot}");
                return;
            }

            FolderValid:

            EditorPrefs.SetString(KeyFileRoot, fileRoot);


            string className = string.Format("Flag{0}", FlagBitwide * flagCount);
            string fileName = className + Extension;
            string filePath = Path.Combine(fileRoot, fileName);

            string guid = AssetDatabase.AssetPathToGUID(filePath);
            bool isExist = !string.IsNullOrEmpty(guid);
            Debug.Log(isExist);
            if (isExist)
            {
                ShowNotification(new GUIContent($"已覆盖{fileName}"));
                //return;
            }

            h = new BuildHandle()
            {
                FlagCount = flagCount,
                className = className,
                fileName = fileName,
                filePath = filePath,
            };

            AppendLine("using System;");
            AppendLine("using System.Text;");
            AppendLine($"public struct {h.className} : IBitwiseFlag<{h.className}>");
            AppendLine("{");
            using (Tab())
            {
                Append_Empty();
                Append_Fields();
                Append_Functions();
            }
            AppendLine("}");

            File.WriteAllText(filePath, h.sb.ToString());

            GenFlagManager(h, fileRoot);

            if (EditorUtility.DisplayDialog("Tips", "需要强制刷新？", "ok"))
            {
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
            
                // if (justCreateRoot)
                // {
                //     if (EditorUtility.DisplayDialog("Tips", "你可能需要重新按下Ctrl+R", "ok"))
                //     {
                //         AssetDatabase.Refresh();
                //     }
                // }
            }
        }
    }
    private void GenFlagManager(BuildHandle h, string folderPath)
    {
        int bits = h.FlagCount * FlagBitwide;
        string strBits = (bits).ToString();
        string flagName = $"Flag{strBits}";
        string className = $"FlagManager{strBits}";
        string filePath = Path.Combine(fileRoot, className + Extension);

        var builder = new CSharpScriptBuilder(filePath);

        builder.AppendUsing("System.Collections.Generic");

        using (builder.BeginClass(className))
        {
            builder.AppendLine($"public static Dictionary<int, {className}> category2Mgr = new Dictionary<int, {className}>();");
            builder.AppendLine($"public static {className} Get(int category) ");
            builder.AppendLine("{");
            builder.AppendLine($"    category2Mgr = category2Mgr ?? new Dictionary<int, {className}>();");
            builder.AppendLine($"    if (!category2Mgr.TryGetValue(category, out var mgr))");
            builder.AppendLine("    {");
            builder.AppendLine($"        mgr = new {className}(category);");
            builder.AppendLine($"        category2Mgr.Add(category, mgr);");
            builder.AppendLine("    }");
            builder.AppendLine($"    return mgr;");
            builder.AppendLine("}");

            builder.AppendLine($"public static {flagName} GetFlag(int category, string idStr)");
            builder.AppendLine("{");
            builder.AppendLine($"    if (category <= 0 || string.IsNullOrEmpty(idStr))");
            builder.AppendLine($"        return {flagName}.Empty;");
            builder.AppendLine($"    var mgr = Get(category);");
            builder.AppendLine($"    if (mgr == null)");
            builder.AppendLine($"        return {flagName}.Empty;");
            builder.AppendLine($"    return mgr.StringToFlag(idStr);");
            builder.AppendLine("}");
            builder.AppendLine($"public static {flagName} GetFlag(FlagDefine flagDefine)");
            builder.AppendLine("{");
            builder.AppendLine($"    if (flagDefine == null)");
            builder.AppendLine($"        return {flagName}.Empty;");
            builder.AppendLine($"    return GetFlag(flagDefine.CategoryID, flagDefine.IDStr);");
            builder.AppendLine("}");
            builder.AppendLine($"public static {flagName} GetFlag(int id)");
            builder.AppendLine("{");
            builder.AppendLine($"    FlagIDInfo info = new FlagIDInfo(id);");
            builder.AppendLine($"    if (!info.IsValid())");
            builder.AppendLine($"        return {flagName}.Empty;");
            builder.AppendLine($"    return GetFlag(info.categoryID, info.flagIDStr);");
            builder.AppendLine("}");
            builder.AppendLine($"public static {flagName} GetFlag(params int[] ids)");
            builder.AppendLine("{");
            builder.AppendLine($"    var res = {flagName}.Empty;");
            builder.AppendLine($"    foreach (var id in ids)");
            builder.AppendLine($"        res.FlagOr(GetFlag(id));");
            builder.AppendLine($"    return res;");
            builder.AppendLine("}");
            builder.AppendEmptyLine();

            builder.AppendLine($"public Dictionary<string, {flagName}> ActionEnum2Flag => _string2Flag;");
            builder.AppendLine($"private Dictionary<string, {flagName}> _string2Flag;");
            builder.AppendLine($"private Dictionary<int, {flagName}> _flagBitIndex2Flag;");
            builder.AppendLine($"private int _bitCount = -1;");
            builder.AppendLine($"private const int TotalBitCount = {bits};");
            builder.AppendLine($"private const int BITUnit = 32;");
            builder.AppendEmptyLine();
            builder.AppendLine($"public int category = -1;");

            builder.AppendLine($"public {className}()");
            builder.AppendLine("{");
            builder.AppendLine($"    _string2Flag = new Dictionary<string, {flagName}>();");
            builder.AppendLine($"    _flagBitIndex2Flag = new Dictionary<int, {flagName}>();");
            builder.AppendLine("}");

            builder.AppendLine($"public {className}(int category) : this()");
            builder.AppendLine("{");
            builder.AppendLine("    this.category = category;");
            builder.AppendLine("}");

            builder.AppendLine($"public {flagName} StringToFlag(List<string> keys) => StringToFlag(keys.ToArray());");

            builder.AppendLine($"public {flagName} StringToFlag(params string[] keys)");
            builder.AppendLine("{");
            builder.AppendLine($"    var temp = {flagName}.Empty;");
            builder.AppendLine("    for (int i = 0; i < keys.Length; i++)");
            builder.AppendLine("        temp |= StringToFlag(keys[i]);");
            builder.AppendLine("    return temp;");
            builder.AppendLine("}");

            builder.AppendLine($"public {flagName} StringToFlag(string key)");
            builder.AppendLine("{");
            builder.AppendLine($"    var flag = {flagName}.Empty;");
            builder.AppendLine("    int targetBitIndex = _bitCount + 1;");
            builder.AppendLine("    if (targetBitIndex > TotalBitCount)");
            builder.AppendLine("    {");
            builder.AppendLine("        targetBitIndex = -1;");
            builder.AppendLine("        return flag;");
            builder.AppendLine("    }");
            builder.AppendLine("");
            builder.AppendLine($"    _string2Flag ??= new Dictionary<string, {flagName}>(TotalBitCount);");
            builder.AppendLine("");
            builder.AppendLine("    if (string.IsNullOrEmpty(key))");
            builder.AppendLine("    {");
            builder.AppendLine("        targetBitIndex = -1;");
            builder.AppendLine($"        return {flagName}.Empty;");
            builder.AppendLine("    }");
            builder.AppendLine("");
            builder.AppendLine("    if (!_string2Flag.TryGetValue(key, out flag))");
            builder.AppendLine("    {");
            builder.AppendLine("        flag = GetFlagByBitIndex(targetBitIndex);");
            builder.AppendLine("        _string2Flag[key] = flag;");
            builder.AppendLine("        _bitCount++;");
            builder.AppendLine("    }");
            builder.AppendLine("    return flag;");
            builder.AppendLine("}");

            builder.AppendLine($"private {flagName} GetFlagByBitIndex(int flagBitIndex)");
            builder.AppendLine("{");
            builder.AppendLine($"    _flagBitIndex2Flag ??= new Dictionary<int, {flagName}>(TotalBitCount);");
            builder.AppendLine($"    if (_flagBitIndex2Flag.TryGetValue(flagBitIndex, out {flagName} res))");
            builder.AppendLine("        return res;");
            builder.AppendLine("    int pos = flagBitIndex / BITUnit;");
            builder.AppendLine($"    var temp = {flagName}.Empty;");
            using (builder.Tab(false, false))
            {
                ForeachFlag(index =>
                {
                    builder.AppendLine($"if (pos == {index}) temp.Value{index} = (uint)(1 << flagBitIndex);");
                });
            }
            builder.AppendLine("    _flagBitIndex2Flag[flagBitIndex] = temp;");
            builder.AppendLine("    return temp;");
            builder.AppendLine("}");
        }
        builder.Gen();
    }


    public void ForeachFlag(Action<int> action)
    {
        h.ForeachFlag(action);
    }

    private void AppendLine()
    {
        h.AppendLine();
    }

    private void AppendLine(string str)
    {
        h.AppendLine(str);
    }

    private TabDomain Tab(bool addScope = false, bool endPara = false)
    {
        return h.Tab(addScope, endPara);
    }

    #region Appends

    private void Append_Empty()
    {
        AppendLine($"public static readonly {h.className} Empty =");
        using (Tab())
        {
            AppendLine("new(){");
            using (Tab())
            {
                ForeachFlag(i => AppendLine($"Value{i} = 0,"));
            }

            AppendLine("};");
        }

        AppendLine();
    }

    private void Append_Fields()
    {
        ForeachFlag(i => AppendLine($"public uint Value{i};"));
        AppendLine();

        AppendLine($"public {h.className} GetEmpty()");
        using (Tab(true, true))
        {
            AppendLine($"return Empty;");
        }
        AppendLine();
    }

    private void Append_Functions()
    {
        AppendLine($"public bool HasAny({h.className} f)");
        using (Tab(true, true))
        {
            AppendLine($"return (this & f);");
        }

        AppendLine($"public bool HasAll({h.className} f)");
        using (Tab(true, true))
        {
            AppendLine($"return (this & f) == f;");
        }

        AppendLine($"public bool HasFlag(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) return (Value{i} & value) != 0;"));
            AppendLine($"return false;");
        }

        AppendLine($"public uint GetFlag(int pos)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) return Value{i};"));
            AppendLine($"return 0;");
        }

        AppendLine($"public void SetFlag(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} = value;"));
        }

        AppendLine($"public void FlagOr(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} |= value;"));
        }

        AppendLine($"public void FlagAnd(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} &= value;"));
        }

        AppendLine($"public void FlagComplement(int pos)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} = ~Value{i};"));
        }

        AppendLine($"public void FlagOrExclusive(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} = Value{i} ^ value;"));
        }

        ///////-----for interface
        AppendLine($"public {h.className} FlagOr({h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"FlagOr({i}, f2.Value{i});"));
            AppendLine("return this;");
        }

        AppendLine($"public {h.className} FlagOrExclusive({h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"FlagOrExclusive({i}, f2.Value{i});"));
            AppendLine("return this;");
        }

        AppendLine($"public {h.className} FlagAnd({h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"FlagAnd({i}, f2.Value{i});"));
            AppendLine("return this;");
        }

        AppendLine($"public {h.className} FlagComplement()");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"FlagComplement({i});"));
            AppendLine("return this;");
        }

        AppendLine($"public bool Equals({h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (Value{i} != f2.Value{i}) return false;"));
            AppendLine("return true;");
        }

        ///////-----operator rewrite

        AppendLine($"public static {h.className} operator | ({h.className} f1, {h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"f1.FlagOr({i}, f2.Value{i});"));
            AppendLine("return f1;");
        }

        AppendLine($"public static {h.className} operator ^ ({h.className} f1, {h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"f1.FlagOrExclusive({i}, f2.Value{i});"));
            AppendLine("return f1;");
        }

        AppendLine($"public static {h.className} operator & ({h.className} f1, {h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"f1.FlagAnd({i}, f2.Value{i});"));
            AppendLine("return f1;");
        }

        AppendLine($"public static {h.className} operator ~ ({h.className} f1)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"f1.FlagComplement({i});"));
            AppendLine("return f1;");
        }

        AppendLine($"public static bool operator == ({h.className} f1, {h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (f1.Value{i} != f2.Value{i}) return false;"));
            AppendLine("return true;");
        }

        AppendLine($"public static bool operator != ({h.className} f1, {h.className} f2)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (f1.Value{i} != f2.Value{i}) return true;"));
            AppendLine("return false;");
        }

        AppendLine($"public static implicit operator bool({h.className} f)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (f.Value{i} > 0) return true;"));
            AppendLine("return false;");
        }

        ///////-----other function 
        AppendLine($"public override string ToString()");
        using (Tab(true, true))
        {
            AppendLine("var sb = new StringBuilder();");
            AppendLine($"sb.AppendLine($\"[ToString] {{nameof({h.className})}}\");");
            ForeachFlag(i => AppendLine($"sb.AppendLine(Convert.ToString(Value{i}, 2));"));
            AppendLine("return sb.ToString();");
        }

        AppendLine($"public bool IsEmpty()");
        using (Tab(true, true))
        {
            AppendLine("return this == Empty;");
        }
    }

    #endregion


}

#endif