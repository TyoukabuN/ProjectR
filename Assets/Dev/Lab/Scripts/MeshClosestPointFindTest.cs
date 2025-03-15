using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;

public class MeshClosestPointFindTest : MonoBehaviour
{
    public SkinnedMeshRenderer skinnedMeshRenderer;
    public ComputeShader computeShader;
    public Transform referTransform;

    public bool isDebug = false;
    [DisableIf("@true")]
    public bool setuped = false;

    ComputeBuffer vertexBuf;
    ComputeBuffer indexBuf;
    ComputeBuffer closestPointBuf;
    ComputeBuffer debugBuf;


    public void OnEnable()
    {
        Clearup();
        Setup();
    }

    public void Update()
    {
        if(!IsValid())
            return;
        FindPoint();
    }

    public void Register(bool enable)
    {
        EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
        if(enable)
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    public void OnPlayModeStateChanged(PlayModeStateChange playerModeStateChange)
    {
        if (playerModeStateChange == PlayModeStateChange.ExitingPlayMode)
            Clearup();
        if (playerModeStateChange == PlayModeStateChange.EnteredPlayMode)
            Clearup();
    }

    public const int GroudThreadsX = 1024;
    public void Setup()
    {
        if (meshHandle == null)
            meshHandle = new MeshHandle(skinnedMeshRenderer);
        meshHandle.Bake();

        vertexBuf ??= new ComputeBuffer(meshHandle.vertexCount, Marshal.SizeOf<Vector3>());
        indexBuf ??= new ComputeBuffer(meshHandle.indexCount, Marshal.SizeOf<int>());
        closestPointBuf ??= new ComputeBuffer(groudThreadsX, Marshal.SizeOf<Vector3>());
        debugBuf ??= new ComputeBuffer(meshHandle.indexCount, Marshal.SizeOf<Vector4>());
        //
        computeShader.SetBuffer(0, "vertexBuf", vertexBuf);
        computeShader.SetBuffer(0, "indexBuf", indexBuf);
        computeShader.SetBuffer(0, "closestPointBuf", closestPointBuf);
        computeShader.SetBuffer(0, "debugBuf", debugBuf);

        setuped = true;
    }
    public bool IsValid()
    {
        if (skinnedMeshRenderer.sharedMesh == null)
            return false;
        if (computeShader == null)
            return false;
        return true;
    }

    public int groudThreadsX = 16;
    public Vector3[] closestPoints;
    public static Vector3 Invalid = new Vector3(999999, 999999, 999999);


    private MeshHandle meshHandle;
    //private List<MeshUtil.Triangle> triangleStructs;

    public void FindPoint()
    {
        Profiler.BeginSample("FindPoint");
        if (!IsValid() || !setuped)
            return;
        meshHandle.Bake();
        var local2WorldMatrix = meshHandle.SkinnedMeshRenderer.localToWorldMatrix;
        int taskAmount = meshHandle.indexCount / 3;
        int triangleCount = meshHandle.indexCount / 3;

        Vector3 checkPoint = this.referTransform != null ? this.referTransform.transform.position : Vector3.zero;

        //给Buffer填SetData
        vertexBuf.SetData(meshHandle.vertices);
        indexBuf.SetData(meshHandle.triangles);

        closestPoints = new Vector3[groudThreadsX];
        Array.Fill(closestPoints, Invalid);
        closestPointBuf.SetData(closestPoints);

        var debug = new Vector4[taskAmount];
        debugBuf.SetData(debug);

        //
        computeShader.SetInt("taskAmount", taskAmount);
        computeShader.SetVector("checkPoint", new Vector4(checkPoint.x, checkPoint.y, checkPoint.z, 0));
        computeShader.SetMatrix("local2world", local2WorldMatrix);

        computeShader.Dispatch(0, Mathf.CeilToInt(taskAmount / 1024.0f), 1, 1);

        closestPointBuf.GetData(closestPoints);
        Vector3 p1 = GetClosestPointReferTo(closestPoints, checkPoint);
        Debug.DrawLine(checkPoint, p1, Color.red, 1f);

        debugBuf.GetData(debug);

        meshHandle.Dispose();
        Profiler.EndSample();
    }
    public Vector3 GetClosestPointReferTo(Vector3[] point, Vector3 referPoint)
    {
        Vector3 p = Vector3.zero;
        float closestDistanceSqr = Mathf.Infinity;

        for (int i = 0; i < point.Length; i++)
        {
            float distanceSqr = (referPoint - point[i]).sqrMagnitude;

            if (distanceSqr < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqr;
                p = point[i];
            }
        }

        return p;
    }

    public void Clearup()
    {
        indexBuf?.Dispose();
        closestPointBuf?.Dispose();
        debugBuf?.Dispose();

        indexBuf = null;
        closestPointBuf = null;
        debugBuf = null;

        meshHandle?.Dispose();
        meshHandle = null;

        setuped = false;
    }

    [Button]
    public void Test()
    {
        Clearup();
        Setup();
        FindPoint();
    }
    public struct Vertex
    {
        public Vector3 pos;
        public Vector3 us;
    }

    public class MeshHandle : IDisposable
    {
        public SkinnedMeshRenderer SkinnedMeshRenderer
        {
            get=> _skinnedMeshRenderer;
            set
            {
                if (_skinnedMeshRenderer == value)
                    return;
                _skinnedMeshRenderer = value;
                triangles = null;
            }
        }

        private SkinnedMeshRenderer _skinnedMeshRenderer;


        public List<Vector3> vertices;
        public int[] triangles;
        public int vertexCount => vertices.Count;
        public int indexCount => triangles.Length;

        public Mesh mesh = new Mesh();

        public MeshHandle(SkinnedMeshRenderer skinnedMeshRenderer)
        {
            this.SkinnedMeshRenderer = skinnedMeshRenderer;
        }

        public void Bake()
        {
            SkinnedMeshRenderer.BakeMesh(mesh);
            vertices ??= new List<Vector3>();

            mesh.GetVertices(vertices);
            if (triangles == null)
                triangles = mesh.triangles;
        }

        public void Dispose()
        {
            vertices = null;
            triangles = null;
        }
    }

    public static class MeshUtil
    {
        public struct Triangle
        {
            public Vector3 a;
            public Vector3 b;
            public Vector3 c;
        }
        public static Triangle[] trianglePoints = null;
        public static Triangle[] FillTriangle(Mesh mesh) => FillTriangle(mesh.vertices, mesh.triangles);
        public static Triangle[] FillTriangle(Vector3[] vertices, int[] triangles)
        {
            trianglePoints = new Triangle[triangles.Length / 3];
            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                trianglePoints[j].a = vertices[triangles[i]];
                trianglePoints[j].b = vertices[triangles[i + 1]];
                trianglePoints[j].c = vertices[triangles[i + 2]];
            }
            return trianglePoints;
        }
        public static void FillTriangle(Mesh mesh, List<Triangle> triangleStructs) => FillTriangle(mesh.vertices, mesh.triangles, triangleStructs);
        public static void FillTriangle(Vector3[] vertices, int[] triangles, List<Triangle> triangleStructs)
        {
            for (int i = 0, j = 0; i < triangles.Length; i += 3, j++)
            {
                triangleStructs[j] = new Triangle
                {
                    a = vertices[triangles[i]],
                    b = vertices[triangles[i + 1]],
                    c = vertices[triangles[i + 2]]
                };
            }
        }
        public static Triangle[] FillTriangle(List<Vector3> vertices, List<int> triangles)
        {
            trianglePoints = new Triangle[triangles.Count / 3];
            for (int i = 0, j = 0; i < triangles.Count; i += 3, j++)
            {
                trianglePoints[j].a = vertices[triangles[i]];
                trianglePoints[j].b = vertices[triangles[i + 1]];
                trianglePoints[j].c = vertices[triangles[i + 2]];
            }
            return trianglePoints;
        }
        public static Triangle[] FillTriangle(Mesh mesh, Matrix4x4 local2World)
        {
            trianglePoints = new Triangle[mesh.triangles.Length / 3];
            for (int i = 0, j = 0; i < mesh.triangles.Length; i += 3, j++)
            {
                trianglePoints[j].a = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i]]);
                trianglePoints[j].b = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 1]]);
                trianglePoints[j].c = local2World.MultiplyPoint(mesh.vertices[mesh.triangles[i + 2]]);
            }
            return trianglePoints;
        }
    }
}
