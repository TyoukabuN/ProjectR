using System;
using UnityEditor;
using UnityEngine;
using System.IO;
using System.Net;
using System.Text;

public class FlagGenerator : EditorWindow
{
    [MenuItem("Tools/BitwiseFlags/Flag生成")]
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

    class TabDomain : System.IDisposable
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

        AppendLine($"private bool HasFlag(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) return (Value{i} & value) != 0;"));
            AppendLine($"return false;");
        }

        AppendLine($"private uint GetFlag(int pos)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) return Value{i};"));
            AppendLine($"return 0;");
        }

        AppendLine($"private void SetFlag(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} = value;"));
        }

        AppendLine($"private void FlagOr(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} |= value;"));
        }

        AppendLine($"private void FlagAnd(int pos, uint value)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} &= value;"));
        }

        AppendLine($"private void FlagComplement(int pos)");
        using (Tab(true, true))
        {
            ForeachFlag(i => AppendLine($"if (pos == {i}) Value{i} = ~Value{i};"));
        }

        AppendLine($"private void FlagOrExclusive(int pos, uint value)");
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

        AppendLine($"public {h.className} FlagComplement({h.className} f1)");
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
            ForeachFlag(i => AppendLine($"if (f1.Value{i} == f2.Value{i}) return false;"));
            AppendLine("return true;");
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
