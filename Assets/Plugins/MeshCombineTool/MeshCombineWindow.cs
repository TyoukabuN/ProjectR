using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class MeshCombineWindow : EditorWindow
{
    [MenuItem("Tools/CombineMesh")]
    static void Open()
    { 
        EditorWindow.GetWindow(typeof(MeshCombineWindow));
    }

    public static Mesh Mesh1 = null;
    public static Mesh Mesh2 = null;
    void OnGUI()
    {
        Mesh1 = EditorGUILayout.ObjectField("Mesh1",Mesh1,typeof(Mesh),false) as Mesh;
        Mesh2 = EditorGUILayout.ObjectField("Mesh2", Mesh2, typeof(Mesh),false) as Mesh;
        if (GUILayout.Button("Run"))
        {
            if (Mesh1 != null && Mesh2 != null)
            { 
                Mesh mesh = CombineMeshes(Mesh1, Mesh2);
                string asestPath = AssetDatabase.GetAssetPath(Mesh1);
                string assetName = Path.GetFileNameWithoutExtension(asestPath);
                var folder = Path.GetDirectoryName(asestPath);
                string name = string.Format("{0}_Combined.mesh", assetName);
                AssetDatabase.CreateAsset(mesh, AssetDatabase.GenerateUniqueAssetPath(Path.Combine(folder, name)));
            }
        }
    }
    Mesh CombineMeshes(Mesh mesh1, Mesh mesh2)
    {
        // 获取第一个网格的数据
        Vector3[] vertices1 = mesh1.vertices;
        Vector3[] normals1 = mesh1.normals;
        Vector2[] uv1 = mesh1.uv;
        BoneWeight[] boneWeights1 = mesh1.boneWeights;
        int[] triangles1 = mesh1.triangles;

        // 获取第二个网格的数据
        Vector3[] vertices2 = mesh2.vertices;
        Vector3[] normals2 = mesh2.normals;
        Vector2[] uv2 = mesh2.uv;
        BoneWeight[] boneWeights2 = mesh2.boneWeights;
        int[] triangles2 = mesh2.triangles;

        // 合并顶点数据
        Vector3[] vertices = new Vector3[vertices1.Length + vertices2.Length];
        vertices1.CopyTo(vertices, 0);
        vertices2.CopyTo(vertices, vertices1.Length);

        // 合并法线数据
        Vector3[] normals = new Vector3[normals1.Length + normals2.Length];
        normals1.CopyTo(normals, 0);
        normals2.CopyTo(normals, normals1.Length);

        // 合并 UV 数据
        Vector2[] uv = new Vector2[uv1.Length + uv2.Length];
        uv1.CopyTo(uv, 0);
        uv2.CopyTo(uv, uv1.Length);

        // 合并 boneWeights 数据
        Vector2[] boneWeights = new Vector2[uv1.Length + uv2.Length];
        boneWeights1.CopyTo(boneWeights, 0);
        boneWeights2.CopyTo(boneWeights, boneWeights1.Length);

        // 合并三角形数据
        int[] triangles = new int[triangles1.Length + triangles2.Length];
        triangles1.CopyTo(triangles, 0);
        for (int i = 0; i < triangles2.Length; i++)
        {
            triangles[i + triangles1.Length] = triangles2[i] + vertices1.Length;
        }

        // 创建并返回合并后的网格
        Mesh combinedMesh = new Mesh();
        combinedMesh.vertices = vertices;
        combinedMesh.normals = normals;
        combinedMesh.uv = uv;

        // 分别设置索引（子网格）
        combinedMesh.subMeshCount = 2;
        combinedMesh.SetIndices(triangles1, MeshTopology.Triangles, 0);
        combinedMesh.SetIndices(triangles, triangles1.Length, triangles.Length - triangles1.Length, MeshTopology.Triangles, 1);

        return combinedMesh;
    }
}
