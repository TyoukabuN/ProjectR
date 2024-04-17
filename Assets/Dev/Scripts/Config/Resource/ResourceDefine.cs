#if UNITY_EDITOR
using Sirenix.Utilities;
using UnityEditor;
using UnityEditor.Build.Pipeline.Interfaces;
#endif

namespace PJR
{
    public partial class ResourceDefine
    {
        [MenuItem("PJR/资源/生成资源列表文件ResourceDefine.Gen")]
        static void GenAssetList()
        {
            string filters = " t:ScriptableObject t:Prefab";
            string[] searchInFolders = new string[]{
                "Assets/Art",
                "Assets/Dev/Prefabs",
            };
            var guids = AssetDatabase.FindAssets(filters, searchInFolders);

            var builder = new CSharpScriptBuilder("Assets/Dev/Scripts/Gen/ResourceDefine.Gen.cs");
            using (builder.BeginNameSpace("PJR")) {
                using (builder.BeginClass(nameof(ResourceDefine),"public partial")) {
                    builder.AppendLine("public static string[] Assets = ");
                    using (builder.Tab(true))
                    {
                        guids.ForEach(guid =>
                        {
                            builder.AppendLine($"\"{AssetDatabase.GUIDToAssetPath(guid)}\",");
                        });
                    }
                    builder.Append(";");
                }
            }
            builder.Gen();
        }
    }
}
