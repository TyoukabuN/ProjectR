using UnityEditor;
using UnityEngine;
using YooAsset.Editor;

public static class BuildTest
{
    [MenuItem("Test/SimulateBuildTest")]
    public static void SimulateBuildTest()
    {
        string buildPipelineName = "BuiltinBuildPipeline";
        string packageName = "DefaultPackage";
        string manifestFilePath = AssetBundleSimulateBuilder.SimulateBuild(buildPipelineName, packageName);
        Debug.Log($"[manifestFilePath]£º{manifestFilePath} ");
    }
}

